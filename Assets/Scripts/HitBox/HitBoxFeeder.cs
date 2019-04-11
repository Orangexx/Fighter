using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class HitBoxFeeder : MonoBehaviour
{
    public ICharacter Owner { get; private set; }
    public BoxCollider2D Collider { get; private set; }

    private float m_Damage = 1f;
    private float m_Strength = 1f;
    //private int m_FXUID = 0;
    private Vector2 m_Force;
    private bool m_DidHit = false;

    //public int Id { get; private set; }
    public HitboxType Type { get; private set; }

    void Awake()
    {
        Collider = GetComponent<BoxCollider2D>();
        Owner = GetComponentInParent<ICharacter>();
        gameObject.tag = transform.parent.tag;

        Collider.enabled = false;
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        Collider = GetComponent<BoxCollider2D>();
        Owner = GetComponentInParent<CharacterMovement>();
    }
#endif
    public void Feed(Vector2 boxSize, Vector2 boxOffset, HitboxType type,
        float damage, float strength, Vector2 forceRange, bool isTrigger)
    {
        Type = type;
        m_Damage = damage;
        m_Strength = strength;
        m_Force = forceRange;
        Collider.size = boxSize;
        Collider.offset = boxOffset;
        Collider.isTrigger = isTrigger;
        m_DidHit = false;

        Collider.enabled = true;
    }

    public void UpdatePoiseDamage(float strength) { m_Strength = strength; }
    public void UpdateAttackDamage(float damage) { m_Damage = damage; }

    public void Disable()
    {
        if (Collider != null)
            Collider.enabled = false;
    }

    //private bool ReportHit(int target)
    //{
    //    return m_Manager.ReportHit(Id, target);
    //}

    //private bool PeekHit(int target)
    //{
    //    return m_Manager.PeekReport(Id, target);
    //}

    private HitBoxFeeder GetFeederFromCollision(Collider2D collision)
    {
        var feeder = collision.GetComponent<HitBoxFeeder>();

        if (feeder == null) return null;
        //if this hitbox already hit someone this frame they need to wait a frame.
        if (feeder.m_DidHit == true) return null;

        ////Check if pair passes matrix
        //var test = HitboxCollisionMatrix.TestPair(Type, feeder.Type);
        ////Since both objects will perform a collision test, only invoke an event if we are receiving.
        //if (test != HitboxCollisionMatrix.EVENT.RECV && test != HitboxCollisionMatrix.EVENT.BOTH) return null;

        return feeder;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var feeder = GetFeederFromCollision(collision);

        Debug.Log("[HitBoxFeeder]: TriggerStay2D");
        if (feeder != null)
            HitBoxManager.Instance.AddContact(this, feeder);
    }

    /// <summary>
    /// Solve a contact between this feeder and param feeder then fire any applicable events.
    /// </summary>
    public void HandleContact(HitBoxFeeder feeder)
    {
        ////Lets ask the manager if we should report this.
        //if (feeder.ReportHit(HitBoxManager.Instance.UID) == false)
        //    //Hit wasn't reported, this animation must have already hit us in a previous frame.
        //    return;
        //Consume the other hurtboxes hit for this frame.
        feeder.m_DidHit = true;
        //Proceed to generate contact data and pass event to the owner.
        var collision = feeder.Collider;
        //var force = Vector2.Lerp(feeder.m_ForceDirection.x, feeder.m_ForceDirection.y,
        //                            Random.Range(0f, 1f)) * Mathf.Lerp(feeder.m_Force.x, feeder.m_Force.y,
        //                            Random.Range(0f, 1f));
        //Flip force direction if the attack is also flipped.

        if (feeder.Owner.FlipX)
            m_Force.x *= -1f;

        //Estimate approximately where the intersection took place.
        var contactPoint = Collider.bounds.ClosestPoint(collision.bounds.center);
        var startY = Mathf.Min(collision.bounds.center.y + collision.bounds.extents.y, Collider.bounds.center.y + (Collider.bounds.extents.y / 2f));
        var endY = Mathf.Max(collision.bounds.center.y - collision.bounds.extents.y, Collider.bounds.center.y - (Collider.bounds.extents.y / 2f));

        contactPoint.y = Mathf.Lerp(startY, endY, Random.Range(0f, 1f));

        //Calculate force, velocity, direction, and damage.
        Owner.HitboxContact(
            new ContactData
            {
                MyHitbox = this,
                TheirHitbox = feeder,
                Damage = feeder.m_Damage,
                PoiseDamage = feeder.m_Strength,
                Force = new Vector2(0.01f,0),
                Point = contactPoint
                //fxID = feeder.m_FXUID
            });
    }
}