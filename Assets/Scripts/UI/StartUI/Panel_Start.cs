/****************************************************************************
 * 2019.3 DESKTOP-CFTQOJO
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.SceneManagement;

namespace Fighter
{
	public class Panel_StartData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class Panel_Start : UIPanel
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as Panel_StartData ?? new Panel_StartData();
			//please add init code here
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
            BtnIntpuSet.AddCallback(new UnityEngine.Events.UnityAction(() =>
            {
                UIMgr.OpenPanel<InputSetList>();
            }));

            BtnStart.AddCallback(new UnityEngine.Events.UnityAction(() =>
            {
                GlobalManager.Instance.InitMainScene(1);
            }));

            BtnAudioSet.AddCallback(() =>
            {
                UIMgr.OpenPanel<AudioSettingPanel>();
            });

            BtnLoad.AddCallback(() =>
            {
                UIMgr.OpenPanel<LevelPanel>();
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
		}

		void ShowLog(string content)
		{
			Debug.Log("[ Panel_Start:]" + content);
		}
	}
}