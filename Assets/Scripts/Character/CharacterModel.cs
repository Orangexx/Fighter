using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class CharacterModel:Model
{
    public override void Init(IConfig cfg)
    {
        base.Init(cfg);
        CharacterConfig mCfg = (CharacterConfig)cfg;
        Hp = mCfg.HP;
        Power = mCfg.Power;
        Poise = mCfg.Poise;
        Level = mCfg.Level;
        PoiseValue = 0;
        AttackSpeed = mCfg.AttackSpeed;
        MoveSpeed = mCfg.MoveSpeed;
    }
}
