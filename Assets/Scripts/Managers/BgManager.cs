using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

[QFramework.QMonoSingletonPath("[Manager]/BgManager")]
public class BgManager : MonoSingleton<BgManager>
{
    private bool isInited = false;
    private ResLoader mBgLoader = ResLoader.Allocate();
    private int mCurrentBgMid;
    private int mLeftBgMid;
    private int mRightBgMid;
    private float mBgMidLength;

    private SpriteRenderer mBgFar;
    private SpriteRenderer[] mBgMids = new SpriteRenderer[3];

    public void Init()
    {
        mBgFar = mBgLoader.LoadSync<GameObject>(GlobalManager.Instance.GameDevSetting.BgFarCamPrefabPath)
                .Instantiate()
                .GetComponentInChildren<SpriteRenderer>();

        var bgMid = mBgLoader.LoadSync<GameObject>(GlobalManager.Instance.GameDevSetting.BgMidPrefabPath)
                      .Instantiate();
        for (int i = 0; i < mBgMids.Length; i++)
        {
            mBgMids = bgMid.GetComponentsInChildren<SpriteRenderer>();
        }


        SetBackground(true);
        SetBackground(false);

        mLeftBgMid = 0;
        mCurrentBgMid = 1;
        mRightBgMid = 2;
        mBgMidLength = GlobalManager.Instance.GameDevSetting.BgMidLength * bgMid.transform.localScale.x;

        isInited = true;
    }

    public bool SetBackground(bool isMid = false)
    {
        if (!isMid)
        {
            mBgFar.sprite = mBgLoader.LoadSprite(GlobalManager.Instance.GameDevSetting.BgFarSpritePath + GlobalManager.Instance.MapLevel);
            Debug.LogFormat("[BgFar]:加载Sprite BgFar{0},{1}", GlobalManager.Instance.MapLevel, mBgFar.sprite != null);
            if (mBgFar.sprite == null) return false;
        }
        else
        {
            for (int i = 0; i < mBgMids.Length; i++)
            {
                mBgMids[i].sprite = mBgLoader.LoadSprite(GlobalManager.Instance.GameDevSetting.BgMidSpritePath + GlobalManager.Instance.MapLevel);
                Debug.LogFormat("[BgMid]:加载Sprite BgMid{0},{1}", GlobalManager.Instance.MapLevel, mBgMids[i].sprite != null);
                if (mBgMids[i].sprite == null) return false;
            }
        }
        return true;
    }
    private void LateUpdate()
    {
        if (!isInited) return;

        if (mBgMids[mCurrentBgMid].transform.position.x + mBgMidLength /2  < GlobalManager.Instance.GetCharactorPos().x)
        {
            mBgMids[mLeftBgMid].transform.Translate(new Vector2(mBgMidLength * 3, 0));
            var tem = mCurrentBgMid;
            mCurrentBgMid = mRightBgMid;
            mRightBgMid = mLeftBgMid;
            mLeftBgMid = tem;
        }

        if (mBgMids[mCurrentBgMid].transform.position.x - mBgMidLength /2 > GlobalManager.Instance.GetCharactorPos().x)
        {
            mBgMids[mRightBgMid].transform.Translate(new Vector2(-mBgMidLength * 3, 0));
            var tem = mCurrentBgMid;
            mCurrentBgMid = mLeftBgMid;
            mLeftBgMid = mRightBgMid;
            mRightBgMid = tem;
        }
    }
}
