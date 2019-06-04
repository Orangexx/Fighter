using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create DevSetting ")]
public class GameDevSetting : ScriptableObject
{
    public string BgMidPrefabPath = "resources://Prefabs/Bg/BgMid";
    public string BgFarCamPrefabPath = "resources://Prefabs/Camera/CameraBgFar";


    //中层背景路径
    public string BgMidSpritePath = "Resources/Sprites/BgMid/BgMid";
    //滚动远景路径
    public string BgFarSpritePath = "Resources/Sprites/BgFar/BgFar";

    //玩家预制体路径
    public string CharactorPath = "resources://Prefabs/Fighter";
    //敌人预制体路径
    public Dictionary<string,string> dic_monster_path = new Dictionary<string, string>
    {
        {"Thief","resources://Prefabs/Thief"}
    };
    public string MainCamePrefabPath = "Resources/Prefabs/Camera/MainCamera";

    public string BarrierWallPrefabPath = "Resources/Prefabs/Map/Barrier Wall";

    public string AudioPath = "Resources/Audio/";

    public string MapRootPath = "Resources/Prefabs/Map/MapRoot";

    public List<string> AudioNames = new List<string>()
    {
        "Attack1",
        "Attack2",
        "Hit1",
        "Hit2",
        "BGM1",
        "BGM2",
        "BGM3"
    };


    public float BgMidLength = 10.74f;
}

