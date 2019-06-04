using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Timers;
using System;

namespace Fighter
{
    public class MainCamera : MonoBehaviour
    {

        [SerializeField] private float mLeftBound = 0;
        [SerializeField] private float mRightBound = 100;
        [SerializeField] public float HalfWidth { private set; get; }
        private float mHeight = 100;
        private Vector2 mOffset;
        private Camera mCamera;

        // Use this for initialization
        void Start()
        {
            mCamera = this.GetComponent<Camera>();
            Vector2 target = GlobalManager.Instance.GetCharactorPos();
            mOffset = new Vector2(transform.position.x - target.x, transform.position.y - target.y);
            HalfWidth = mCamera.orthographicSize * mCamera.aspect;
            mRightBound = 1000;
        }

        // Update is called once per frame
        void LateUpdate()
        {

            transform.position = new Vector3
                (Mathf.Clamp(GlobalManager.Instance.GetCharactorPos().x + mOffset.x, mLeftBound + HalfWidth, mRightBound - HalfWidth),
                transform.position.y,
                transform.position.z);
        }

        public void StopMove()
        {

        }

        // 待修改 LateUpdate 逻辑和方法逻辑，实现限制范围
        public void SetRange(float leftBound, float rightBound)
        {
            mLeftBound = leftBound;
            mRightBound = rightBound;
        }

        public void ZoomCamera()
        {

        }

        public void ShakeCamera()
        {

        }
    }
}


