using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create DevSetting ")]
public class GameDevSetting : ScriptableObject
{
    public string BgMidPrefabPath = "resources://Prefabs/Bg/BgMid";
    public string BgFarCamPrefabPath = "resources://Prefabs/Camera/CameraBgFar";


    //�в㱳��·��
    public string BgMidSpritePath = "Resources/Sprites/BgMid/BgMid";
    //����Զ��·��
    public string BgFarSpritePath = "Resources/Sprites/BgFar/BgFar";

    //���Ԥ����·��
    public string CharactorPath = "resources://Prefabs/Fighter";
    //����Ԥ����·��
    public Dictionary<string,string> dic_monster_path = new Dictionary<string, string>
    {
        {"Thief","resources://Prefabs/Thief"}
    };
    public string MainCamePrefabPath = "Resources/Prefabs/Camera/MainCamera";

    public string BarrierWallPrefabPath = "Resources/Prefabs/Map/Barrier Wall";

    public string AudioPaht = "Resources/Audio/";

    public List<string> AudioNames = new List<string>()
    {
        "Attack1",
        "Attack2",
        "Hit1",
        "Hit2",
        "BGM1"
    };


    public float BgMidLength = 10.74f;
}

