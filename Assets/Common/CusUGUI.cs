using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public static class CusButton
{
    public static void AddCallback(this Button btn, UnityAction onClickCallback)
    {
        if (btn != null)
            btn.onClick.AddListener(onClickCallback);
    }

    public static void RemoveAllCallback(this Button btn)
    {
        if (btn != null)
            btn.onClick.RemoveAllListeners();
    }
}

public static class CusText
{
    public static void SetText(this Text text, string content)
    {
        if (text == null)
            return;
        text.text = content;
    }
}


