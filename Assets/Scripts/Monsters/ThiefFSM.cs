using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class ThiefFSM : ACTFSM, ICharacter
{
    [SerializeField] private Transform mTarget;
    [SerializeField] private Slider mHpSlider;
    [SerializeField] private float mSpeed = 4f;
    [SerializeField] private float mHatredAmount = 0f;
    private List<XFSMLite.XFSMState> mHurtedStates = new List<XFSMLite.XFSMState>();
    private MonsterModel mThiefModel;

    public class Command
    {
        public string Name;
        public float CurCD;
        public float Range;
        public float CD;
    }

    

    [SerializeField] private Dictionary<string, Command> dic_skill_cd = new Dictionary<string, Command>()
    {
        {"Attack1",new Command{Name = "Attack1",CurCD = 0,Range = 1,CD = 4} },
        {"Attack2",new Command{Name = "Attack2",CurCD = 0,Range = 0.5f,CD = 4}},
        {"Move",new Command{ Name = "Move",CurCD = 0,Range = 2,CD = 5} }
    };


    protected override void _InitPath()
    {
        base._InitPath();
        mHitboxDataPath = "Resources/AnimaBoxDatas/Thief/Thief_{0}";
        mStateMapPath = Application.dataPath + "/Resources/SQLites/Fighter.db";
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
            { "击倒",new FSMTrigger(()=>{ return mModel.PoiseValue < -4; })},
            { "受击",new FSMTrigger(() => {return mModel.PoiseValue < 0; })},
            {"恢复",new FSMTrigger(() => { return mModel.PoiseValue >0; }) }
        };
    }

    protected override void _InitOther()
    {
        base._InitOther();
        mThiefModel = (MonsterModel)mModel;
        mModel.Init(ConfigManager.Instance.GetMonsterConfig("Thief"));
    }

    protected override void _InitOtherFinally()
    {
        base._InitOtherFinally();
        mTarget = GlobalManager.Instance.Charactor.transform;
        FlipX = false;
        mHpSlider.maxValue = mModel.Hp;
        mHpSlider.value = mModel.Hp;
    }

    public void LateUpdate()
    {
        if (mXFSMLite.State == "Thief_Walk")
        {
            _InMove();
        }
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
            mRigbody.velocity = new Vector2(mSpeed, 0f);
        }
        else
        {
            if (!FlipX)
            {
                transform.Rotate(Vector2.up, 180);
                FlipX = !FlipX;
            }
            mRigbody.velocity = new Vector2(-mSpeed, 0f);
        }
    }

    private bool _Attack1()
    {
        _Rotate();
        var info = dic_skill_cd["Attack1"];
        if (Mathf.Abs(mTarget.position.x - transform.position.x) < info.Range && info.CurCD == 0)
        {
            mHatredAmount += 0.05f;
            if(mHatredAmount >= 1)
            {
                mHatredAmount = 0;
                _SetCD(info);
                return true;
            }
        }
        return false;
    }

    private bool _Attack2()
    {
        _Rotate();
        var info = dic_skill_cd["Attack2"];
        if (Mathf.Abs(mTarget.position.x - transform.position.x) < info.Range && info.CurCD == 0)
        {
            mHatredAmount += 0.05f;
            if (mHatredAmount >= 1)
            {
                mHatredAmount = 0;
                _SetCD(info);
                return true;
            }
        }
        return false;
    }

    private bool _Move()
    {
        var info = dic_skill_cd["Move"];
        var attack1 = dic_skill_cd["Attack1"];
        var attack2 = dic_skill_cd["Attack2"];
        if (Mathf.Abs(mTarget.position.x - transform.position.x) > info.Range && info.CurCD == 0)
        {
            if(attack1.CurCD == 0 || attack2.CurCD == 0)
            {
                _SetCD(info);
                return true;
            }
        }

        return false;
    }

    private void _Rotate()
    {
        if (mTarget.position.x > transform.position.x)
        {
            if (FlipX)
            {
                transform.Rotate(Vector2.up, 180);
                FlipX = !FlipX;
            }
        }
        else
        {
            if (!FlipX)
            {
                transform.Rotate(Vector2.up, 180);
                FlipX = !FlipX;
            }
        }
    }

    private void _SetCD(Command info)
    {
        info.CurCD = info.CD;
        StartCoroutine(GameUtils.Wait(info.CD, new System.Action(() => { info.CurCD = 0f; })));
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
                mModel.PoiseValue -= contactData.PoiseDamage;
                StartCoroutine(GameUtils.Wait(Time.deltaTime, new System.Action(() =>
                 {
                     if (mXFSMLite.State == "Thief_HurtInSky")
                     {
                         if (contactData.Force.y != 0)
                             mRigbody.velocity = new Vector2(contactData.Force.x, 3);
                         else
                             mRigbody.velocity = new Vector2(contactData.Force.x, 1);
                     }
                     else
                         mRigbody.velocity = contactData.Force;
                 })));


                mModel.Hp -= (int)contactData.Damage;
                mHpSlider.value = mModel.Hp;
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
}
