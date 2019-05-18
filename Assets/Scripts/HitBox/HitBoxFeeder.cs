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
    private float mRemainTime;
    private XFSMLite.QFSMState mState;
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
        Owner = GetComponentInParent<ICharacter>();
    }
#endif
    public void Feed(Vector2 boxSize, Vector2 boxOffset, HitboxType type,
        float damage, float strength, Vector2 forceRange, bool isTrigger,float remainTiem,XFSMLite.QFSMState state)
    {
        Type = type;
        m_Damage = damage;
        m_Strength = strength;
        m_Force = forceRange;
        Collider.size = boxSize;
        Collider.offset = boxOffset;
        Collider.isTrigger = isTrigger;
        m_DidHit = false;
        mRemainTime = remainTiem;
        mState = state;
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
        if (feeder.m_DidHit == true) return null;

        return feeder;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var feeder = GetFeederFromCollision(collision);
        if (feeder == null) return;
        if (feeder.Type == HitboxType.HURT && feeder.transform.parent != this.transform.parent)
        {
            Debug.Log("[HitBoxFeeder]: TriggerStay2D");
            if (feeder != null)
                HitBoxManager.Instance.AddContact(this, feeder);
        }
    }

    /// <summary>
    /// Solve a contact between this feeder and param feeder then fire any applicable events.
    /// </summary>
    public void HandleContact(HitBoxFeeder feeder)
    {
        feeder.m_DidHit = true;
        var collision = feeder.Collider;
        //var force = Vector2.Lerp(feeder.m_ForceDirection.x, feeder.m_ForceDirection.y,
        //                            Random.Range(0f, 1f)) * Mathf.Lerp(feeder.m_Force.x, feeder.m_Force.y,
        //                            Random.Range(0f, 1f));
        //Flip force direction if the attack is also flipped.

        if (feeder.Owner.FlipX)
            feeder.m_Force.x *= -1f;

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
                Force = feeder.m_Force,
                Point = contactPoint,
                RemainTime = feeder.mRemainTime,
                State = feeder.mState,
                //fxID = feeder.m_FXUID
            });
    }
}