using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Character : MonoBehaviour, ICharacter
    {
        #region Properties
        protected Transform m_Transform;
        public Transform Transform { get { return m_Transform; } }

        public float Health { get { return m_Health; } set { m_Health = value; } }

        //Components
        public Rigidbody2D RigidBody { get { return m_RigidBody; } }
        protected Rigidbody2D m_RigidBody;
        protected Animator m_Animator;
        protected SpriteRenderer m_Renderer;
        public Collider2D Collider { get { return m_Collider; } }
        protected Collider2D m_Collider;

        protected Character m_Opponent;

        //Player variables
        [SerializeField]
        protected float m_Health = 200;
        [SerializeField]
        protected Vector2 m_BodyOffset = Vector2.zero;
        [SerializeField]
        protected float m_NormalGravity = 1f;
        [SerializeField]
        protected float m_FallingGravity = 5f;
        [SerializeField]
        protected float m_GroundDrag = 15f;
        [SerializeField]
        protected float m_AirDrag = 3f;
        [SerializeField]
        protected int m_GroundSpeed = 100;
        [SerializeField]
        protected int m_AirSpeed = 25;
        [SerializeField]
        protected int m_JumpForce = 1500;
        [SerializeField]
        protected SpritePalette m_ActivePalette;
        [SerializeField]
        protected SpritePaletteGroup m_PaletteGroup;
        [SerializeField]
        protected float m_BasePoise = 1f;
        [SerializeField]
        protected LayerMask m_GroundLayer;
        [SerializeField]
        protected AudioSource m_HitSound;
        [SerializeField]
        protected AudioSource m_FallSound;
        [SerializeField]
        protected AudioSource[] m_Sound;

        protected bool m_Grounded;
        protected float m_Poise;
        protected float m_AnimationMovementModifier = 1f;
        protected bool m_AnimationAllowDirectionChange = true;
        protected bool m_IsGuarding;
        protected int m_DefaultLayer;
        protected Color m_DefaultColor;
        protected AudioSource m_AudioSource;
        protected WaitForSeconds HitEffectWait = new WaitForSeconds(0.05f);
        protected WaitForSeconds m_HitStopDuration = new WaitForSeconds(0.15833f);
        protected WaitForSeconds m_KnockdownDuration = new WaitForSeconds(1f);

        private HitboxManager m_HitboxManager;

        public bool BounceEnabled { get; set; }

        public SpritePalette ActivePalette { get { return m_ActivePalette; } }
        public SpritePaletteGroup PaletteGroup { get { return m_PaletteGroup; } }

        public float Poise
        {
            get { return m_BasePoise + m_Poise; }
            set { m_Poise = value; }
        }

        protected bool IsGuarding
        {
            get { return m_IsGuarding; }
            set { m_Animator.SetBool("guard", m_IsGuarding = value); }
        }
        protected bool IsBackHeld { get; set; }

        protected bool UpAction { set { m_Animator.SetBool("upaction", value); } }

        protected bool LockFlip { get; set; }

        public bool FlipX
        {
            get { return m_Renderer.flipX; }
            protected set { if(LockFlip == false) m_Renderer.flipX = value; }
        }

        public string CurrentAnimationName
        {
            get
            {
                var clipinfo = m_Animator.GetCurrentAnimatorClipInfo(0);
                return (clipinfo == null || clipinfo.Length == 0) ? "" : clipinfo[0].clip.name;
            }
        }

        public INPUTFLAG InputState { get; protected set; }

        public Queue<UnityAction> PhysicsActionStack { get { return m_PhysicsActions; } }
        //Physics action queue
        protected Queue<UnityAction> m_PhysicsActions = new Queue<UnityAction>(16);
        protected ContactFilter2D m_TerrainFilter;

        public Vector2 JoystickInput { get; set; }

        protected ContactPoint2D[] m_GroundedResults = new ContactPoint2D[4];
        protected Vector2 m_GroundNormal = new Vector2(0f, 1f);
        protected int lastGroundedFrame;
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            m_Transform = transform;
            m_RigidBody = GetComponent<Rigidbody2D>();
            m_Animator = GetComponent<Animator>();
            m_Renderer = GetComponent<SpriteRenderer>();
            m_Collider = GetComponent<Collider2D>();
            m_HitboxManager = GetComponent<HitboxManager>();
            m_AudioSource = GetComponent<AudioSource>();
            m_DefaultLayer = gameObject.layer;
            m_TerrainFilter.useLayerMask = true;
            m_TerrainFilter.layerMask = m_GroundLayer;
            m_TerrainFilter.useNormalAngle = true;
            m_TerrainFilter.minNormalAngle = 30f;
            m_TerrainFilter.maxNormalAngle = 150f;
            m_DefaultColor = m_Renderer.color;

            SetPalette(m_ActivePalette);
        }

        protected virtual void Start()
        {
            var characters = FindObjectsOfType<Character>();

            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != this)
                {
                    m_Opponent = characters[i];
                    break;
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            while (m_PhysicsActions.Count > 0)
                m_PhysicsActions.Dequeue()();

            float xVel = JoystickInput.x;
            IsBackHeld = Mathf.Abs(xVel) < 0.1f ? false : (Mathf.Sign(xVel) != Mathf.Sign(m_Opponent.m_Transform.position.x - m_Transform.position.x));
            bool jumpheld = checkflag(InputState, INPUTFLAG.UP);
            UpAction = checkflag(InputState, INPUTFLAG.UP);
            m_Grounded = IsGrounded();

            if (IsBackHeld)
                xVel *= 0.75f;

            var opponentClipName = m_Opponent.CurrentAnimationName;

            IsGuarding = IsBackHeld && opponentClipName.Contains("atk");

            if (!m_Grounded && (!jumpheld || m_RigidBody.velocity.y < 0))
            {
                m_RigidBody.gravityScale = m_FallingGravity;
            }
            else
            {
                m_RigidBody.gravityScale = m_NormalGravity;
            }

            if (m_Grounded)
            {
                m_Collider.offset = new Vector2(m_BodyOffset.x, m_BodyOffset.y);
                m_RigidBody.drag = m_GroundDrag;

                m_RigidBody.AddForce(Vector2.right * xVel * m_GroundSpeed * m_AnimationMovementModifier);
            }
            else
            {
                m_Collider.offset = new Vector2(m_BodyOffset.x, m_BodyOffset.y);
                m_RigidBody.drag = m_AirDrag;
                m_RigidBody.AddForce(new Vector2(xVel * m_AirSpeed * m_AnimationMovementModifier, 0f));
            }

            m_Animator.SetFloat("speed", Mathf.Abs(m_RigidBody.velocity.x) + Mathf.Abs(m_RigidBody.velocity.y));
            m_Animator.SetFloat("xvelocity", xVel * (FlipX ? 1f : -1f));
            m_Animator.SetBool("crouch", JoystickInput.y < -0.5f);
            m_Animator.SetBool("ground", m_Grounded);

            UpdateDirection();
        }

        protected bool IsGrounded()
        {
            //return Physics2D.OverlapCircleNonAlloc(transform.position - new Vector3(0f, -1.5f), 0.1f, m_Results) > 1;

            
            var count = m_RigidBody.GetContacts(m_TerrainFilter, m_GroundedResults);

            if (count > 0)
            {
                Vector2 normal = Vector3.zero;

                for (int i = 0; i < count; i++)
                    normal += m_GroundedResults[i].normal;

                m_GroundNormal = normal / count;

                lastGroundedFrame = Mathf.Clamp(lastGroundedFrame - 1, 0, 7);
            }
            else
                lastGroundedFrame = Mathf.Clamp(lastGroundedFrame + 1, 0, 7);

            return lastGroundedFrame < 5;
        }

        public void UpdateDirection()
        {
            if(m_AnimationAllowDirectionChange == true && m_AnimationMovementModifier > 0f && m_Opponent != null)
            {
                var delta = m_Transform.position.x - m_Opponent.m_Transform.position.x;

                FlipX = delta > 0 ? true : false;

            }
            /*Use this to make this characters control more like a beat'em up
             * if (Mathf.Abs(JoystickInput.x) >= 0.05f && m_AnimationMovementModifier > 0f)
            {
                if (JoystickInput.x <= 0.05f && FlipX == false)
                    FlipX = true;
                else if (JoystickInput.x >= 0.05f && FlipX == true)
                    FlipX = false;
            }*/
        }

        public void Rebind()
        {
            if(m_Animator != null)
                m_Animator.Rebind();
        }

        protected bool checkflag(INPUTFLAG state, INPUTFLAG flag) { return (state & flag) != 0; }

        private IEnumerator HitEffect(Color color)
        {
            m_Renderer.color = color;
            yield return HitEffectWait;
            m_Renderer.color = Color.Lerp(color, m_DefaultColor, 0.5f);
            yield return HitEffectWait;
            m_Renderer.color = m_DefaultColor;
        }

        public void CancelGrab()
        {
            PlayHitSound(4f);
            m_Animator.SetTrigger("stagger");
            m_Animator.ResetTrigger("grab_confirm");
            m_RigidBody.AddForce(new Vector2(m_JumpForce * (FlipX ? 1f : -1f), 0f));
            EffectSpawner.PlayHitEffect(2, (m_Transform.position + m_Opponent.m_Transform.position) / 2f,
                m_Renderer.sortingOrder + 1, FlipX);
        }

        public void PlayHitSound(float basePitch)
        {
            if(m_AudioSource != null)
            {
                m_AudioSource.pitch = UnityEngine.Random.Range(basePitch - 0.1f, basePitch + 0.1f);
                m_AudioSource.Play();
            }
        }

        protected IEnumerator Hitstop()
        {
            m_Animator.SetFloat("playbackspeed", 0f);
            yield return m_HitStopDuration;
            m_Animator.SetFloat("playbackspeed", 1f);
        }

        protected IEnumerator Wakeup()
        {
            yield return m_KnockdownDuration;
            if (Health > 0)
                m_Animator.SetTrigger("wakeup");
        }

        public void SetPalette(SpritePalette palette)
        {
            m_ActivePalette = palette;

            if (palette != null)
            {
                var block = new MaterialPropertyBlock();

                if (m_Renderer == null) m_Renderer = GetComponent<SpriteRenderer>();
                m_Renderer.GetPropertyBlock(block);
                block.SetTexture("_SwapTex", palette.Texture);
                m_Renderer.SetPropertyBlock(block);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.relativeVelocity.magnitude > 5 &&
                collision.collider.name.Equals("Boundary") &&
                BounceEnabled)
            {
                m_FallSound.Play();
#if UNITY_2018_2_OR_NEWER
                m_RigidBody.AddForce(collision.relativeVelocity * new Vector2(0.25f, 1f) * 30f);
#else
                m_RigidBody.AddForce(Vector2.Scale(collision.relativeVelocity, new Vector2(0.25f, 1f) * 30f));
#endif
            }
        }

        #endregion

        #region Events

        public virtual void HitboxContact(ContactData data)
        {
            switch (data.MyHitbox.Type)
            {
                case HitboxType.ARMOR:
                    if (data.TheirHitbox.Type == HitboxType.TECH)
                        OnHitReceived(data.Damage, data.PoiseDamage, data.Force);
                    break;
                case HitboxType.TRIGGER:
                    {
                        if (data.TheirHitbox.Type == HitboxType.GRAB)
                            m_Animator.SetTrigger("stagger");
                        else
                        {
                            Character enemy = (Character)data.TheirHitbox.Owner;

                            enemy.OnAttackHit();
                            OnHitReceived(data.Damage, data.PoiseDamage, data.Force);
                            PlayHitSound(2f);
                            EffectSpawner.PlayHitEffect(data.fxID, data.Point, m_Renderer.sortingOrder + 1, !data.TheirHitbox.Owner.FlipX);
                        }
                    }
                    break;
                case HitboxType.GUARD:
                    {
                        if (data.TheirHitbox.Type == HitboxType.GRAB)
                            m_Animator.SetTrigger("stagger");
                        else
                        {
                            Character enemy = (Character)data.TheirHitbox.Owner;

                            OnGuardReceived(enemy.m_Transform.position);
                            enemy.OnAttackGuarded();
                            PlayHitSound(1f);
                            //2 == block FX
                            EffectSpawner.PlayHitEffect(2, data.Point, m_Renderer.sortingOrder + 1, !data.TheirHitbox.Owner.FlipX);
                        }

                    }
                    break;
                case HitboxType.GRAB:
                    {
                        if(data.TheirHitbox.Type == HitboxType.GRAB ||
                            data.TheirHitbox.Type == HitboxType.TECH)
                        {
                            CancelGrab();
                        }
                        else
                        {
                            m_Animator.SetTrigger("grab_confirm");
                            //attach the opponent to my hand through events and lock him in the stagger state
                        }
                    }
                    break;
                case HitboxType.TECH:
                    if (data.TheirHitbox.Type == HitboxType.TECH)
                        CancelGrab();
                    break;
            }
        }

        public void EVENT_CLEAR_INPUTS()
        {
            m_Animator.ResetTrigger("punch");
            m_Animator.ResetTrigger("kick");
            m_Animator.ResetTrigger("jump");
            m_Animator.ResetTrigger("grab");
            m_Animator.ResetTrigger("grab_confirm");
            LockFlip = false;
        }

        public void EVENT_ENABLE_COLLISIONS()
        {
            m_Collider.enabled = true;
        }

        public void EVENT_DISABLE_COLLISIONS()
        {
            m_Collider.enabled = false;
        }

        public void EVENT_PLAY_SOUND(int index)
        {
            if (index < 0 || index >= m_Sound.Length) return;

            m_Sound[index].Play();
        }

        public virtual void OnHitReceived(float damage, float strength, Vector2 force)
        {
            m_Health -= damage;
            if (m_Grounded == false)
                force *= 0.85f;

            var poise = Poise - strength;

            if (m_Health > 0f)
            {
                if (poise <= -3f)
                {
                    m_Animator.SetTrigger("knockback");
                    StartCoroutine(Wakeup());
                    if (m_Grounded)
                        m_RigidBody.AddForce(force + new Vector2(m_JumpForce / 2f * Mathf.Sign(force.x), m_JumpForce / 2f));
                    else
                    {
                        m_RigidBody.velocity = Vector2.zero;
                        m_RigidBody.AddForce(force + new Vector2(m_JumpForce / 4f * Mathf.Sign(force.x), m_JumpForce / 2f));
                    }
                    m_Animator.SetBool("ground", m_Grounded = false);
                    //knockback
                }
                else if (poise <= 0f)
                {
                    m_Animator.SetTrigger("stagger");
                    m_RigidBody.AddForce(new Vector2(force.x, Mathf.Min(0f, force.y)));
                }
                else
                    m_RigidBody.AddForce(new Vector2(force.x / 2f, Mathf.Min(0f, force.y)));
            }
            else
            {
                m_Health = 0f;
                StartCoroutine(HitEffect(new Color(5f, 5f, 5f, 1f)));
                m_Animator.SetTrigger("knockback");
                if(m_Grounded)
                    m_RigidBody.AddForce(force + new Vector2(m_JumpForce / 2f * Mathf.Sign(force.x), m_JumpForce / 2f));

            }
        }

        public virtual void OnGuardReceived(Vector3 attackerPosition)
        {
            Vector2 direction = Vector3.Normalize(m_Transform.position - attackerPosition);

            m_RigidBody.AddForce(direction * 400f);
        }

        public virtual void OnAttackHit()
        {
            StartCoroutine(Hitstop());
            m_RigidBody.AddForce(new Vector2(200f * (FlipX ? 1f : -1f), 0f));
        }

        public void EVENT_SET_BOUNCE(float bounciness)
        {
            BounceEnabled = bounciness == 0f ? false : true;
        }

        public void EVENT_SET_HEIGHT(float height)
        {
            m_BodyOffset.y = height;
        }

        public virtual void OnAttackGuarded()
        {
            Vector2 direction = Vector3.Normalize(m_Transform.position - m_Opponent.m_Transform.position);

            m_RigidBody.AddForce(direction * 400f);
            StartCoroutine(Hitstop());
        }

        public void EVENT_ENABLE_MOVE()
        {
            m_AnimationMovementModifier = 1f;
        }

        public void EVENT_DISABLE_MOVE()
        {
            m_AnimationMovementModifier = 0f;
        }

        public void EVENT_ENABLE_DIRECTION()
        {
            m_AnimationAllowDirectionChange = true;
        }

        public void EVENT_DISABLE_DIRECTION()
        {
            m_AnimationAllowDirectionChange = false;
        }

        public void EVENT_ENABLE_BOTH()
        {
            m_AnimationMovementModifier = 1f;
            m_AnimationAllowDirectionChange = true;
        }

        public void EVENT_SET_SPEED(float speed)
        {
            m_AnimationMovementModifier = speed;
        }

        public void EVENT_JUMP()
        {
            m_PhysicsActions.Enqueue(() =>
            {
                m_RigidBody.AddForce(new Vector2((m_JumpForce / 3f * Mathf.Clamp((JoystickInput.x * 100f), -1f, 1f)), m_JumpForce));
                lastGroundedFrame = 10;
            });
        }

        //Perform the throw
        public void EVENT_PROJECTILE_1(AnimationEvent e)
        {
            Vector2 origin, direction;
            m_HitboxManager.DecodeOriginAndDirection(e.intParameter, e.floatParameter, out origin, out direction);

            origin.x *= FlipX ? -1f : 1f;
            m_Opponent.m_Collider.enabled = true;
            m_Opponent.m_RigidBody.bodyType = RigidbodyType2D.Dynamic;
            m_Opponent.m_Transform.position = m_Transform.position + (Vector3)origin;
#if UNITY_2018_2_OR_NEWER
            m_Opponent.m_RigidBody.AddForce(new Vector2(m_JumpForce * 0.75f * (FlipX ? -1f : 1f), m_JumpForce) * direction);
#else
            m_Opponent.m_RigidBody.AddForce(Vector2.Scale(new Vector2(m_JumpForce * 0.75f * (FlipX ? -1f : 1f), m_JumpForce), direction));
#endif
            m_Opponent.m_Animator.SetTrigger("knockback");
            m_Opponent.StartCoroutine("Wakeup");
        }

        //Grab positioning
        public void EVENT_PROJECTILE_2(AnimationEvent e)
        {
            Vector2 origin, direction;
            m_HitboxManager.DecodeOriginAndDirection(e.intParameter, e.floatParameter, out origin, out direction);

            origin.x *= FlipX ? -1f : 1f;
            m_Opponent.m_RigidBody.bodyType = RigidbodyType2D.Kinematic;
            m_Opponent.m_Transform.position = m_Transform.position + (Vector3)origin;
            m_Opponent.m_Collider.enabled = false;
        }

        //I don't think flipping the sprite deserves it's own event when I can just use a generic slot.
        public void EVENT_PROJECTILE_3()
        {
            if (IsBackHeld)
                FlipX = !FlipX;
            LockFlip = true;
        }

        //Going to use projectile slot 4 for forcing myself around.
        public void EVENT_PROJECTILE_4(AnimationEvent e)
        {
            Vector2 origin, direction;

            m_HitboxManager.DecodeOriginAndDirection(e.intParameter, e.floatParameter, out origin, out direction, false);
            origin.x *= FlipX ? -1f : 1f;

            m_RigidBody.AddForce(new Vector2(direction.x * (FlipX ? -1f : 1f), direction.y) * 50f);
        }
        #endregion
    }
}