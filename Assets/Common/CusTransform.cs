using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CusTransform
{
    public static void SetPositionX(this Transform t, float newX)
    {
        t.position = new Vector3(newX, t.position.y, t.position.z);
    }

    public static void SetPositionY(this Transform t, float newY)
    {
        t.position = new Vector3(newY, t.position.y, t.position.z);
    }

    public static void SetPositionZ(this Transform t, float newZ)
    {
        t.position = new Vector3(newZ, t.position.y, t.position.z);
    }

}
