using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MonsterModel : Model
{
    public override void Init(IConfig cfg)
    {
        base.Init(cfg);
        MonsterConfig mCfg = (MonsterConfig)cfg;
        Hp = mCfg.HP;
        Power = mCfg.Power;
        Poise = mCfg.Poise;
        PoiseValue = 0;
        AttackSpeed = mCfg.AttackSpeed;
        MoveSpeed = mCfg.MoveSpeed;
    }
}

