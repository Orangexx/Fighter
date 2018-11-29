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
    private CompositeDisposable EveryUpdateDisposables = new CompositeDisposable();
    /// <summary>
    /// FSM callfunc.
    /// </summary>
    public delegate void FSMCallfunc(params object[] param);

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
        public QFSMState(string name, FSMCallfunc inState)
        {
            Name = name;
            InState = inState;
        }

        public FSMCallfunc InState; // 回调函数
                                    /// <summary>
                                    /// The translation dict.
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
        public FSMCallfunc OnTranslationCallback; // 回调函数

        public QFSMTranslation(string fromState, string name, string toState, FSMCallfunc onTranslationCallback)
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

    /// <summary>
    /// Adds the state.
    /// </summary>
    /// <param name="name">Name.</param>
    public void AddState(string name)
    {
        mStateDict[name] = new QFSMState(name);
    }

    public void AddState(string name, FSMCallfunc inState)
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
    public void AddTranslation(string fromState, string name, string toState, FSMCallfunc callfunc)
    {
        mStateDict[fromState].TranslationDict[name] = new QFSMTranslation(fromState, name, toState, callfunc);
    }

    /// <summary>
    /// Start the specified name.
    /// </summary>
    /// <param name="name">Name.</param>
    public void Start(string name)
    {
        EveryUpdateDisposables.Clear();

        Observable.EveryUpdate()
            .Subscribe(_ =>
            { mStateDict[name].InState(); })
            .AddTo(EveryUpdateDisposables);
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
            var tempTranslation = mStateDict[mCurState].TranslationDict[name];
            tempTranslation.OnTranslationCallback(param);

            if (mStateDict[tempTranslation.ToState].InState != null)
            {
                EveryUpdateDisposables.Clear();

                Observable.EveryUpdate()
                    .Subscribe(_ =>
                    { mStateDict[tempTranslation.ToState].InState(); })
                    .AddTo(EveryUpdateDisposables);
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

