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



    private GlobalManager()
    {
    }
    public void Init(GameObject charactor,MainCamera mainCamera, GameDevSetting devSetting)
    {
        //��ʼ�����ñ�
       

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

public class ConfigManager:Singleton<ConfigManager>
{
    private List<CharacterConfig> lst_character_cfg = new List<CharacterConfig>();
    private Dictionary<string, MonsterConfig> dic_monster_cfg = new Dictionary<string, MonsterConfig>();

    private ConfigManager()
    {
    }

    public void Init()
    {
        Sqlite sqlite = new Sqlite(Application.dataPath + "/SQLites/Fighter.db");
        lst_character_cfg = sqlite.SelectTable<CharacterConfig>();
        Debug.LogFormat("[CharacterCfg]: {0}", lst_character_cfg[0].Poise);
        foreach (var cfg in sqlite.SelectTable<MonsterConfig>())
        {
            if (!dic_monster_cfg.ContainsKey(cfg.Name))
                dic_monster_cfg[cfg.Name] = cfg;
        }
        sqlite.Close();
    }

    public CharacterConfig GetCharacterConfig(int level)
    {
        foreach (var cfg in lst_character_cfg)
        {
            if (cfg.Level == level)
                return cfg;
        }
        return null;
    }

    public MonsterConfig GetMonsterConfig(string monster)
    {
        if (dic_monster_cfg.ContainsKey(monster))
            return dic_monster_cfg[monster];
        else
            return null;
    }

}