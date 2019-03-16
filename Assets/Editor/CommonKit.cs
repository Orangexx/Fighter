using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CommonKit
{
    [MenuItem("Assets/获取资源路径")]
    public static void GetSelectPrefabFilePath()
    {
        GUIUtility.systemCopyBuffer =  AssetDatabase.GetAssetPath(Selection.objects[0]);
    }
}
