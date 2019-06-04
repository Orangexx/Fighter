using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fighter
{
    public class CharacterModel : Model
    {
        public Action OnHpChanged;
        public Action OnExpChanged;
        public Action OnLevelChanged;
        public int Exp;
        public int ExpNeed;

        public override void Init(IConfig cfg)
        {
            base.Init(cfg);
            CharacterConfig mCfg = (CharacterConfig)cfg;
            Hp = mCfg.HP;
            Power = mCfg.Power;
            Poise = mCfg.Poise;
            Level = mCfg.Level;
            PoiseValue = 0;
            ExpNeed = mCfg.Exp;
            Exp = 0;
            OnHpChanged += new Action(() =>
            {
                if (Hp <= 0)
                    GlobalManager.Instance.GameOver(false);
            });
        }

        public void AddExp(int exp)
        {
            Exp += exp;
            OnExpChanged.Invoke();
            if (Exp >= ExpNeed)
                LevelUp();
        }

        private void LevelUp()
        {
            Level++;

            var mCfg = ConfigManager.Instance.GetCharacterConfig(Level);
            if (mCfg == null)
                return;
            Hp = mCfg.HP;
            Power = mCfg.Power;
            Poise = mCfg.Poise;
            Level = mCfg.Level;
            PoiseValue = 0;
            ExpNeed = mCfg.Exp;
            Exp = 0;

            OnLevelChanged.Invoke();
        }
    }

}
