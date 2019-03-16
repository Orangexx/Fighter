using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMidController :MonoBehaviour
{

    [SerializeField] private Transform mTarget;
    private float mLastX;

    void Awake()
    {
        mLastX = mTarget.position.x;
    }

    private void LateUpdate()
    {
        transform.Translate(new Vector2(mTarget.transform.position.x - mLastX, 0));
        mLastX = mTarget.transform.position.x;
    }
}
