/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.Fighter;
using UnityEngine.SceneManagement;

namespace Fighter
{
	public class LosePanelData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class LosePanel : UIPanel
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as LosePanelData ?? new LosePanelData();
			//please add init code here
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
            Button_MainMenu.AddCallback(() =>
            {
                GlobalManager.Instance.HideMainScene();
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
			Debug.Log("[ LosePanel:]" + content);
		}
	}
}