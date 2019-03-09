using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create SkillConfig ")]
public class SkillConfig : ScriptableObject
{
    public int ID;
    public string Name;
    public List<Box> AttackBoxs;
    public List<Box> HurtBoxs;
}


[System.Serializable]
public struct Box
{
    public float PosX;
    public float PosY;
    public float High;
    public float Width;
}
