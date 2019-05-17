using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class GlobalManager : Singleton<GlobalManager>
{
    public int MapLevel = 3;
    public GameDevSetting GameDevSetting { get; private set; }
    public MainCamera MainCamera { get; private set; }
    public GameObject Charactor { private set;  get; }

    private GlobalManager()
    {
    }
    public void Init(GameObject charactor,MainCamera mainCamera, GameDevSetting devSetting)
    {
        GameDevSetting = devSetting;
        Charactor = charactor;
        MainCamera = mainCamera;
    }

    public Vector2 GetCharactorPos()
    {
        return Charactor.transform.position;
    }

}

