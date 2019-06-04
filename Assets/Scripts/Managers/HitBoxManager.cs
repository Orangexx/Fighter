using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UniRx;

[QFramework.QMonoSingletonPath("[Manager]/HitBoxManager")]
public class HitBoxManager : MonoSingleton<HitBoxManager>
{
    private GameObject HitFx;
    private ResLoader mResLoader = ResLoader.Allocate();
    private CompositeDisposable mLife = new CompositeDisposable();

    public void Init()
    {
        HitFx = mResLoader.LoadSync<GameObject>("Resources/Prefabs/HitFXPrefab").Instantiate();
    }

    public void ShowMainGame()
    {
        Observable.EveryLateUpdate().Subscribe(_ =>
        {
            mContackPairs.Sort(ContactComparison);

            for (int i = 0; i < mContackPairs.Count; i++)
                mContackPairs[i].a.HandleContact(mContackPairs[i].b);

            mContackPairs.Clear();
        }).AddTo(mLife);
    }

    public void LeaveMainGame()
    {
        mLife.Clear();
    }

    private struct ContactPair
    {
        public HitBoxFeeder a;
        public HitBoxFeeder b;
    }

    private List<ContactPair> mContackPairs = new List<ContactPair>();

    public void AddContact(HitBoxFeeder a,HitBoxFeeder b)
    {
        mContackPairs.Add(new ContactPair { a = a, b = b });
    }

    public void PlayHitFX(Vector3 position)
    {
        HitFx.SetActive(false);
        HitFx.transform.position = position;
        HitFx.SetActive(true);
    }

    private int ContactComparison(ContactPair x, ContactPair y)
    {
        return ContactDistance(x) - ContactDistance(y);
    }

    private int ContactDistance(ContactPair pair)
    {
        Vector3 aPos = pair.a.transform.position, bPos = pair.b.transform.position;
        float xDirection = Mathf.Sign(bPos.x - aPos.x);

        aPos.x -= xDirection * (pair.a.Collider.size.x / 2f);
        bPos.x += xDirection * (pair.b.Collider.size.x / 2f);

        return Mathf.RoundToInt(Vector3.Distance(aPos, bPos) * 1000f);
    }

}

public struct ContactData
{
    public HitBoxFeeder MyHitbox;
    public HitBoxFeeder TheirHitbox;

    public float Damage;
    public float PoiseDamage;
    /// <summary>
    /// Identifier of the hit effect this attack uses.
    /// </summary>
    //public int fxID;
    /// <summary>
    /// Amount of force to receive from this attack
    /// </summary>
    public Vector2 Force;
    /// <summary>
    /// Intersection point between these two hitboxes. Place a hit effect at this location.
    /// </summary>
    public Vector2 Point;

    public float RemainTime;
    public XFSMLite.XFSMState State;
}



