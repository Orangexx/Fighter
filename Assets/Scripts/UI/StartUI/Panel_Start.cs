/****************************************************************************
 * 2019.3 DESKTOP-CFTQOJO
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.SceneManagement;

namespace QFramework.Fighter
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
            BtnSetting.AddCallback(new UnityEngine.Events.UnityAction(() =>
            {
                UIMgr.OpenPanel<InputSetList>();
            }));

            BtnStart.AddCallback(new UnityEngine.Events.UnityAction(() =>
            {
                GlobalManager.Instance.MapLevel = 1;
                SceneManager.LoadScene(1);
                UIMgr.CloseAllPanel();
            }));
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