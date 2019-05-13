using System.Collections.Generic;
using UnityEngine;

class ThiefFSM : ACTFSM
{
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
            { "移动",new FSMTrigger(() => { return false; })},
            { "在地上",_IsGround},
            {"攻击1",new FSMTrigger(() => { return true; })},
            {"攻击2",new FSMTrigger(() => { return false; })},
            { "击倒",new FSMTrigger(()=>{ return false; })},
            { "受击",new FSMTrigger(() => {return false; })},
            {"恢复",new FSMTrigger(() => { return false; }) }
        };
    }

    protected override void _InitOther()
    {
        base._InitOther();
    }
}
