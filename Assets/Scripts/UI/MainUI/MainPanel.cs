/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace Fighter
{
    public class MainPanelData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class MainPanel : UIPanel
	{
        private CharacterModel mCharacterModel;

		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as MainPanelData ?? new MainPanelData();
            //please add init code here
            mCharacterModel = GlobalManager.Instance.Character.GetComponent<CharacterModel>();
            mCharacterModel.OnHpChanged += _SetHpSlider;
            mCharacterModel.OnExpChanged += _SetExpSlider;
            mCharacterModel.OnLevelChanged += _SetLevel;
            Slider_Hp.maxValue = mCharacterModel.Hp;
            Slider_Hp.value = mCharacterModel.Hp;
            Slider_Exp.value = mCharacterModel.Exp; ;
            Slider_Exp.maxValue = mCharacterModel.ExpNeed;
            Text_Level.text = "Lv." + mCharacterModel.Level.ToString();
		}


		protected override void RegisterUIEvent()
		{
            Setting.AddCallback(() =>
            {
                GlobalManager.Instance.isPaused = true;
                Time.timeScale = 0;
                UIMgr.OpenPanel<SettingPanel>();
            });

		}

		protected override void OnShow()
		{
			base.OnShow();
		}

		protected override void OnHide()
		{
			base.OnHide();
		}

		protected override void OnClose()
		{
			base.OnClose();
            mCharacterModel.OnHpChanged -= _SetHpSlider;
            mCharacterModel.OnExpChanged -= _SetExpSlider;
            mCharacterModel.OnLevelChanged -= _SetLevel;
            Setting.RemoveAllCallback();
        }

        private void _SetHpSlider()
        {
            Slider_Hp.value = mCharacterModel.Hp;
        }

        private void _SetExpSlider()
        {
            Slider_Exp.value = mCharacterModel.Exp;
        }

        private void _SetLevel()
        {
            Text_Level.text = "Lv." + mCharacterModel.Level.ToString();
            Slider_Hp.maxValue = mCharacterModel.Hp;
            Slider_Hp.value = mCharacterModel.Hp;
            Slider_Exp.maxValue = mCharacterModel.ExpNeed;
            Slider_Exp.value = mCharacterModel.Exp;
        }

        void ShowLog(string content)
		{
			Debug.Log("[ MainPanel:]" + content);
		}
	}
}