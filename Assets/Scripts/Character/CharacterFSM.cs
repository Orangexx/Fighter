using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFSM : MonoBehaviour {

    [SerializeField] private Animator mAnimator;

    private Sqlite mSqlite;
    private List<StateMap> mStates;
    private XFSMLite mQFSMLite;

    private void Awake()
    {
        if(mAnimator == null)
        {
            Debug.LogError("[CharacterFSM]:¶¯»­×´Ì¬¼°Î´¹ÒÔØ");
            return;
        }
        mSqlite = new Sqlite(Application.dataPath + "/SQLites/Fighter.db");
        mStates = mSqlite.SelectTable<StateMap>();
        mQFSMLite = new XFSMLite();
    }

    private void Start()
    {
        _InitFSM();
    }

    void _InitFSM()
    {
        mStates.ForEach(state =>
        {
            Debug.LogFormat("[character]: {0}", state.StateName);
            if (!mQFSMLite.HasState(state.StateName))
                mQFSMLite.AddState(state.StateName);

            List<string> nextStates = new List<string>();
            for (int i = 0; i < nextStates.Count; i++)
            {
                if (!mQFSMLite.HasState(nextStates[i]))
                    mQFSMLite.AddState(nextStates[i]);

                mQFSMLite.AddTranslation(state.StateName,nextStates[i],nextStates[i],null);
            }
        });
    }


}
