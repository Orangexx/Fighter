using System.Collections;
using UnityEngine;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
#pragma warning disable 0649
    public partial class PlayerController : Character
    {
        private enum PlayerInputModifier { P1, P2 }
        private static readonly string[] StrInputModifier = {"1", "2" };
        private WaitForSeconds m_SpecialBufferClear;
        [SerializeField]
        InputHistory m_InputManager;
        [SerializeField]
        PlayerInputModifier m_InputModifier;

        public bool SuppressInputs { get; set; }

        #region MonoBehaviour Methods

        protected override void Start()
        {
            base.Start();
            m_SpecialBufferClear = new WaitForSeconds(0.5f);
            m_InputManager.Event.AddListener(InputListener);
            m_InputManager.OnChange.AddListener(InputChanged);
            m_InputManager.InputMod = StrInputModifier[(int)m_InputModifier];
        }

        protected override void FixedUpdate()
        {
            if (!SuppressInputs)
            {
                JoystickInput = new Vector2(
                    Input.GetAxis("Horizontal" + StrInputModifier[(int)m_InputModifier]),
                    Input.GetAxis("Vertical" + StrInputModifier[(int)m_InputModifier]));

                if (Mathf.Abs(JoystickInput.x) < 0.15f)
                    JoystickInput = new Vector2(0f, JoystickInput.y);

                InputState = m_InputManager.CurrentState;
            }
            else
                JoystickInput = Vector2.zero;

            base.FixedUpdate();
        }

        #endregion

        #region InputHandling
        public void InputListener(INPUTFLAG thisInput)
        {
            if (SuppressInputs) return;

            if (checkflag(thisInput, INPUTFLAG.UP) && m_Grounded)
            {
                m_Animator.SetTrigger("jump");
            }
            else if (checkflag(thisInput, INPUTFLAG.LIGHT))
            {
                if (checkflag(m_InputManager.CurrentState, INPUTFLAG.HEAVY) ||
                    checkflag(thisInput, INPUTFLAG.HEAVY))
                    m_Animator.SetTrigger("grab");
                else 
                {
                    INPUTFLAG forward = FlipX ? INPUTFLAG.LEFT : INPUTFLAG.RIGHT;

                    if (m_Grounded && 
                        m_InputManager.ExistsInHistory(INPUTFLAG.DOWN, INPUTFLAG.DOWN | forward, forward, INPUTFLAG.LIGHT))
                    {
                        m_Animator.SetTrigger("uppercut");
                        StartCoroutine(ClearUppercutBuffer());
                    }
                    else
                        m_Animator.SetTrigger("punch");
                }
            }
            else if (checkflag(thisInput, INPUTFLAG.HEAVY))
            {
                if (checkflag(m_InputManager.CurrentState, INPUTFLAG.LIGHT) ||
                    checkflag(thisInput, INPUTFLAG.LIGHT))
                    m_Animator.SetTrigger("grab");
                else
                    m_Animator.SetTrigger("kick");
            }
        }

        protected void InputChanged(INPUTFLAG currentState)
        {

        }

        private IEnumerator ClearUppercutBuffer()
        {
            yield return m_SpecialBufferClear;
            m_Animator.ResetTrigger("uppercut");
        }
        #endregion
    }
}