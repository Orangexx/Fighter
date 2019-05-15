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
