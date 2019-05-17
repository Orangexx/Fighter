using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QFramework;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager>
{
    private List<CharacterConfig> lst_character_cfg = new List<CharacterConfig>();
    private Dictionary<string, MonsterConfig> dic_monster_cfg = new Dictionary<string, MonsterConfig>();
    private Dictionary<int, List<LevelConfig>> dic_level_cfgs = new Dictionary<int, List<LevelConfig>>();

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
        foreach (var cfg in sqlite.SelectTable<LevelConfig>())
        {
            if (!dic_level_cfgs.ContainsKey(cfg.Level))
                dic_level_cfgs[cfg.Level] = new List<LevelConfig>();

            dic_level_cfgs[cfg.Level].Add(cfg);
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

    public List<LevelConfig> GetLevelConfigs(int level)
    {
        if (!dic_level_cfgs.ContainsKey(level))
            return null;

        return dic_level_cfgs[level];
    }

}