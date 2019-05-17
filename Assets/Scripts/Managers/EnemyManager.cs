using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QFramework;
using UnityEngine;

[QFramework.QMonoSingletonPath("[Manager]/EnemyManager")]
public class EnemyManager : MonoSingleton<EnemyManager>
{
    private ResLoader mEnemyLoader = ResLoader.Allocate();
    //用于保存预制体
    private Dictionary<string, GameObject> dic_name_enemyGO = new Dictionary<string, GameObject>();
    //用于管理所有生成的 enemy
    private Dictionary<string, List<GameObject>> dic_name_enemylst = new Dictionary<string, List<GameObject>>();

    public void Init()
    {
        ResMgr.Init();
        //初始化所有的 enemy 预制体
        foreach (var monster in GlobalManager.Instance.GameDevSetting.dic_monster_path)
        {
            if (!dic_name_enemyGO.ContainsKey(monster.Key))
                dic_name_enemyGO[monster.Key] = mEnemyLoader.LoadSync<GameObject>(monster.Value);
        }
    }

    public void CreatEnemy(string name,Vector2 position)
    {
        if (!dic_name_enemyGO.ContainsKey(name))
            return;
        if (!dic_name_enemylst.ContainsKey(name))
            dic_name_enemylst[name] = new List<GameObject>();

        var enemy = Instantiate(dic_name_enemyGO[name]);
        enemy.transform.position = position;
        dic_name_enemylst[name].Add(enemy);
    }

    public void OnEnemyDead(string name,GameObject enemyGO)
    {
        if (!dic_name_enemylst.ContainsKey(name))
            return;
        if (dic_name_enemylst[name].Count <= 0)
            return;

        if (dic_name_enemylst[name].Contains(enemyGO))
        {
            Destroy(enemyGO);
            dic_name_enemylst[name].Remove(enemyGO);
            if (dic_name_enemylst[name].Count <= 0)
                dic_name_enemylst.Remove(name);
        }


        GlobalManager.Instance.CharacterModel.AddExp(ConfigManager.Instance.GetMonsterConfig(name).Exp);

        if (dic_name_enemylst.Count <= 0)
        {
            LevelManager.Instance.ClearAllWall();
            LevelManager.Instance.SetNextRoom();
        }
    }
}

