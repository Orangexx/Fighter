using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Timers;
using System;

public class MainCamera : MonoBehaviour {

    [SerializeField]private float mLeftBound= 0;
    [SerializeField]private float mRightBound = 10;
    [SerializeField] private float mWidth = 0;
    private float mHeight = 100; 
    private Vector2 mOffset;
    private Camera mCamera;

	// Use this for initialization
	void Start () {
        mCamera = this.GetComponent<Camera>();
        Vector2 target = GlobalManager.Instance.GetCharactorPos();
        mOffset = new Vector2(transform.position.x - target.x, transform.position.y - target.y);
        mWidth = mCamera.orthographicSize * mCamera.aspect;
    }
	
	// Update is called once per frame
	void LateUpdate () {

        transform.position = new Vector3
            (Mathf.Clamp(GlobalManager.Instance.GetCharactorPos().x + mOffset.x,mLeftBound + mWidth , mRightBound - mWidth),
            transform.position.y,
            transform.position.z);
    }

    public void StopMove()
    {

    }

    // 待修改 LateUpdate 逻辑和方法逻辑，实现限制范围
    public void SetRange(float leftBound,float rightBound)
    {
        mLeftBound = leftBound;
        mHeight = rightBound;
    }

    public void ZoomCamera()
    {

    }

    public void ShakeCamera()
    {

    }
}
