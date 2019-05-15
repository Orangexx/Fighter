﻿using System.Collections.Generic;
using UnityEngine;

class ThiefFSM : ACTFSM,ICharacter
{
    [SerializeField] private Transform mTarget;

    public new bool FlipX { get; private set; }

    protected override void _InitPath()
    {
        base._InitPath();
        mHitboxDataPath = "Assets/Resources/AnimaBoxDatas/Thief/Thief_{0}.asset";
        mStateMapPath = Application.dataPath + "/SQLites/Fighter.db";
        mStateMapTableName = "ThiefStateMap";
    }

    protected override void _InitTriggerDic()
    {
        base._InitTriggerDic();
        mTriggerFuns = new Dictionary<string, FSMTrigger>()
        {
            { "!在地上",new FSMTrigger(() =>{ return !_IsGround(); })},
            { "移动",new FSMTrigger(() => { return _Move(); })},
            { "在地上",_IsGround},
            {"攻击1",new FSMTrigger(() => { return _Attack1(); })},
            {"攻击2",new FSMTrigger(() => { return _Attack2(); })},
            { "击倒",new FSMTrigger(()=>{ return false; })},
            { "受击",new FSMTrigger(() => {return false; })},
            {"恢复",new FSMTrigger(() => { return false; }) }
        };
    }

    protected override void _InitOtherFinally()
    {
        base._InitOtherFinally();
        mTarget = GlobalManager.Instance.Charactor.transform;
        FlipX = false;
    }

    public void Update()
    {
        if (mXFSMLite.State == "Thief_Walk")
        {
            _InMove();
        }
    }

    private bool _Move()
    {
        if (Mathf.Abs(mTarget.position.x - transform.position.x) > 2)
            return true;

        return false;
    }

    private void _InMove()
    {
        if (mTarget.position.x > transform.position.x)
        {
            if (FlipX)
            {
                transform.Rotate(Vector2.up, 180);
                FlipX = !FlipX;
            }
            mRigbody.velocity = new Vector2(5f, 0f);
        }
        else
        {
            if (!FlipX)
            {
                transform.Rotate(Vector2.up, 180);
                FlipX = !FlipX;
            }
            mRigbody.velocity = new Vector2(-5f, 0f);
        }
    }

    private bool _Attack1()
    {
        if (Mathf.Abs(mTarget.position.x - transform.position.x) < 0.6)
            return true;
        return false;
    }

    private bool _Attack2()
    {
        if (Mathf.Abs(mTarget.position.x - transform.position.x) < 0.2)
            return true;
        return false;
    }

    public void HitboxContact(ContactData contactData)
    {

    }
}
