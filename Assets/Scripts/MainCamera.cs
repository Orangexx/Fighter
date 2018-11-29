using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Timers;
using System;

public class MainCamera : MonoBehaviour {

    public GameObject target;
    public float width= 100;
    public float height = 100; 

    private Vector3 offset;

	// Use this for initialization
	void Start () {
        width = 100;
        height = 100;
        offset = transform.position - target.transform.position;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = new Vector3
            (Mathf.Clamp(target.transform.position.x + offset.x,-width,width),
            transform.position.y,
            Mathf.Clamp(target.transform.position.z+offset.z, -height, height));
    }
}
