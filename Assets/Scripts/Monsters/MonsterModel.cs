using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MonsterModel : Model
{
    private bool mIsInited = false;
    private string mName;

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
        mIsInited = true;
    }

    private void LateUpdate()
    {
        if (!mIsInited)
            return;
        if (Hp < 0)
            EnemyManager.Instance.OnEnemyDead(mName, this.gameObject);
    }
}

