
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using QFramework;
using System;

public class StatePanelUI : MonoSingleton<StatePanelUI>
{
    public Transform StateListContent;
    public StateItemUI StateItemTem;
    public Animator TargetAnimator;
    public Action<string> OnSelState;

    private void Awake()
    {
        StateItemTem.gameObject.SetActive(false);
        var clips = AnimationUtils.GetAnimationClips(TargetAnimator);
        foreach (var item in clips)
        {
            var ui = Instantiate(StateItemTem,StateListContent);
            ui.gameObject.SetActive(true);
            ui.SelBtn.AddCallback(() => { OnSelState.Invoke(item.name); });
            ui.StateName.text = item.name;
        }
    }
}
#endif