using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using QFramework;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "CharacterModelTable")]
class CharacterModelTable : ScriptableObject
{
    public List<CharacterConfig> CharacterCfgs = new List<CharacterConfig>();
}

//class CfgManager : MonoSingleton<CfgManager>
//{
//    protected string CfgPath;

//    public 
//}


//class CharacterCfgManager: 
//{
    
//}
