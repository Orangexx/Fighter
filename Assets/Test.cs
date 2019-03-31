using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour {

    XHitboxAnimation m_Data;
	// Use this for initialization
	void Start () {
        m_Data = AssetDatabase.LoadAssetAtPath<XHitboxAnimation>("Assets/Resources/AnimaBoxDatas/Fighter/Fighter_Attack1.asset");
        Debug.LogFormat("{0},{1},{2},{3},{4}", m_Data.name, m_Data.strength, m_Data.clip, m_Data.force,m_Data.framedata.Length);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
