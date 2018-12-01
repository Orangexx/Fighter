using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UniRx;

public class CharacterFSM : MonoBehaviour
{

    enum eInStateTrigger
    {
        inIdle = 1,
        inWalk = 2,
        inRun = 3
    }

    private delegate void InState();

    [SerializeField] private Animator mAnimator;

    private float time = 0.0f;
    private Sqlite mSqlite;
    private List<StateMap> mStates;
    private XFSMLite mQFSMLite;
    private bool mLastL;
    private Dictionary<string, Bool> mTriggerKeys;

    private void Awake()
    {
        if (mAnimator == null)
        {
            Debug.LogError("[CharacterFSM]:动画状态及未挂载");
            return;
        }
        mSqlite = new Sqlite(Application.dataPath + "/SQLites/Fighter.db");
        mStates = mSqlite.SelectTable<StateMap>();
        mSqlite.Close();
        mQFSMLite = new XFSMLite();
    }

    private void Start()
    {
        mTriggerKeys = new Dictionary<string, Bool>()
        {
            {"攻击",InputController.Instance.Battack},
            {"方向",InputController.Instance.isMoving},
            { "跳跃",InputController.Instance.Bjump },
            { "Skill1",InputController.Instance.BSkill1},
            { "Skill2",InputController.Instance.BSkill2},
            { "Skill3",InputController.Instance.BSkill3}
        };

        _InitFSM();

        if (mStates.Count <= 0)
            return;

        mQFSMLite.Start(mStates[0].StateName);
    }

    void _InitFSM()
    {
        mStates.ForEach(state =>
        {
            List<StateTrigger> triggers = state.GetStateTriggers();
            Debug.LogFormat("[character]: {0}", state.StateName);
            if (!mQFSMLite.HasState(state.StateName))
            {
                mQFSMLite.AddState(state.StateName, _GetTriggerFunc(triggers));
            }
        });

        mStates.ForEach(state =>
        {
            List<StateTrigger> triggers = state.GetStateTriggers();
            for (int i = 0; i < triggers.Count; i++)
            {
                int j = i;
                mQFSMLite.AddTranslation(state.StateName, triggers[j].NextStateName, triggers[j].NextStateName, new XFSMLite.ToNextStateFunc((target) =>
                {
                    time = 0f;
                    mAnimator.Play(triggers[j].NextStateName);
                    Debug.LogFormat("{0} ————> {1}", state.StateName, triggers[j].NextStateName);
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
                    }

                    if (!mTriggerKeys.ContainsKey(triggers[j].TriggerKey))
                        return;

                    if (mTriggerKeys[triggers[j].TriggerKey].BOOL)
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
                    if (!mTriggerKeys.ContainsKey(triggers[j].TriggerKey))
                        return;
                    if (mTriggerKeys[triggers[j].TriggerKey].BOOL)
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
                    if (!mTriggerKeys.ContainsKey(triggers[j].TriggerKey))
                        return;
                    if (mTriggerKeys[triggers[j].TriggerKey].BOOL)
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
}

