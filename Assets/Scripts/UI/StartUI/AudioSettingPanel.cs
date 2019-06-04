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
	public class AudioSettingPanelData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class AudioSettingPanel : UIPanel
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as AudioSettingPanelData ?? new AudioSettingPanelData();
            MusicSlider.maxValue = 1f;
            MusicSlider.minValue = 0f;
            AudioSlider.maxValue = 1f;
            AudioSlider.minValue = 0f;

		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
            MusicSlider.onValueChanged.AddListener((volume) => 
            {
                AudioManager.Instance.SetBGMVolume(volume);
            });

            AudioSlider.onValueChanged.AddListener((volume) =>
            {
                AudioManager.Instance.SetEffectVolume(volume);
            });

            ButtonReturn.AddCallback(() =>
            {
                UIMgr.ClosePanel<AudioSettingPanel>();
            });
		}

		protected override void OnShow()
		{
            base.OnShow();
            AudioSlider.value = AudioManager.Instance.GetEffectVolume();
            MusicSlider.value = AudioManager.Instance.GetBGMVolume();
		}

		protected override void OnHide()
		{
			base.OnHide();
		}

		protected override void OnClose()
		{
            MusicSlider.onValueChanged.RemoveAllListeners();
            AudioSlider.onValueChanged.RemoveAllListeners();
            ButtonReturn.RemoveAllCallback();
			base.OnClose();
		}

		void ShowLog(string content)
		{
			Debug.Log("[ AudioSettingPanel:]" + content);
		}
	}
}