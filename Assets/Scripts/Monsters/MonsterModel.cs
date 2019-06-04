using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fighter
{
    class MonsterModel : Model
    {
        private bool mIsInited = false;
        private string mName;
        public int Exp;

        public override void Init(IConfig cfg)
        {
            base.Init(cfg);
            MonsterConfig mCfg = (MonsterConfig)cfg;
            mName = mCfg.Name;
            Hp = mCfg.HP;
            Power = mCfg.Power;
            Poise = mCfg.Poise;
            PoiseValue = 0;
            Speed = mCfg.Speed;
            Exp = mCfg.Exp;
            mIsInited = true;
        }

        private void LateUpdate()
        {
            if (!mIsInited)
                return;
            if (Hp <= 0)
                EnemyManager.Instance.OnEnemyDead(mName, this.gameObject);
        }
    }

}

