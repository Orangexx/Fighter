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
	public class LevelPanelData : UIPanelData
	{
        // TODO: Query Mgr's Data
        public int Count;
        public List<Sprite> lst_level_sprite = new List<Sprite>(); 
	}

	public partial class LevelPanel : UIPanel
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as LevelPanelData ?? new LevelPanelData();
            mData.Count = ConfigManager.Instance.GetLevelCount();

            var resLoader = ResLoader.Allocate();

            for (int i = 0; i < mData.Count; i++)
            {
                mData.lst_level_sprite.Add(
                    resLoader.LoadSprite(
                        GlobalManager.Instance.GameDevSetting.BgMidSpritePath + 
                        (i + 1).ToString()));
            }
            
			//please add init code here
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
            BtnReturn.AddCallback(() =>
            {
                UIMgr.Push<LevelPanel>();
            });
		}

		protected override void OnShow()
		{
			base.OnShow();

            for (int i = 0; i < mData.Count; i++)
            {
                BtnLevel.Instantiate()
                    .Parent(Content)
                    .LocalScaleIdentity()
                    .SetItem(mData.lst_level_sprite[i], i + 1);
            }
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
			Debug.Log("[ LevelPanel:]" + content);
		}
	}
}