using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameDevSetting")]
public class GameDevSetting : ScriptableObject
{
    public string BgMidPrefabPath = "resources://Prefabs/Bg/BgMid";
    public string BgFarCamPrefabPath = "resources://Prefabs/Camera/CameraBgFar";


    public string BgMidSpritePath = "resources://Sprites/BgMid/BgMid";
    public string BgFarSpritePath = "resources://Sprites/BgFar/BgFar";

    public string CharactorPath = "resources://Prefabs/Fighter";
    public string ThiefPath = "resources://Prefabs/Thief";
    public string MainCamePrefabPath = "Resources/Prefabs/Camera/MainCamera";

    public float BgMidLength = 10.74f;
}

