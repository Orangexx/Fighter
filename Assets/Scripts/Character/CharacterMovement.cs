using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour,ICharacter
{

    [SerializeField] private Rigidbody2D mRigbody;

    [Space(5)]
    [Header("DataSetting")]
    [SerializeField] private float mRunSpeed;
    [SerializeField] private float mInSkySpeed;
    [SerializeField] private float mOtherSpeed;

    private CharacterFSM mFSM;

    public bool FlipX { get; private set; }

    // Use this for initialization
    void Start()
    {
        FlipX = false;
        mFSM = this.GetComponent<CharacterFSM>();
    }

    // Update is called once per frame
    void Update()
    {
        eCharacterState state = mFSM.GetState();

        switch (state)
        {
            case eCharacterState.Normal:
                _InOther();
                break;
            case eCharacterState.Run:
                _InRun();
                break;
            case eCharacterState.InSky:
                _InSky();
                break;
            default:
                break;
        }
    }

    void _InRun()
    {
        if (Input.GetKey(mFSM.LEFT))
        {
            if (!FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = true;
            mRigbody.velocity = new Vector2(-mRunSpeed, mRigbody.velocity.y);
        }
        else if (Input.GetKey(mFSM.RIGHT))
        {
            if (FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = false;
            mRigbody.velocity = new Vector2(mRunSpeed, mRigbody.velocity.y);
        }
    }

    void _InSky()
    {
        if (Input.GetKey(mFSM.LEFT))
        {
            if (!FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = true;
            mRigbody.velocity = new Vector2(-mRunSpeed/2, mRigbody.velocity.y);
        }
        else if (Input.GetKey(mFSM.RIGHT))
        {
            if (FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = false;
            mRigbody.velocity = new Vector2(mRunSpeed/2, mRigbody.velocity.y);
        }
    }

    void _InOther()
    {
        if (Input.GetKey(mFSM.LEFT))
        {
            if (!FlipX)
                return;
            else mRigbody.velocity = new Vector2(-mOtherSpeed, mRigbody.velocity.y);
        }
        else if (Input.GetKey(mFSM.RIGHT))
        {
            if (FlipX)
                return;
            mRigbody.velocity = new Vector2(mOtherSpeed, mRigbody.velocity.y);
        }
    }

    public void HitboxContact(ContactData contactData)
    {
        if (contactData.TheirHitbox.transform.parent == contactData.MyHitbox.transform.parent)
            return;
        switch (contactData.TheirHitbox.Type)
        {
            case HitboxType.TRIGGER:
                break;
            case HitboxType.HURT:
                Debug.LogFormat("[HitBox]: HURT IN  Force{0}",contactData.Force);
                mRigbody.AddForce(contactData.Force);
                break;
            case HitboxType.GUARD:
                break;
            case HitboxType.ARMOR:
                break;
            case HitboxType.GRAB:
                break;
            case HitboxType.TECH:
                break;
            default:
                break;
        }
    }
}
