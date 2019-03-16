using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Timers;
using System;

public class MainCamera : MonoBehaviour {

    public float width= 100;
    public float height = 100; 
    private Vector2 mOffset;

	// Use this for initialization
	void Start () {

        Vector2 target = GlobalManager.Instance.GetCharactorPos();
        mOffset = new Vector2(transform.position.x - target.x, transform.position.y - target.y);
    }
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = new Vector3
            (Mathf.Clamp(GlobalManager.Instance.GetCharactorPos().x + mOffset.x,-width,width),
            transform.position.y,
            transform.position.z);
    }
}
