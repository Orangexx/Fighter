using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using QFramework;

[CreateAssetMenu(menuName = "MonsterModel ")]
public class MonsterModel : ScriptableObject
{
    public int Hp;
    public int Velocity;
    public int Power;
}
