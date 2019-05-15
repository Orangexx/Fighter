using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UniRx;

public class MonsterAI : MonoBehaviour,ICharacter
{

    [SerializeField] protected Transform mTarget;
    [SerializeField] protected ACTFSM mFSM;

    public bool FlipX { get; private set; }

    void Awake()
    {
        FlipX = false;
        mFSM = this.GetComponent<ACTFSM>();
    }

    void Start()
    {

    }

    public virtual void HitboxContact(ContactData contactData)
    {

    }

    //public virtual void 


    
}
