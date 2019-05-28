using UniRx;
using System.Collections.Generic;

public class XFSMLite
{
    private CompositeDisposable InStateDisposables = new CompositeDisposable();
    public delegate void ToNextStateFunc(params object[] param);
    public delegate void InStateFunc(params object[] param);
    public delegate void OnStateBegin(params object[] param);

    public class XFSMState
    {
        public string Name;

        public XFSMState(string name)
        {
            Name = name;
        }
        //public XFSMState(string name, InStateFunc inState)
        //{
        //    Name = name;
        //    InState = inState;
        //}

        public XFSMState(string name, InStateFunc inState,OnStateBegin onBegin = null)
        {
            Name = name;
            InState = inState;
            OnStateBegin = onBegin;
        }

        //当前状态的监听方法
        public InStateFunc InState;
        public OnStateBegin OnStateBegin;                           

        /// <summary>
        /// 图中的指出的有向线段
        /// </summary>
        public readonly Dictionary<string, XFSMTranslation> TranslationDict = new Dictionary<string, XFSMTranslation>();
    }

    public class XFSMTranslation
    {
        public string FromState;
        public string Name;
        public string ToState;
        public ToNextStateFunc OnTranslationCallback; // 回调函数

        public XFSMTranslation(string fromState, string name, string toState, ToNextStateFunc onTranslationCallback)
        {
            FromState = fromState;
            ToState = toState;
            Name = name;
            OnTranslationCallback = onTranslationCallback;
        }
    }

    public string State { get; private set; }
    public XFSMState CurState
    {
        get { return mStateDict[State]; }
    }
    Dictionary<string, XFSMState> mStateDict = new Dictionary<string, XFSMState>();

    public void AddState(string name, InStateFunc inState = null,OnStateBegin onStateBegin = null)
    {
        mStateDict[name] = new XFSMState(name, inState,onStateBegin);
    }

    public bool HasState(string name)
    {
        return mStateDict.ContainsKey(name);
    }

    public void AddTranslation(string fromState, string name, string toState, ToNextStateFunc callfunc)
    {
        mStateDict[fromState].TranslationDict[name] = new XFSMTranslation(fromState, name, toState, callfunc);
    }

    public void HandleEvent(string name, params object[] param)
    {
        if (State != null && mStateDict[State].TranslationDict.ContainsKey(name))
        {
            InStateDisposables.Clear();

            var tempTranslation = mStateDict[State].TranslationDict[name];
            tempTranslation.OnTranslationCallback.Invoke(param);
            mStateDict[tempTranslation.ToState].OnStateBegin.Invoke();

            if (mStateDict[tempTranslation.ToState].InState != null)
            {
                Observable.EveryUpdate()
                    .Subscribe(_ =>
                    { mStateDict[tempTranslation.ToState].InState(); })
                    .AddTo(InStateDisposables);
            }


            State = tempTranslation.ToState;
        }
    }

    public void Start(string name)
    {
        InStateDisposables.Clear();

        mStateDict[name].OnStateBegin.Invoke();
        Observable.EveryUpdate()
            .Subscribe(_ =>
            { mStateDict[name].InState(); })
            .AddTo(InStateDisposables);
        State = name;

    }

    public void Clear()
    {
        mStateDict.Clear();
    }

    public void OnDes()
    {
        InStateDisposables.Clear();
    }
}

