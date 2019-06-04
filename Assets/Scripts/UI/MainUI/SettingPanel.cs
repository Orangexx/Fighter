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
    public class SettingPanelData : UIPanelData
    {
        // TODO: Query Mgr's Data
    }

    public partial class SettingPanel : UIPanel
    {
        protected override void InitUI(IUIData uiData = null)
        {
            mData = uiData as SettingPanelData ?? new SettingPanelData();
            //please add init code here
        }

        protected override void RegisterUIEvent()
        {
            Button_Resume.AddCallback(() =>
                {
                    GlobalManager.Instance.isPaused = false;
                    Time.timeScale = 1;
                    UIMgr.ClosePanel<SettingPanel>();
                });

            Button_InputSet.AddCallback(() =>
            {
                UIMgr.OpenPanel<InputSetList>();
            });

            Button_MainMenu.AddCallback(() =>
            {
                GlobalManager.Instance.HideMainScene();
            });

            Button_AudioSet.AddCallback(() =>
            {
                UIMgr.OpenPanel<AudioSettingPanel>();
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
            Debug.Log("[ SettingPanel:]" + content);
        }
    }
}