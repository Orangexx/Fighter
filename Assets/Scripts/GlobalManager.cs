using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEditor;

public class GlobalManager : Singleton<GlobalManager>
{
    public int MapLevel = 3;
    public GameDevSetting GameDevSetting { get; private set; }
    private GameObject mCharactor;

    private GlobalManager() { }

    public void Init(GameObject charactor,GameDevSetting devSetting)
    {
        GameDevSetting = devSetting;
        mCharactor = charactor;

        //Init Other Managers.
        BgManager.Instance.Init();
    }

    public Vector2 GetCharactorPos()
    {
        return mCharactor.transform.position;
    }

}
