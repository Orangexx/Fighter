using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackGardenStudios.HitboxStudioPro;

public class CharacterMovement : MonoBehaviour, ICharacter
{

    [SerializeField] private Rigidbody2D mRigbody;

    [Space(5)]
    [Header("DataSetting")]
    [SerializeField] private float mRunSpeed;
    [SerializeField] private float mInSkySpeed;
    [SerializeField] private float mJumpSpeed;
    [SerializeField] private float mOtherSpeed;


    private CharacterTriggers mTriggers;
    private CharacterFSM mFSM;

    public SpritePalette ActivePalette { get; private set; }

    public SpritePaletteGroup PaletteGroup { get; private set; }

    public float Poise { get; set; }

    public bool FlipX { get; private set; }

    // Use this for initialization
    void Start()
    {
        FlipX = false;
        mTriggers = this.GetComponent<CharacterTriggers>();
        mFSM = this.GetComponent<CharacterFSM>();
    }

    // Update is called once per frame
    void Update()
    {

        switch (mFSM.GetState())
        {
            case eState.Normal:
                _InOther();
                break;
            case eState.Run:
                _InRun();
                break;
            case eState.InSky:
                _InSky();
                break;
            default:
                break;
        }

        if (InputController.Instance.jump && mTriggers.IsGrounded.BOOL)
            _StartJump();
    }

    void _InRun()
    {
        if (InputController.Instance.left)
        {
            if (!FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = true;
            mRigbody.velocity = new Vector2(-mRunSpeed, mRigbody.velocity.y);
        }
        else if (InputController.Instance.right)
        {
            if (FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = false;
            mRigbody.velocity = new Vector2(mRunSpeed, mRigbody.velocity.y);
        }
    }


    void _StartJump()
    {
        mRigbody.velocity = new Vector2(0, mJumpSpeed);

    }
    void _InSky()
    {
        if (InputController.Instance.left)
        {
            if (!FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = true;
            mRigbody.velocity = new Vector2(-mRunSpeed, mRigbody.velocity.y);
        }
        else if (InputController.Instance.right)
        {
            if (FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = false;
            mRigbody.velocity = new Vector2(mRunSpeed, mRigbody.velocity.y);
        }
    }

    void _InOther()
    {
        if (InputController.Instance.left)
        {
            if (!FlipX)
                return;
            else mRigbody.velocity = new Vector2(-mOtherSpeed, mRigbody.velocity.y);
        }
        else if (InputController.Instance.right)
        {
            if (FlipX)
                return;
            mRigbody.velocity = new Vector2(mOtherSpeed, mRigbody.velocity.y);
        }
    }

    public void HitboxContact(ContactData data)
    {
        Debug.LogFormat("{0},{1}", data.MyHitbox.Type, data.TheirHitbox.Type);
    }
}
