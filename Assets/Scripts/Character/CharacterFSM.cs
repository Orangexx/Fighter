using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEditor;
using RuntimeEditor;

public class CharacterFSM : ACTFSM, ICharacter
{
    [Header("Keyboard keys")]
    public KeyCode UP;
    public KeyCode DOWN;
    public KeyCode LEFT;
    public KeyCode RIGHT;
    public KeyCode JUMP;
    public KeyCode ATTACK;
    public KeyCode SKILL2;
    public KeyCode SKILL3;
    public KeyCode SKILL1;
    public KeyCode DUNFU;

    [Space(5)]
    [Header("DataSetting")]
    [SerializeField] private float mRunSpeed;
    [SerializeField] private float mInSkySpeed;
    [SerializeField] private float mOtherSpeed;
    private CharacterModel mCharacterModel;
    private List<XFSMLite.QFSMState> mHurtedStates = new List<XFSMLite.QFSMState>();

    #region Init
    protected override void _InitPath()
    {
        base._InitPath();
        mHitboxDataPath = "Assets/Resources/AnimaBoxDatas/Fighter/Fighter_{0}.asset";
        mStateMapPath = Application.dataPath + "/SQLites/Fighter.db";
    }
    protected override void _InitTriggerDic()
    {
        base._InitTriggerDic();
        mTriggerFuns = new Dictionary<string, FSMTrigger>()
        {
            { "!在地上",new FSMTrigger(() =>{ return !_IsGround(); })},
            { "!方向",new FSMTrigger(()=>{ return !Input.GetKey(LEFT) && !Input.GetKey(RIGHT); })},
            { "在地上",_IsGround},
            {"攻击",new FSMTrigger(() => { return Input.GetKey(ATTACK); })},
            {"方向",new FSMTrigger(() => {return Input.GetKey(LEFT) || Input.GetKey(RIGHT); }) },
            { "跳跃",new FSMTrigger(() => { return Input.GetKey(JUMP); }) },
            { "技能1",new FSMTrigger(() => { return Input.GetKey(SKILL1); } ) },
            { "技能2",new FSMTrigger(() => { return Input.GetKey(SKILL2); })},
            { "技能3",new FSMTrigger(() => { return Input.GetKey(SKILL3); })},
            { "蹲伏",new FSMTrigger(() => {return Input.GetKey(DUNFU); }) },
            { "击倒",new FSMTrigger(()=>{ return mModel.PoiseValue<-5; })},
            { "受击",new FSMTrigger(() => {return mModel.PoiseValue<0; })},
            {"恢复",new FSMTrigger(() => { return mModel.PoiseValue>=0; }) }
        };
    }
    protected override void _InitOther()
    {
        base._InitOther();
        _SetInputKey();
        mModel.Init(ConfigManager.Instance.GetCharacterConfig(1));
        mCharacterModel = (CharacterModel)mModel;
    }
    protected override void _InitOtherFinally()
    {
        base._InitOtherFinally();
        FlipX = false;
        mHurtedStates.Clear();
    }
    private void _SetInputKey()
    {
        var setSqlite = new Sqlite(Application.dataPath + "/SQLites/InputSetting.db");
        var inpuSetting = setSqlite.SelectTable<InputSetting>();
        UP = (KeyCode)System.Enum.Parse(typeof(KeyCode), inpuSetting[0].Key);
        DOWN = (KeyCode)System.Enum.Parse(typeof(KeyCode), inpuSetting[1].Key);
        LEFT = (KeyCode)System.Enum.Parse(typeof(KeyCode), inpuSetting[2].Key);
        RIGHT = (KeyCode)System.Enum.Parse(typeof(KeyCode), inpuSetting[3].Key);
        JUMP = (KeyCode)System.Enum.Parse(typeof(KeyCode), inpuSetting[4].Key);
        ATTACK = (KeyCode)System.Enum.Parse(typeof(KeyCode), inpuSetting[5].Key);
        SKILL1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), inpuSetting[6].Key);
        SKILL2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), inpuSetting[7].Key);
        SKILL3 = (KeyCode)System.Enum.Parse(typeof(KeyCode), inpuSetting[8].Key);
        DUNFU = (KeyCode)System.Enum.Parse(typeof(KeyCode), inpuSetting[9].Key);
        setSqlite.Close();
    }
    #endregion

    #region MoveMent
    void Update()
    {
        switch (mXFSMLite.State)
        {
            case "Run":
                _InRun();
                break;
            case "JumpInSky":
                _InSky();
                break;
            default:
                _InOther();
                break;
        }
    }

    void _InRun()
    {
        if (Input.GetKey(LEFT))
        {
            if (!FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = true;
            mRigbody.velocity = new Vector2(-mRunSpeed, mRigbody.velocity.y);
        }
        else if (Input.GetKey(RIGHT))
        {
            if (FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = false;
            mRigbody.velocity = new Vector2(mRunSpeed, mRigbody.velocity.y);
        }
    }

    void _InSky()
    {
        if (Input.GetKey(LEFT))
        {
            if (!FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = true;
            mRigbody.velocity = new Vector2(-mRunSpeed / 2, mRigbody.velocity.y);
        }
        else if (Input.GetKey(RIGHT))
        {
            if (FlipX)
                transform.Rotate(Vector2.up, 180);
            FlipX = false;
            mRigbody.velocity = new Vector2(mRunSpeed / 2, mRigbody.velocity.y);
        }
    }

    void _InOther()
    {
        if (Input.GetKey(LEFT))
        {
            if (!FlipX)
                return;
            else mRigbody.velocity = new Vector2(-mOtherSpeed, mRigbody.velocity.y);
        }
        else if (Input.GetKey(RIGHT))
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
                if (mHurtedStates.Contains(contactData.State))
                    return;
                Debug.LogFormat("[HitBox]: {0}", contactData.State.Name);
                mHurtedStates.Add(contactData.State);
                StartCoroutine(GameUtils.Wait(contactData.RemainTime, () => mHurtedStates.Remove(contactData.State)));
                //todo
                mRigbody.AddForce(contactData.Force);
                mCharacterModel.PoiseValue -= contactData.PoiseDamage;
                HitBoxManager.Instance.PlayHitFX(contactData.Point);
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
    #endregion
}

