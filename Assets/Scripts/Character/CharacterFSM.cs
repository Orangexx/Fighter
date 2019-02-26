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
    private delegate void MoveFunc();

    [SerializeField] private Animator mAnimator;
    [SerializeField] private Rigidbody2D mRigbody;
    [SerializeField] private SpriteRenderer mSpriteRenderer; 

    private float time = 0.0f;
    private Sqlite mSqlite;
    private List<StateMap> mStates;
    private XFSMLite mQFSMLite;
    private bool mLookLeft;
    private Bool mIsGrounded = new Bool(false);
    private float mHSpeed = 0;
    private float mVSpeed = 0;
    private float mDistToGround = 0.35f;
    const float NORMALSPEED = 20f;
    private Dictionary<string, Bool> mTriggerKeys;

    [SerializeField] private LayerMask mGrounded;

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
            { "isGrounded",mIsGrounded},
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
                mQFSMLite.AddState(state.StateName, new XFSMLite.InStateFunc((target) =>
                {
                    if (_GetMoveFunc(state.GetStateMoves()) != null)
                        _GetMoveFunc(state.GetStateMoves())();
                    _GetTriggerFunc(triggers)();
                }));
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
                    mHSpeed = 0f;
                    mAnimator.Play(triggers[j].NextStateName);
                    for (int k = 0; k < mStates.Count; k++)
                    {
                        if (mStates[k].StateName == triggers[j].NextStateName)
                        {
                            if (mStates[k].GetStateMoves() != null)
                                _SetMoveSpeed(mStates[k].GetStateMoves())();
                            break;
                        }
                    }

                    Debug.LogFormat("{0} ————> {1}", state.StateName, triggers[j].NextStateName);
                }));
            }
        });
    }
   
    XFSMLite.ToNextStateFunc _SetMoveSpeed(List<StateMove> moves)
    {
        XFSMLite.ToNextStateFunc setMoveInfoFunc;

        List<MoveFunc> setMoveInfos = new List<MoveFunc>();

        if (moves == null)
            return null;

        for (int i = 0; i < moves.Count; i++)
        {
            MoveFunc setMoveInfo = null;
            int j = i;

            if (moves[j].IsMoveHorizontal)
            {
                setMoveInfo = new MoveFunc(() =>
                {
                    mHSpeed = moves[j].MoveSpeed == -1 ?
                                mRigbody.velocity.x == 0 ? NORMALSPEED : mRigbody.velocity.x
                                : moves[j].MoveSpeed;
                });
            }

            if (!moves[j].IsMoveHorizontal)
            {
                setMoveInfo = new MoveFunc(() =>
                {
                    mVSpeed = moves[j].MoveSpeed;
                    mRigbody.velocity = new Vector2(mRigbody.velocity.x, mVSpeed);
                });
            }

            setMoveInfos.Add(setMoveInfo);
        }

        setMoveInfoFunc = new XFSMLite.ToNextStateFunc((target) =>
        {
            for (int i = 0; i < setMoveInfos.Count; i++)
            {
                int j = i;
                if (setMoveInfos[j] != null)
                    setMoveInfos[j]();
            }
        });

        return setMoveInfoFunc;
    }

    XFSMLite.InStateFunc _GetMoveFunc(List<StateMove> moveInfos)
    {
        XFSMLite.InStateFunc inStateMove;

        List<MoveFunc> moveFuncs = new List<MoveFunc>();

        if (moveInfos == null)
            return null;

        for (int i = 0; i < moveInfos.Count; i++)
        {
            MoveFunc moveFunc = null;
            int j = i;

            if (moveInfos[j].IsMoveHorizontal)
            {
                moveFunc = new MoveFunc(() =>
                {
                    if (moveInfos[j].MoveType == "0" || moveInfos[j].MoveType == "-1")
                    {
                        if (InputController.Instance.left)
                        {
                            if (!mLookLeft)
                                transform.Rotate(Vector2.up, 180);
                            mLookLeft = true;
                            mRigbody.velocity = new Vector2(-mHSpeed, mRigbody.velocity.y);
                        }
                        else if (InputController.Instance.right)
                        {
                            if (mLookLeft)
                                transform.Rotate(Vector2.up, 180);
                            mLookLeft = false;
                            mRigbody.velocity = new Vector2(mHSpeed, mRigbody.velocity.y);
                        }
                        else
                        {
                            mRigbody.velocity = new Vector2(0,mRigbody.velocity.y);
                            if (_IsGrounded())
                            {
                                mQFSMLite.HandleEvent("Idle");
                            }
                        }

                    }

                    if (moveInfos[j].MoveType == "1" && time >= mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2 && time <= mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2 + Time.deltaTime)
                    {
                        transform.Translate(new Vector2(mHSpeed, 0));
                    }
                });
                moveFuncs.Add(moveFunc);
            }
        }

        inStateMove = new XFSMLite.InStateFunc((target) =>
        {
            for (int i = 0; i < moveFuncs.Count; i++)
            {
                int j = i;
                moveFuncs[j]();
            }
        });

        return inStateMove;
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

                    if (!mTriggerKeys.ContainsKey(triggers[j].TriggerKey))
                        return;

                    if (mTriggerKeys[triggers[j].TriggerKey].BOOL)
                    {
                        _IsGrounded();
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

    bool _IsGrounded()
    {
        mDistToGround = mSpriteRenderer.sprite.bounds.size.y / 2 -0.01f;
        mIsGrounded.BOOL = Physics2D.Raycast(transform.position, Vector2.down, mDistToGround, mGrounded);
        Debug.DrawLine(transform.position, new Vector2(transform.position.x,transform.position.y+mDistToGround), Color.green, 0.5f);
        RaycastHit2D a = Physics2D.Raycast(transform.position, Vector2.down, mDistToGround);
        return Physics2D.Raycast(transform.position, Vector2.down, mDistToGround, (1 << 8)) && mRigbody.velocity.y == 0;
    }
}

