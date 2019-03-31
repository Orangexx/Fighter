using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create AnimaBoxData1111 ")]
[Serializable]
public class XHitboxAnimation : ScriptableObject
{
    public AnimationClip clip;
    public XHitboxAnimationFrame[] framedata;
    public float strength;
    public float damage;
    public Vector2 force;
    public GameObject hitFx;
}

[Serializable]
public struct XHitboxAnimationFrame
{
    public HitboxColliderData[] collider;
    public float time;
}

[Serializable]
public struct HitboxColliderData
{
    public RectInt rect;
    public HitboxType type;
}

[Serializable]
public enum HitboxType
{
    TRIGGER = 0,
    HURT = 1,
    GUARD = 2,
    ARMOR = 3,
    GRAB = 4,
    TECH = 5
}
