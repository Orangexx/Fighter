using QFramework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ACTFSM : MonoBehaviour
{
    private delegate void InState();
    protected delegate bool FSMTrigger();

    [Header("Setting")]
    [SerializeField] protected string mHitboxDataPath;
    [SerializeField] protected string mStateMapPath;
    [SerializeField] protected string mStateMapTableName;
    [SerializeField] protected Dictionary<string, FSMTrigger> mTriggerFuns = new Dictionary<string, FSMTrigger>();

    [Space(5)]
    [Header("GameObject")]
    [SerializeField] protected Animator mAnimator;
    [SerializeField] protected Rigidbody2D mRigbody;
    [SerializeField] protected SpriteRenderer mSpriteRenderer;
    [SerializeField] protected LayerMask mMapLayer;
    [SerializeField] protected Model mModel;

    [Space(5)]
    [Header("跳跃速度")]
    [SerializeField] private float mJumpSpeed;

    [Space(5)]
    [Header("Other Settings")]
    [SerializeField] protected float mPixelPerUnit;
    [SerializeField] protected int mPhyBoxCount;

    protected List<StateMap> mStates = new List<StateMap>();
    protected XFSMLite mXFSMLite = new XFSMLite();
    protected Dictionary<string, XHitboxAnimation> mHitboxData = new Dictionary<string, XHitboxAnimation>();

    #region FsmUse
    private int mLastFrame = -1;
    private float mTriggerTime = 0;
    private float mHitboxTime = 0;
    #endregion

    #region Hitbox
    private HitboxColliderData[] mCurHitBoxData;
    private float mScale;
    private BoxCollider2D[] mColliders;
    private HitBoxFeeder[] mFeeders;
    private const int MAX_HITBOXES = 10;
    #endregion

    public bool FlipX { protected set; get; }

    #region Init
    private void Awake()
    {
        mModel = GetComponent<Model>();

        //设置路径以及转移方法
        _InitPath();
        _InitOther();

        //载入配置数据
        _InitFSMMapData();
        _InitHitboxData();

        //设置 HitboxCollider
        _InitHitboxCollider();
    }
    protected virtual void _InitPath()
    {

    }
    protected virtual void _InitTriggerDic()
    {

    }
    protected virtual void _InitOther()
    {

    }

    private void _InitFSMMapData()
    {
        var sqlite = new Sqlite(mStateMapPath);

        if (string.IsNullOrEmpty(mStateMapTableName))
            mStates = sqlite.SelectTable<StateMap>();
        else
            mStates = sqlite.SelectTable<StateMap>(mStateMapTableName);

        sqlite.Close();
    }
    private void _InitHitboxData()
    {
        mStates.ForEach(state =>
        {
            mHitboxData.Add(state.StateName,
                AssetDatabase.LoadAssetAtPath<XHitboxAnimation>(
                    String.Format(mHitboxDataPath, state.StateName)));
        });
    }

    private void _InitHitboxCollider()
    {
        var scale = transform.localScale;

        mScale = scale.x;
        mColliders = GetComponentsInChildren<BoxCollider2D>();
        mFeeders = GetComponentsInChildren<HitBoxFeeder>();
        if (mColliders == null || mColliders.Length < MAX_HITBOXES)
        {
            mColliders = new BoxCollider2D[MAX_HITBOXES];
            mFeeders = new HitBoxFeeder[MAX_HITBOXES];
            for (int i = 0; i < mColliders.Length; i++)
            {
                var newGameObject = new GameObject("collider (" + i + ")");
                newGameObject.transform.SetParent(transform, false);
                var collider = newGameObject.AddComponent<BoxCollider2D>();
                var feeder = newGameObject.AddComponent<HitBoxFeeder>();
                collider.gameObject.layer = i < mPhyBoxCount ? gameObject.layer : gameObject.layer + 5;
                collider.enabled = false;
                mColliders[i] = collider;
                mFeeders[i] = feeder;
            }
        }
    }
    #endregion

    private void Start()
    {
        _InitTriggerDic();
        _InitFSM();
        if (mStates.Count > 0)
            mXFSMLite.Start(mStates[0].StateName);
        _InitOtherFinally();
    }

    protected virtual void _InitOtherFinally()
    {

    }

    private void _InitFSM()
    {
        //添加 Instate 方法
        mStates.ForEach(state =>
        {
            List<StateTrigger> triggers = state.GetStateTriggers();
            XHitboxAnimation hitboxData;
            mHitboxData.TryGetValue(state.StateName, out hitboxData);
            Debug.LogFormat("[CharacterFSM]: {0}", state.StateName);
            if (!mXFSMLite.HasState(state.StateName))
            {
                mXFSMLite.AddState(
                    state.StateName,
                    new XFSMLite.InStateFunc((target) =>
                    {
                        if (hitboxData != null) _GetHitboxFunc(hitboxData)();
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
                    mXFSMLite.AddTranslation(state.StateName, triggers[j].NextStateName, triggers[j].NextStateName, new XFSMLite.ToNextStateFunc((target) =>
                    {
                        mLastFrame = -1;
                        mTriggerTime = 0f;
                        mHitboxTime = 0f;
                        mRigbody.velocity = new Vector2(0, mJumpSpeed);
                        mAnimator.Play(triggers[j].NextStateName);

                        Debug.LogFormat("{0} ——> {1}", state.StateName, triggers[j].NextStateName);
                    }));
                    continue;
                }
                mXFSMLite.AddTranslation(state.StateName, triggers[j].NextStateName, triggers[j].NextStateName, new XFSMLite.ToNextStateFunc((target) =>
                {
                    mLastFrame = -1;
                    mTriggerTime = 0f;
                    mHitboxTime = 0f;
                    mRigbody.velocity = new Vector2(0, mRigbody.velocity.y);
                    mAnimator.Play(triggers[j].NextStateName);
                    Debug.LogFormat("{0} ——> {1}", state.StateName, triggers[j].NextStateName);
                }));
            }
        });
    }

    private XFSMLite.InStateFunc _GetTriggerFunc(List<StateTrigger> triggers)
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

                        if (mTriggerTime >= mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length)
                            mXFSMLite.HandleEvent(triggers[j].NextStateName);
                        return;
                    }

                    if (!mTriggerFuns.ContainsKey(triggers[j].TriggerKey))
                        return;

                    if (mTriggerFuns[triggers[j].TriggerKey]())
                    {
                        mXFSMLite.HandleEvent(triggers[j].NextStateName);
                    }
                });
            }

            if (triggers[j].TriggerTime == "-1")
            {
                inState = new InState(() =>
                {
                    if (mTriggerTime >= 0.3f || mTriggerTime <= 0.1f)
                        return;
                    if (!mTriggerFuns.ContainsKey(triggers[j].TriggerKey))
                        return;
                    if (mTriggerFuns[triggers[j].TriggerKey]())
                    {
                        mXFSMLite.HandleEvent(triggers[j].NextStateName);
                    }
                });
            }

            if (triggers[j].TriggerTime == "1")
            {
                inState = new InState(() =>
                {
                    if (mTriggerTime < (mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.3f) || mTriggerTime >= mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length)
                        return;
                    if (!mTriggerFuns.ContainsKey(triggers[j].TriggerKey))
                        return;
                    if (mTriggerFuns[triggers[j].TriggerKey]())
                    {
                        mXFSMLite.HandleEvent(triggers[j].NextStateName);
                    }
                });
            }

            if (inState != null)
                inStates.Add(inState);
        }
        toNextStateFunc = new XFSMLite.InStateFunc((target) =>
        {
            mTriggerTime += Time.deltaTime;
            for (int i = 0; i < inStates.Count; i++)
            {
                inStates[i]();
            }
        });

        return toNextStateFunc;
    }

    private XFSMLite.InStateFunc _GetHitboxFunc(XHitboxAnimation xHitboxData)
    {
        List<InState> setHitDatas = new List<InState>();
        XFSMLite.InStateFunc setFrameFunc = null;
        float[] times = new float[xHitboxData.framedata.Length];

        for (int i = 0; i < times.Length; i++)
            times[i] = xHitboxData.framedata[i].time;

        for (int i = 0; i < xHitboxData.framedata.Length; i++)
        {
            int m = i;
            InState inFrame = null;

            inFrame = new InState(() =>
            {
                _SetColliders(xHitboxData, m);
            });
            setHitDatas.Add(inFrame);
        }

        setFrameFunc = new XFSMLite.InStateFunc((target) =>
        {
            mHitboxTime = (mHitboxTime + Time.deltaTime) % xHitboxData.clip.length;
            int frame = _GetNowFrame(mHitboxTime, times);
            if (frame == mLastFrame)
                return;
            else
                setHitDatas[frame]();

            mLastFrame = frame;
        });

        return setFrameFunc;
    }

    private void _SetColliders(XHitboxAnimation data, int index)
    {
        for (int i = 0; i < mFeeders.Length; i++)
        {
            mFeeders[i].Disable();
        }

        var colliders = data.framedata[index].collider;

        mCurHitBoxData = colliders;

        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];
            var rect = collider.rect;
            Vector2 size = new Vector2(rect.size.x / mPixelPerUnit, rect.size.y / mPixelPerUnit);
            Vector2 offset = new Vector2((rect.position.x + rect.size.x / 2) / mPixelPerUnit, (rect.position.y + rect.size.y / 2) / mPixelPerUnit);


            var remainTime = mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length - mTriggerTime;
            if (collider.type == HitboxType.TRIGGER)
            {
                if (i > mPhyBoxCount)
                    return;

                mFeeders[i].Feed(size, offset, collider.type, data.damage, data.strength, data.force, false, remainTime, mXFSMLite.CurState);
                mFeeders[i + mPhyBoxCount].Feed(size, offset, collider.type, data.damage, data.strength, data.force, true, remainTime, mXFSMLite.CurState);
            }
            else if (collider.type != HitboxType.TRIGGER)
            {
                int j = i > mPhyBoxCount * 2 - 1 ? i : i += mPhyBoxCount + 1;
                mFeeders[j].Feed(size, offset, collider.type, data.damage, data.strength, data.force, true, remainTime, mXFSMLite.CurState);
            }
        }
    }
    private int _GetNowFrame(float time, float[] times)
    {
        for (int i = 0; i < times.Length - 1; i++)
        {
            int j = i;
            if (time >= times[j] && time < times[j + 1])
                return j;
        }
        if (time >= times[times.Length - 1])
            return times.Length - 1;
        return -1;
    }

    #region TirggerFun
    protected bool _IsGround()
    {
        float mDistToGround = mSpriteRenderer.sprite.bounds.size.y / 2 - 0.1f;
        //todo 待增加左右两边的检测，不光中心检测
        return Physics2D.Raycast(transform.position, Vector2.down, mDistToGround, mMapLayer);
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (mCurHitBoxData != null)
            for (int i = 0; i < mCurHitBoxData.Length; i++)
            {
                var color = mCurHitBoxData[i].type == HitboxType.HURT ? Color.red : Color.green;
                color.a = 0.75f;
                Gizmos.color = color;
                Rect rect = new Rect(mCurHitBoxData[i].rect.x / mPixelPerUnit, mCurHitBoxData[i].rect.y / mPixelPerUnit, mCurHitBoxData[i].rect.width / mPixelPerUnit, mCurHitBoxData[i].rect.height / mPixelPerUnit);

                if (FlipX)
                {
                    rect.x *= -1;
                    rect.width *= -1;
                }

                Gizmos.DrawCube(new Vector3(transform.position.x + rect.x + rect.width / 2f, transform.position.y + rect.y + rect.height / 2f, transform.position.z), new Vector3(rect.width, rect.height, 1));
            }
    }
#endif
}