using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QFramework;
using UnityEngine;
using UniRx;

[QFramework.QMonoSingletonPath("[Manager]/LevelManager")]
public class LevelManager: MonoSingleton<LevelManager>
{
    public int CurLevel { private set; get; }
    private int mCurRoom;
    private bool isSeted;
    private List<LevelConfig> mCurConfigs;
    private GameObject mBarrierWallTem;
    private GameObject[] mWalls;
    private ResLoader mResLoader = ResLoader.Allocate();

    public void Init(int level)
    {
        ResMgr.Init();
        mBarrierWallTem = mResLoader.LoadSync<GameObject>(GlobalManager.Instance.GameDevSetting.BarrierWallPrefabPath);
        mWalls = new GameObject[2]
        {
            Instantiate(mBarrierWallTem),
            Instantiate(mBarrierWallTem)
        };
        ClearAllWall();
        CurLevel = level;
        mCurRoom = 0;
        mCurConfigs = ConfigManager.Instance.GetLevelConfigs(level);
        SetRoom();

        Observable.EveryLateUpdate().Subscribe(_ =>
        {
            if (!isSeted)
                if (GlobalManager.Instance.Charactor.transform.position.x >= mCurConfigs[mCurRoom].StartPos + GlobalManager.Instance.MainCamera.HalfWidth)
                    SetRoom();

        }).AddTo(this);
    }

    public void ClearAllWall()
    {
        mWalls[0].SetActive(false);
        mWalls[1].SetActive(false);
    }

    public void SetNextRoom()
    {
        isSeted = false;
        mCurRoom++;
        if (mCurRoom >= mCurConfigs.Count)
        {
            mCurRoom = 0;
            CurLevel++;
            mCurConfigs = ConfigManager.Instance.GetLevelConfigs(CurLevel);
            if (mCurConfigs == null)
            {
                GlobalManager.Instance.GameOver(true);
                Destroy(this.gameObject);
                return;
            }
            BgManager.Instance.SetBackground(false);
            BgManager.Instance.SetBackground(true);
            GlobalManager.Instance.Charactor.transform.position = new Vector2(2, 0);
        }
        GlobalManager.Instance.MainCamera.SetRange(0,mCurConfigs[mCurRoom].EndPos);
    }

    private void SetRoom()
    {
        isSeted = true;
        var cfg = mCurConfigs[mCurRoom];
        mWalls[0].transform.position = new Vector3(cfg.StartPos, 0);
        mWalls[1].transform.position = new Vector3(cfg.EndPos, 0);
        mWalls[0].SetActive(true);
        mWalls[1].SetActive(true);
        string[] monsterStrs = cfg.Monsters.Split('|');

        foreach (var str in monsterStrs)
        {
            string[] monster = str.Split('.');
            if (monster.Length != 2) continue;


            for (int i = 0; i < monster[1].ToInt(); i++)
            {
                EnemyManager.Instance.CreatEnemy(monster[0], new Vector2(Random.Range(cfg.StartPos, cfg.EndPos),0));
            }
        }

        GlobalManager.Instance.MainCamera.SetRange(mCurConfigs[mCurRoom].StartPos, mCurConfigs[mCurRoom].EndPos);
    }

}
