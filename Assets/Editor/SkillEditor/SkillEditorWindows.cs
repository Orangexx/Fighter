//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Linq;


//namespace XiaoXiao.SkillEditor
//{
//    public class SkillEditorWindow : EditorWindow
//    {
//        float toolbarWidth = 300f;

//        GameObject target;
//        bool showColliders = true;
//        bool showGizmos = false;
//        bool showAnimation;
//        bool showAttackData;
//        bool showFrameData;
//        bool showEvents;

//        bool showCharacterEditor;

//        static Texture2D backgroundImage;
//        static Texture2D editorBackgroundImage;

//        private static readonly float MINZOOM = 1f;
//        private static readonly float MAXZOOM = 8f;

//        //priority 设置分组
//        [MenuItem("ACT2D/SkillEditor", priority = 1000)]
//        static void Init()
//        {
//            SkillEditorWindow window = (SkillEditorWindow)EditorWindow.GetWindow(typeof(SkillEditorWindow));
//            window.titleContent = new GUIContent("Skill Editor");
//            window.Show();
//        }

//        private void OnGUI()
//        {
//            //监听事件并重绘
//            wantsMouseMove = true;
//            var currentEvent = Event.current;
//            if (currentEvent.type == EventType.MouseDrag ||
//                 currentEvent.type == EventType.MouseDown ||
//                 currentEvent.type == EventType.MouseMove)
//                Repaint();

//            //加载背景
//            if (editorBackgroundImage == null)
//                editorBackgroundImage = Resources.Load<Texture2D>("GrayCheckerBackground");
//            if (backgroundImage == null)
//                backgroundImage = MakeTex(1, 1, new Color(0.1f, 0.1f, 0.1f, 1f));
//            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), backgroundImage, ScaleMode.StretchToFill);

//            SpriteRenderer render = null;

//            //绘制
//            EditorGUILayout.BeginHorizontal();
//            {
//                var color = GUI.backgroundColor;

//                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(toolbarWidth));
//                {
//                    GUILayout.Label("Select Character", EditorStyles.boldLabel);
//                    var previoustarget = target;
//                    target = (GameObject)EditorGUILayout.ObjectField(target, typeof(GameObject), false);
//                    if (previoustarget != target)
//                    {
//                        Repaint();
//                        return;
//                    }
//                }
//                EditorGUILayout.EndVertical();
//            }
//            EditorGUILayout.EndHorizontal();
//        }

//        static private Texture2D MakeTex(int width, int height, Color col)
//        {
//            Color[] pix = new Color[width * height];

//            for (int i = 0; i < pix.Length; i++)
//                pix[i] = col;

//            Texture2D result = new Texture2D(width, height);
//            result.SetPixels(pix);
//            result.Apply();

//            return result;
//        }

//    }



//}
