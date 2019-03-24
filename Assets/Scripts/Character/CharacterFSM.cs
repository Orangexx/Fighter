using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

//角色表现跳转
public class CharacterFSM : MonoBehaviour
{
    private delegate void InState();
    private delegate bool FSMTrigger();

    //设置按键
    private Sqlite mSetSqlite;
    private List<InputSetting> mInpuSetting;


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

    [Space(5)]
    [Header("GameObject")]
    [SerializeField] private Animator mAnimator;
    [SerializeField] private Rigidbody2D mRigbody;
    [SerializeField] private SpriteRenderer mSpriteRenderer;
    [SerializeField] private LayerMask mMapLayer;


    [Space(5)]
    [Header("跳跃速度")]
    [SerializeField] private float mJumpSpeed;

    private float time = 0.0f;
    private Sqlite mSqlite;
    private List<StateMap> mStates;
    private XFSMLite mQFSMLite;
    private Dictionary<string, FSMTrigger> mTriggerFuns;
    private CharacterTriggers mTriggers;

    private void Awake()
    {
        Debug.Log("[CharacterFSM] Awake()");
        mTriggers = this.GetComponent<CharacterTriggers>();
        if (mAnimator == null)
        {
            Debug.LogError("[CharacterFSM]:动画状态及未挂载");
            return;
        }
        mSqlite = new Sqlite(Application.dataPath + "/SQLites/Fighter.db");
        mStates = mSqlite.SelectTable<StateMap>();
        mSqlite.Close();


        _SetInputKey();

        mQFSMLite = new XFSMLite();
    }

    private void Start()
    {
        mTriggerFuns = new Dictionary<string, FSMTrigger>()
        {
            { "!isGrounded",new FSMTrigger(() =>{ return !_IsGround(); })},
            { "!方向",new FSMTrigger(()=>{ return !Input.GetKey(LEFT) && !Input.GetKey(RIGHT); })},
            { "isGrounded",_IsGround},
            {"攻击",new FSMTrigger(() => { return Input.GetKey(ATTACK); })},
            {"方向",new FSMTrigger(() => {return Input.GetKey(LEFT) || Input.GetKey(RIGHT); }) },
            { "跳跃",new FSMTrigger(() => { return Input.GetKey(JUMP); }) },
            { "Skill1",new FSMTrigger(() => { return Input.GetKey(SKILL1); } ) },
            { "Skill2",new FSMTrigger(() => { return Input.GetKey(SKILL2); })},
            { "Skill3",new FSMTrigger(() => { return Input.GetKey(SKILL3); })}
        };

        _InitFSM();

        if (mStates.Count <= 0)
            return;

        mQFSMLite.Start(mStates[0].StateName);
    }

    private void _InitFSM()
    {
        //添加 Instate 方法
        mStates.ForEach(state =>
        {
            List<StateTrigger> triggers = state.GetStateTriggers();
            Debug.LogFormat("[CharacterFSM]: {0}", state.StateName);
            if (!mQFSMLite.HasState(state.StateName))
            {
                mQFSMLite.AddState(
                    state.StateName,
                    new XFSMLite.InStateFunc((target) =>
                    {
                        _GetTriggerFunc(triggers)();
                    }));
            }
        });


        //重置运动状态  单独处理跳跃
        mStates.ForEach(state =>
        {
            List<StateTrigger> triggers = state.GetStateTriggers();
            for (int i = 0; i < triggers.Count; i++)
            {
                int j = i;

                if (triggers[j].NextStateName == "JumpStart")
                {
                    mQFSMLite.AddTranslation(state.StateName, triggers[j].NextStateName, triggers[j].NextStateName, new XFSMLite.ToNextStateFunc((target) =>
                    {
                        time = 0f;
                        mRigbody.velocity = new Vector2(0, mJumpSpeed);
                        mAnimator.Play(triggers[j].NextStateName);
                        Debug.LogFormat("{0} ——> {1}", state.StateName, triggers[j].NextStateName);
                    }));
                    continue;
                }
                mQFSMLite.AddTranslation(state.StateName, triggers[j].NextStateName, triggers[j].NextStateName, new XFSMLite.ToNextStateFunc((target) =>
                {
                    time = 0f;
                    mRigbody.velocity = new Vector2(0, mRigbody.velocity.y);
                    mAnimator.Play(triggers[j].NextStateName);
                    Debug.LogFormat("{0} ——> {1}", state.StateName, triggers[j].NextStateName);
                }));
            }
        });
    }

    XFSMLite.InStateFunc _GetTriggerFunc(List<StateTrigger> triggers)
    {
        XFSMLite.InStateFunc toNextStateFunc;

        List<InState> inStates = new List<InState>();

        for (int i = 0; i < triggers.Count; i++)
        {
            InState inState = null;
            int j = i;

            if (triggers[j].TriggerTime == "0")
            {
                inState = new InState(() =>
                {
                    if (triggers[j].TriggerKey == "null")
                    {

                        if (time >= mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length)
                            mQFSMLite.HandleEvent(triggers[j].NextStateName);
                        return;
                    }

                    if (!mTriggerFuns.ContainsKey(triggers[j].TriggerKey))
                        return;

                    if (mTriggerFuns[triggers[j].TriggerKey]())
                    {
                        mQFSMLite.HandleEvent(triggers[j].NextStateName);
                    }
                });
            }

            if (triggers[j].TriggerTime == "-1")
            {
                inState = new InState(() =>
                {
                    if (time >= 0.3f || time <= 0.1f)
                        return;
                    if (!mTriggerFuns.ContainsKey(triggers[j].TriggerKey))
                        return;
                    if (mTriggerFuns[triggers[j].TriggerKey]())
                    {
                        mQFSMLite.HandleEvent(triggers[j].NextStateName);
                    }
                });
            }

            if (triggers[j].TriggerTime == "1")
            {
                inState = new InState(() =>
                {
                    if (time < (mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.3f) || time >= mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length)
                        return;
                    if (!mTriggerFuns.ContainsKey(triggers[j].TriggerKey))
                        return;
                    if (mTriggerFuns[triggers[j].TriggerKey]())
                    {
                        mQFSMLite.HandleEvent(triggers[j].NextStateName);
                    }
                });
            }

            if (inState != null)
                inStates.Add(inState);
        }
        toNextStateFunc = new XFSMLite.InStateFunc((target) =>
        {
            time += Time.deltaTime;
            for (int i = 0; i < inStates.Count; i++)
            {
                inStates[i]();
            }
        });

        return toNextStateFunc;
    }

    public eCharacterState GetState()
    {
        switch (mQFSMLite.State)
        {
            case "Run":
                return eCharacterState.Run;
            case "Idle":
                return eCharacterState.Idle;
            case "JumpInSky":
                return eCharacterState.InSky;
            default:
                return eCharacterState.Normal;
        }

    }

    private void _SetInputKey()
    {
        mSetSqlite = new Sqlite(Application.dataPath + "/SQLites/InputSetting.db");
        mInpuSetting = mSetSqlite.SelectTable<InputSetting>();
        UP = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[0].Key);
        DOWN = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[1].Key);
        LEFT = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[2].Key);
        RIGHT = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[3].Key);
        JUMP = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[4].Key);
        ATTACK = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[5].Key);
        SKILL1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[6].Key);
        SKILL2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[7].Key);
        SKILL3 = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[8].Key);
        Debug.Log(mInpuSetting[7].Key);
        mSetSqlite.Close();
    }

    private bool _IsGround()
    {
        float mDistToGround = mSpriteRenderer.sprite.bounds.size.y / 2 - 0.1f;
        //待增加左右两边的检测，不光中心检测
        return Physics2D.Raycast(transform.position, Vector2.down, mDistToGround, mMapLayer);
    }
}

public enum eCharacterState
{
    Normal,
    Idle,
    Run,
    InSky
}

