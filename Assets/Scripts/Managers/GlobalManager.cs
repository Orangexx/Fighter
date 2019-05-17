using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using QFramework.Example;

public class GlobalManager : MonoSingleton<GlobalManager>
{
    public int MapLevel;
    public GameDevSetting GameDevSetting { get; private set; }
    public MainCamera MainCamera { get; private set; }
    public GameObject Charactor { private set;  get; }
    public CharacterModel CharacterModel;
    public bool isPaused = false;

    private GlobalManager()
    {
    }
    public void Init(GameObject charactor,MainCamera mainCamera, GameDevSetting devSetting)
    {
        GameDevSetting = devSetting;
        Charactor = charactor;
        CharacterModel = charactor.GetComponent<CharacterModel>();
        MainCamera = mainCamera;
    }

    public Vector2 GetCharactorPos()
    {
        if (Charactor != null)
            return Charactor.transform.position;
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
        Destroy(this.gameObject);
    }

}

