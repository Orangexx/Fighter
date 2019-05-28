
#if UNITY_EDITOR
using RuntimeEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System.Linq;
using QFramework;
using System.IO;
using UnityEditor;

namespace RuntimeEditor
{
    public class NextStatePanelUI : MonoSingleton<NextStatePanelUI>
    {
        [SerializeField] private NextStateItemUI mStateTem;
        [SerializeField] private Transform mListContent;
        [SerializeField] private Animator mAnimator;
        [SerializeField] private NextStateAddUI mStateTransAddUI;

        private List<NextStateItemUI> lst_nextState_item = new List<NextStateItemUI>();

        private string mStateMapPath;
        private Dictionary<string, StateMap> dic_name_stateMap  = new Dictionary<string, StateMap>();
        private AnimationClip[] animationClips;
        private Dictionary<string, AnimationClip> dic_name_clip = new Dictionary<string, AnimationClip>();
        private AnimationClip mCurAnimaClip;
        private StateMap mCurStateMap;
        //初始化 Dropdown，载入数据，载入人物
        private void Awake()
        {
            mStateMapPath = "Assets/Resources/StateMapDatas/" + mAnimator.gameObject.name + "StateMap";
            if (!Directory.Exists(mStateMapPath))
            {
                Directory.CreateDirectory(mStateMapPath);
            }


            animationClips = AnimationUtils.GetAnimationClips(mAnimator);
            foreach (var item in animationClips)
                dic_name_clip.Add(item.name, item);

            foreach (var item in dic_name_clip)
            {
                var filePath = String.Format(mStateMapPath + "/{0}.asset", mAnimator.gameObject.name + "_" + item.Key);

#if UNITY_EDITOR
                if (!File.Exists(filePath))
                {
                    var stateMap = ScriptableObject.CreateInstance<StateMap>();
                    AssetDatabase.CreateAsset(stateMap,filePath);
                }

                    dic_name_stateMap.Add(item.Key,
                    AssetDatabase.LoadAssetAtPath<StateMap>(
                        String.Format(mStateMapPath + "/{0}.asset", mAnimator.gameObject.name + "_" + item.Key)));
            }
#endif


            mStateTransAddUI.NextStateSel.ClearOptions();
            mStateTem.TriggerTem.TriggerSel.ClearOptions();
            mStateTem.StateSetTem.TriggerSel.ClearOptions();
            mStateTem.TriggerTem.TriggerSel.AddOptions(new List<string>(Enum.GetNames(typeof(TriggerAction))));
            mStateTem.StateSetTem.TriggerSel.AddOptions(new List<string>(Enum.GetNames(typeof(TriggerAction))));
            mStateTransAddUI.NextStateSel.AddOptions(animationClips.Select((item) => { return item.name; }).ToList());
            mStateTem.gameObject.SetActive(false);
            mStateTem.StateSetTem.gameObject.SetActive(false);
            mStateTem.TriggerTem.gameObject.SetActive(false);

            mStateTransAddUI.AddBtn.AddCallback(() => 
            {
                if (mCurStateMap.Dic_name_triggers == null)
                    mCurStateMap.Dic_name_triggers = new Dictionary<string, List<StateTriggers>>();

                if (!mCurStateMap.Dic_name_triggers.ContainsKey(animationClips[mStateTransAddUI.NextStateSel.value].name))
                    mCurStateMap.Dic_name_triggers[animationClips[mStateTransAddUI.NextStateSel.value].name] = new List<StateTriggers>();
                _RefreshPanel();
            });

            StatePanelUI.Instance.OnSelState += _OnSelState;
        }

        private void _OnSelState(string stateName)
        {
            mCurAnimaClip = dic_name_clip[stateName];
            mCurStateMap = dic_name_stateMap[stateName];

            if (lst_nextState_item.Count > 0)
            {
                CommonUtils.DesAllGO(lst_nextState_item.Select((itemui) => { return itemui.gameObject; }).ToList());
                lst_nextState_item.Clear();
            }
            foreach (var item in mCurStateMap.Dic_name_triggers)
            {
                string nextStateStr = item.Key;
                var ui = Instantiate(mStateTem,mListContent);
                ui.gameObject.SetActive(true);
                lst_nextState_item.Add(ui);
                var setUI = Instantiate(mStateTem.StateSetTem, ui.transform);
                setUI.gameObject.SetActive(true);
                // 添加
                setUI.AddBtn.AddCallback(() => { _AddTrigger(nextStateStr, (TriggerAction)setUI.TriggerSel.value); });
                // 删除
                setUI.DeleteBtn.AddCallback(() => { _DeleteNextState(nextStateStr); });
                setUI.StateSel.text = nextStateStr;

                if(ui.Triggers.Count>0)
                {
                    CommonUtils.DesAllGO(ui.Triggers.Select((itemui) => { return itemui.gameObject; }).ToList());
                    ui.Triggers.Clear();
                }
                foreach (var trigger in item.Value)
                {
                    var triggerUI = Instantiate(mStateTem.TriggerTem, ui.transform);
                    triggerUI.gameObject.SetActive(true);
                    ui.Triggers.Add(triggerUI);
                    triggerUI.TriggerName.text = "todo";
                    triggerUI.TriggerSel.value = (int)trigger.trigger;
                    //待做，分隔符以及处理方法
                    triggerUI.ParamsInput.text = trigger.ParamToString();
                    triggerUI.TriggerSel.onValueChanged.AddListener((index) => { _OnTriggerChanged(trigger, (TriggerAction)index); });
                    triggerUI.ParamsInput.onEndEdit.AddListener((str) => { _OnParamStrChanged(trigger,str); });
                    //todo，完成删除
                    triggerUI.DeleteBtn.AddCallback(() => { _DeleteTrigger(nextStateStr, trigger); });
                }
            }
        }

        private void _RefreshPanel()
        {
            _OnSelState(mCurAnimaClip.name);
        }

        private void _AddTrigger(string nextState, TriggerAction actionType)
        {
            mCurStateMap.Dic_name_triggers[nextState].Add
                (new StateTriggers()
                {
                    param = new string[0],
                    trigger = actionType
                });
            _RefreshPanel();
        }

        private void _DeleteNextState(string nextState)
        {
            if (mCurStateMap.Dic_name_triggers.ContainsKey(nextState))
                mCurStateMap.Dic_name_triggers.Remove(nextState);
            _RefreshPanel();
        }

        private void _DeleteTrigger(string nextState, StateTriggers trigger)
        {
            if (!mCurStateMap.Dic_name_triggers.ContainsKey(nextState))
                return;
            if (!mCurStateMap.Dic_name_triggers[nextState].Contains(trigger))
                return;

            mCurStateMap.Dic_name_triggers[nextState].Remove(trigger);
            _RefreshPanel();

        }

        private void _OnParamStrChanged(StateTriggers trigger,string paramStr)
        {
            trigger.param = paramStr.Split(',');
            _RefreshPanel();
        }

        private void _OnTriggerChanged(StateTriggers trigger, TriggerAction actionType)
        {
            trigger.trigger = actionType;
            _RefreshPanel();
        }
    }
}
#endif
