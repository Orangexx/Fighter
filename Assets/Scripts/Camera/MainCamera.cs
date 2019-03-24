using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Timers;
using System;

public class MainCamera : MonoBehaviour {

    private float mWidth= 100;
    private float mHeight = 100; 
    private Vector2 mOffset;

	// Use this for initialization
	void Start () {

        Vector2 target = GlobalManager.Instance.GetCharactorPos();
        mOffset = new Vector2(transform.position.x - target.x, transform.position.y - target.y);
    }
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = new Vector3
            (Mathf.Clamp(GlobalManager.Instance.GetCharactorPos().x + mOffset.x,-mWidth, mWidth),
            transform.position.y,
            transform.position.z);
    }

    public void StopMove()
    {

    }

    // 待修改 LateUpdate 逻辑和方法逻辑，实现限制范围
    public void SetRange(float width,float height)
    {
        mWidth = width;
        mHeight = height;
    }

    public void ZoomCamera()
    {

    }

    public void ShakeCamera()
    {

    }
}
