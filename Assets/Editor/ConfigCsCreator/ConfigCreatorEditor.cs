using UnityEngine;
using UnityEditor;
using System.IO;

public class ConfigCreatorEditor : EditorWindow
{
    string csConfigFilePath = "Scripts/Configs";
    string sqliteFilePath = "test.db";
    string tableName = "";
    bool firstCreat = false;

    ConfigCreatorEditor()
    {
        this.titleContent = new GUIContent("ConifgCS Creator");
    }

    [MenuItem("Tools/SQLiteCreator")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ConfigCreatorEditor));
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        tableName = EditorGUILayout.TextField("Table Name", tableName);
        GUILayout.Space(10);
        csConfigFilePath = EditorGUILayout.TextField("CS Path", csConfigFilePath);
        GUILayout.Space(10);
        sqliteFilePath = EditorGUILayout.TextField("SQLite File Path", sqliteFilePath);

        if (GUILayout.Button("Creat CS"))
        {
            if (!Directory.Exists(Application.dataPath + "/" + csConfigFilePath))
            {
                Directory.CreateDirectory(Application.dataPath + "/" + csConfigFilePath);
            }
            ConfigCreator.Creat("Assets/" + csConfigFilePath, "Assets/" + sqliteFilePath, tableName,firstCreat);
        }

        if (GUILayout.Button("Creat All Table CS"))
        {
            if (!Directory.Exists(Application.dataPath + "/" + csConfigFilePath))
            {
                Directory.CreateDirectory(Application.dataPath + "/" + csConfigFilePath);
            }
            ConfigCreator.CreatAll("Assets/"+ csConfigFilePath, "Assets/" + sqliteFilePath,firstCreat);
        }

        firstCreat = GUILayout.Toggle(firstCreat, "First Creat");
    }
}