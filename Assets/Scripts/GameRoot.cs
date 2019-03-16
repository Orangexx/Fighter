using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEditor;

public class GameRoot : MonoBehaviour {

    ResLoader mResLoader = ResLoader.Allocate();
    private GameDevSetting mGameDevSetting;


    private GameObject mCharactor;

    private void Awake()
    {
        ResMgr.Init();
        mGameDevSetting = AssetDatabase.LoadAssetAtPath<GameDevSetting>("Assets/DevSetting.asset");
        mCharactor = (GameObject)Instantiate(mResLoader.LoadSync(mGameDevSetting.CharactorPath));
        mResLoader.LoadSync<GameObject>(mGameDevSetting.MainCamePrefabPath).Instantiate();
        GlobalManager.Instance.Init(mCharactor,mGameDevSetting);
    }

    private void Start()
    {
        
    }
}
