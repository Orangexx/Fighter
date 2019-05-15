using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEditor;

public class GlobalManager : Singleton<GlobalManager>
{

    //公共资源，或者通过脚本内部的限制提供接口
    public int MapLevel = 3;
    public GameDevSetting GameDevSetting { get; private set; }
    public MainCamera MainCamera { get; private set; }


    //私有资源，只给外界提供方法去用
    public GameObject Charactor { private set;  get; }

    private GlobalManager() { }

    public void Init(GameObject charactor,MainCamera mainCamera, GameDevSetting devSetting)
    {
        GameDevSetting = devSetting;
        Charactor = charactor;
        MainCamera = mainCamera;
        //Init Other Managers.
        BgManager.Instance.Init();
    }

    public Vector2 GetCharactorPos()
    {
        return Charactor.transform.position;
    }

}
