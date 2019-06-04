using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace Fighter
{
    public class GlobalManager : MonoSingleton<GlobalManager>
    {
        public GameDevSetting GameDevSetting { get; private set; }
        public MainCamera MainCamera { get; private set; }
        public GameObject Character { private set; get; }
        public CharacterModel CharacterModel;
        public GameObject Maproot;
        public bool isPaused = false;


        private ResLoader mResLoader = ResLoader.Allocate();
        private GameObject mResCharacter;
        private GameObject mResCamera;
        private GameObject mResMapRoot;

        //Only Once
        public void Init()
        {
            GameDevSetting = mResLoader.LoadSync<GameDevSetting>("Resources/DevSetting");
            mResCharacter = mResLoader.LoadSync<GameObject>(GameDevSetting.CharactorPath);
            mResCamera = mResLoader.LoadSync<GameObject>(GameDevSetting.MainCamePrefabPath);
            mResMapRoot = mResLoader.LoadSync<GameObject>(GameDevSetting.MapRootPath);
            ConfigManager.Instance.Init();
            AudioManager.Instance.Init();
            LevelManager.Instance.Init();
            HitBoxManager.Instance.Init();
            EnemyManager.Instance.Init();
            BgManager.Instance.Init();
        }

        public Vector2 GetCharactorPos()
        {
            if (Character != null)
                return Character.transform.position;
            else
                return Vector2.zero;
        }

        public void GameOver(bool isWin)
        {
            Time.timeScale = 0;
            if (isWin)
                UIMgr.OpenPanel<WinPanel>();
            else
                UIMgr.OpenPanel<LosePanel>();
        }

        public void DeleteAllManager()
        {
            Destroy(BgManager.Instance.gameObject);
            Destroy(EnemyManager.Instance.gameObject);
            Destroy(HitBoxManager.Instance.gameObject);
            Destroy(LevelManager.Instance.gameObject);
            Destroy(AudioManager.Instance.gameObject);
            Destroy(this.gameObject);
        }

        public void InitMainScene(int level)
        {
            Character = Instantiate(mResCharacter);
            MainCamera = mResCamera.Instantiate().GetComponent<MainCamera>();
            Maproot = Instantiate(mResMapRoot);
            CharacterModel = Character.GetComponent<CharacterModel>();

            LevelManager.Instance.ShowMainGame(level);
            HitBoxManager.Instance.ShowMainGame();
            BgManager.Instance.ShowMainGame();

            UIMgr.CloseAllPanel();
            UIMgr.OpenPanel<MainPanel>();
            Time.timeScale = 1;
        }

        public void HideMainScene()
        {
            Time.timeScale = 0;
            LevelManager.Instance.LeavelMainGame();
            HitBoxManager.Instance.LeaveMainGame();
            BgManager.Instance.LeaveMainGame();
            EnemyManager.Instance.LeaveMainGame();
            AudioManager.Instance.StopBGM();

            Character.DestroySelf();
            MainCamera.gameObject.DestroySelf();
            Maproot.DestroySelf();
            CharacterModel = null;
            UIMgr.CloseAllPanel();
            UIMgr.OpenPanel<Panel_Start>();
        }
    }

}
