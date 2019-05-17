using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameDevSetting")]
public class GameDevSetting : ScriptableObject
{
    public string BgMidPrefabPath = "resources://Prefabs/Bg/BgMid";
    public string BgFarCamPrefabPath = "resources://Prefabs/Camera/CameraBgFar";


    //�в㱳��·��
    public string BgMidSpritePath = "resources://Sprites/BgMid/BgMid";
    //����Զ��·��
    public string BgFarSpritePath = "resources://Sprites/BgFar/BgFar";

    //���Ԥ����·��
    public string CharactorPath = "resources://Prefabs/Fighter";
    //����Ԥ����·��
    public Dictionary<string,string> dic_monster_path = new Dictionary<string, string>
    {
        {"Thief","resources://Prefabs/Thief"}
    };
    public string MainCamePrefabPath = "Resources/Prefabs/Camera/MainCamera";

    public float BgMidLength = 10.74f;
}

