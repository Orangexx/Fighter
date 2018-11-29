using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CusGameObject
{
    public static void CustomActive(this GameObject go ,bool isActive)
    {
        if (go != null)
            go.SetActive(isActive);
    }
}
