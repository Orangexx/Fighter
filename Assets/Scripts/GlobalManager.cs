using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEditor;

public class GlobalManager : Singleton<GlobalManager>
{

    //������Դ������ͨ���ű��ڲ��������ṩ�ӿ�
    public int MapLevel = 3;
    public GameDevSetting GameDevSetting { get; private set; }
    public MainCamera MainCamera { get; private set; }


    //˽����Դ��ֻ������ṩ����ȥ��
    private GameObject mCharactor;

    private GlobalManager() { }

    public void Init(GameObject charactor,MainCamera mainCamera, GameDevSetting devSetting)
    {
        GameDevSetting = devSetting;
        mCharactor = charactor;
        MainCamera = mainCamera;
        //Init Other Managers.
        BgManager.Instance.Init();
    }

    public Vector2 GetCharactorPos()
    {
        return mCharactor.transform.position;
    }

}
