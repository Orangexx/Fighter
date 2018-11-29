/****************************************************************************
 * 2018.11 DESKTOP-CFTQOJO
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Events;

namespace QFramework.Example
{
	public class InputSetItemData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class InputSetItem : UIPanel
	{
        public void Init(InputSetting inputSetData,UnityAction onButton)
        {
            mKeyName.SetText(inputSetData.Type);
            mText.SetText(inputSetData.Key);
            mButton.RemoveAllCallback();
            mButton.AddCallback(onButton);
        }

        public void UpdateView(InputSetting inputSetData)
        {
            mKeyName.SetText(inputSetData.Type);
            mText.SetText(inputSetData.Key);
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
			Debug.Log("[ InputSetItem:]" + content);
		}
	}
}