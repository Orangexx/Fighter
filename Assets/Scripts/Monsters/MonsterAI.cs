using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour,ICharacter
{

    [SerializeField] protected Transform mTarget;
    [SerializeField] protected ACTFSM mFSM;

    public bool FlipX { get; private set; }

    public void HitboxContact(ContactData contactData)
    {

    }

    void Awake()
    {
        FlipX = false;
        //mFSM = this.GetComponent<ACTFSM>();
    }

    // Use this for initialization
    void Start ()
    {
		
	}
}
