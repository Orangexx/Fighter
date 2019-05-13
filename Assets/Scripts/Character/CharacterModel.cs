using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using QFramework;

public class CharacterModel : MonoSingleton<CharacterModel>
{

    public enum PopeType
    {
        hp = 1,
        velocity = 2,
        power = 3
    }

    public IntReactiveProperty Hp { private get; set; }
    public int Velocity { private get; set; }
    public int Level { private get; set; }
    public int SkillPoint { private get; set; }
    public int Power { private get; set; }
    public float Poise;
    public void Init()
    {
        Hp.Value = 100;
        Velocity = 100;
        Level = 0;
        SkillPoint = 1;
        Power = 10;
    }

    public void Study(PopeType studyType)
    {
        SkillPoint--;
        switch (studyType)
        {
            case PopeType.hp:
                Hp.Value += 20;
                break;
            case PopeType.velocity:
                Velocity += 10;
                break;
            case PopeType.power:
                Power += 10;
                break;
            default:
                break;
        }
    }

    public void Hurted(int hurtNum)
    {
        Hp.Value -= hurtNum;
    }

    public void LevelUp()
    {
        Level++;
        SkillPoint++;
    }
}
