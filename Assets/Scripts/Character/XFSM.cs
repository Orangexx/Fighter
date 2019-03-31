using UnityEngine;
using System.Collections;
using QFramework;
using UniRx;


using System.Collections.Generic;
/// <summary>
/// QFSM lite.
/// </summary>
public class XFSMLite
{
    private CompositeDisposable InStateDisposables = new CompositeDisposable();
    private CompositeDisposable ToNextStateDisposables = new CompositeDisposable();

    public delegate void ToNextStateFunc(params object[] param);
    public delegate void InStateFunc(params object[] param);


    /// <summary>
    /// QFSM state.
    /// </summary>
    class QFSMState
    {
        public string Name;

        public QFSMState(string name)
        {
            Name = name;
        }
        public QFSMState(string name, InStateFunc inState)
        {
            Name = name;
            InState = inState;
        }

        //当前状态的监听方法
        public InStateFunc InState;
                                   
        /// <summary>
        /// 图中的指出的有向线段
        /// </summary>
        public readonly Dictionary<string, QFSMTranslation> TranslationDict = new Dictionary<string, QFSMTranslation>();
    }

    /// <summary>
    /// Translation 
    /// </summary>
    public class QFSMTranslation
    {
        public string FromState;
        public string Name;
        public string ToState;
        public ToNextStateFunc OnTranslationCallback; // 回调函数

        public QFSMTranslation(string fromState, string name, string toState, ToNextStateFunc onTranslationCallback)
        {
            FromState = fromState;
            ToState = toState;
            Name = name;
            OnTranslationCallback = onTranslationCallback;
        }
    }

    /// <summary>
    /// The state of the m current.
    /// </summary>
    string mCurState;

    public string State
    {
        get { return mCurState; }
    }

    /// <summary>
    /// The m state dict.
    /// </summary>
    Dictionary<string, QFSMState> mStateDict = new Dictionary<string, QFSMState>();

    public void AddState(string name)
    {
        mStateDict[name] = new QFSMState(name);
    }

    public void AddState(string name, InStateFunc inState)
    {
        mStateDict[name] = new QFSMState(name, inState);
    }

    public bool HasState(string name)
    {
        return mStateDict.ContainsKey(name);
    }

    /// <summary>
    /// Adds the translation.
    /// </summary>
    /// <param name="fromState">From state.</param>
    /// <param name="name">Name.</param>
    /// <param name="toState">To state.</param>
    /// <param name="callfunc">Callfunc.</param>
    public void AddTranslation(string fromState, string name, string toState, ToNextStateFunc callfunc)
    {
        mStateDict[fromState].TranslationDict[name] = new QFSMTranslation(fromState, name, toState, callfunc);
    }

    /// <summary>
    /// Start the specified name.
    /// </summary>
    /// <param name="name">Name.</param>
    public void Start(string name)
    {
        InStateDisposables.Clear();

        Observable.EveryUpdate()
            .Subscribe(_ =>
            { mStateDict[name].InState(); })
            .AddTo(InStateDisposables);
        mCurState = name;

    }

    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="param">Parameter.</param>
    public void HandleEvent(string name, params object[] param)
    {
        if (mCurState != null && mStateDict[mCurState].TranslationDict.ContainsKey(name))
        {
            ToNextStateDisposables.Clear();
            InStateDisposables.Clear();

            var tempTranslation = mStateDict[mCurState].TranslationDict[name];
            tempTranslation.OnTranslationCallback(param);

            if (mStateDict[tempTranslation.ToState].InState != null)
            {
                Observable.EveryUpdate()
                    .Subscribe(_ =>
                    { mStateDict[tempTranslation.ToState].InState(); })
                    .AddTo(InStateDisposables);
            }


            mCurState = tempTranslation.ToState;
        }
    }

    /// <summary>
    /// Clear this instance.
    /// </summary>
    public void Clear()
    {
        mStateDict.Clear();
    }
}

