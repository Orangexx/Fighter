using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class GameUtils
{
    public static IEnumerator Wait(float t, Action action)
    {
        yield return new WaitForSeconds(t);//运行到这，暂停t秒

        action.Invoke();
    }
}
