/****************************************************************************
 * 2018.11 DESKTOP-CFTQOJO
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Fighter
{
    public class InputSetListData : UIPanelData
    {
        // TODO: Query Mgr's Data
    }

    public partial class InputSetList : UIPanel
    {
        private Sqlite mSetSqlite;
        private List<InputSetting> mInputSettings;
        private InputSetItem mSelInputItem;
        private InputSetting mSelInputData;

        protected override void InitUI(IUIData uiData = null)
        {
            mData = uiData as InputSetListData ?? new InputSetListData();
            //please add init code here
            mSetSqlite = new Sqlite(Application.dataPath + "/Resources/SQLites/InputSetting.db");
            mInputSettings = mSetSqlite.SelectTable<InputSetting>();
            _InitItems();
        }

        protected override void RegisterUIEvent()
        {
            mSaveBtn.AddCallback(new UnityEngine.Events.UnityAction(() =>
            {
                mSetSqlite.UpdateTable(mInputSettings, "Type");
                if(GlobalManager.Instance.Charactor != null)
                    if(GlobalManager.Instance.Charactor.GetComponent<CharacterFSM>() != null)
                    {
                        GlobalManager.Instance.Charactor.GetComponent<CharacterFSM>().ReloadInputKey();
                    }
            }));

            mCancelBtn.AddCallback(new UnityEngine.Events.UnityAction(() =>
            {
                mInputSettings = mSetSqlite.SelectTable<InputSetting>();
                _InitItems();
            }));

            mReturnBtn.AddCallback(new UnityEngine.Events.UnityAction(() =>
            {
                UIMgr.ClosePanel<InputSetList>();
            }));
        }

        void _InitItems()
        {
            mItemRoot.DestroyAllChild();
            mInputSettings.ForEach(setData =>
            {
                mSetItemTem
                .Instantiate()
                .Parent(mItemRoot)
                .LocalIdentity()
                .ApplySelfTo(setItem =>
                {
                    setItem.Init(setData, new UnityEngine.Events.UnityAction(() =>
                    {
                        mSelInputItem = setItem;
                        mSelInputData = setData;
                    }));
                }).Show();
            });
        }

        private void OnGUI()
        {
            Event e = Event.current;
            if (mSelInputItem != null && e.isKey)
            {
                mSelInputData.Key = e.keyCode.ToString();
                mSelInputItem.UpdateView(mSelInputData);
                mSelInputItem = null;
            }
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
            mSetSqlite.Close();
            mInputSettings.Clear();
            mInputSettings = null;
            base.OnClose();
        }

        void ShowLog(string content)
        {
            Debug.Log("[ InputSetList:]" + content);
        }
    }
}