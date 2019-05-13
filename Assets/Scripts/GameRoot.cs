using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEditor;

public class GameRoot : MonoBehaviour
{

    ResLoader mResLoader = ResLoader.Allocate();
    private GameDevSetting mGameDevSetting;


    private GameObject mCharactor;
    private MainCamera mMainCamera;

    private void Awake()
    {
        ResMgr.Init();
        mGameDevSetting = AssetDatabase.LoadAssetAtPath<GameDevSetting>("Assets/DevSetting.asset");
        mCharactor = (GameObject)Instantiate(mResLoader.LoadSync(mGameDevSetting.CharactorPath));
        Instantiate(mResLoader.LoadSync(mGameDevSetting.ThiefPath));
        mMainCamera = mResLoader.LoadSync<GameObject>(mGameDevSetting.MainCamePrefabPath).Instantiate().GetComponent<MainCamera>();
        GlobalManager.Instance.Init(mCharactor, mMainCamera, mGameDevSetting);
    }

    private void Start()
    {

    }
}
