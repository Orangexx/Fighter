/****************************************************************************
 * 2018.11 DESKTOP-CFTQOJO
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
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
            mSetSqlite = new Sqlite(Application.dataPath + "/SQLites/InputSetting.db");
            mInputSettings = mSetSqlite.SelectTable<InputSetting>();
            _InitItems();
        }

        protected override void RegisterUIEvent()
        {
            mSaveBtn.AddCallback(new UnityEngine.Events.UnityAction(() =>
            {
                mSetSqlite.UpdateTable(mInputSettings, "Type");
            }));

            mCancelBtn.AddCallback(new UnityEngine.Events.UnityAction(() =>
            {
                mInputSettings = mSetSqlite.SelectTable<InputSetting>();
                _InitItems();
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