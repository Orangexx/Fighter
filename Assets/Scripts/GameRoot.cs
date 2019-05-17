using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEditor;
using QFramework.Example;
using QFramework.Fighter;

public class GameRoot : MonoSingleton<GameRoot>
{

    ResLoader mResLoader = ResLoader.Allocate();
    private GameDevSetting mGameDevSetting;


    private GameObject mCharactor;
    private MainCamera mMainCamera;
    private GameObject mHitFX;

    private void Awake()
    {
        ResMgr.Init();
        ConfigManager.Instance.Init();
        mGameDevSetting = AssetDatabase.LoadAssetAtPath<GameDevSetting>("Assets/DevSetting.asset");
        mCharactor = (GameObject)Instantiate(mResLoader.LoadSync(mGameDevSetting.CharactorPath));
        mMainCamera = mResLoader.LoadSync<GameObject>(mGameDevSetting.MainCamePrefabPath).Instantiate().GetComponent<MainCamera>();

        GlobalManager.Instance.Init(mCharactor, mMainCamera, mGameDevSetting);
        EnemyManager.Instance.Init();
        LevelManager.Instance.Init(GlobalManager.Instance.MapLevel);
        BgManager.Instance.Init();
        HitBoxManager.Instance.HitFx = Instantiate(mResLoader.LoadSync<GameObject>("Resources/Prefabs/HitFXPrefab"));
        UIMgr.OpenPanel<MainPanel>();
    }
}
