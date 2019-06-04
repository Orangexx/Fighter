
using UnityEngine;
using QFramework;
using UniRx;

namespace Fighter
{
    [QFramework.QMonoSingletonPath("[Manager]/BgManager")]
    public class BgManager : MonoSingleton<BgManager>
    {
        private bool isInited = false;
        private ResLoader mBgLoader = ResLoader.Allocate();
        private int mCurrentBgMid;
        private int mLeftBgMid;
        private int mRightBgMid;
        private float mBgMidLength;


        private GameObject mResBgFar;
        private GameObject mResBgMid;

        private GameObject mBgFar;
        private GameObject mBgMid;
        private SpriteRenderer mBgFarSprite;
        private SpriteRenderer[] mBgMidSprites = new SpriteRenderer[3];

        private CompositeDisposable mLife = new CompositeDisposable();
        public void Init()
        {
            mResBgFar = mBgLoader.LoadSync<GameObject>(GlobalManager.Instance.GameDevSetting.BgFarCamPrefabPath);
            mResBgMid = mBgLoader.LoadSync<GameObject>(GlobalManager.Instance.GameDevSetting.BgMidPrefabPath);
            mBgMidLength = GlobalManager.Instance.GameDevSetting.BgMidLength * mResBgMid.transform.localScale.x;

            isInited = true;
        }

        public void ShowMainGame()
        {
            mBgFar = mResBgFar.Instantiate();
            mBgFarSprite = mBgFar.GetComponentInChildren<SpriteRenderer>();
            mBgMid = mResBgMid.Instantiate();
            for (int i = 0; i < mBgMidSprites.Length; i++)
            {
                mBgMidSprites = mBgMid.GetComponentsInChildren<SpriteRenderer>();
            }
            SetBackground(true);
            SetBackground(false);
            mLeftBgMid = 0;
            mCurrentBgMid = 1;
            mRightBgMid = 2;

            Observable.EveryLateUpdate().Subscribe(_ =>
            {
                if (!isInited) return;

                if (mBgMidSprites[mCurrentBgMid].transform.position.x + mBgMidLength / 2 < GlobalManager.Instance.GetCharactorPos().x)
                {
                    mBgMidSprites[mLeftBgMid].transform.Translate(new Vector2(mBgMidLength * 3, 0));
                    var tem = mCurrentBgMid;
                    mCurrentBgMid = mRightBgMid;
                    mRightBgMid = mLeftBgMid;
                    mLeftBgMid = tem;
                }

                if (mBgMidSprites[mCurrentBgMid].transform.position.x - mBgMidLength / 2 > GlobalManager.Instance.GetCharactorPos().x)
                {
                    mBgMidSprites[mRightBgMid].transform.Translate(new Vector2(-mBgMidLength * 3, 0));
                    var tem = mCurrentBgMid;
                    mCurrentBgMid = mLeftBgMid;
                    mLeftBgMid = mRightBgMid;
                    mRightBgMid = tem;
                }
            }).AddTo(mLife);
        }

        public void LeaveMainGame()
        {
            mBgFar.DestroySelf();
            mBgFarSprite = null;
            mBgMid.DestroySelf();
            mLife.Clear();
        }

        public bool SetBackground(bool isMid = false)
        {
            if (!isMid)
            {
                mBgFarSprite.sprite = mBgLoader.LoadSprite(GlobalManager.Instance.GameDevSetting.BgFarSpritePath + LevelManager.Instance.CurLevel);
                Debug.LogFormat("[BgFar]:加载Sprite BgFar{0},{1}", LevelManager.Instance.CurLevel, mBgFarSprite.sprite != null);
                if (mBgFarSprite.sprite == null) return false;
            }
            else
            {
                for (int i = 0; i < mBgMidSprites.Length; i++)
                {
                    mBgMidSprites[i].sprite = mBgLoader.LoadSprite(GlobalManager.Instance.GameDevSetting.BgMidSpritePath + LevelManager.Instance.CurLevel);
                    Debug.LogFormat("[BgMid]:加载Sprite BgMid{0},{1}", LevelManager.Instance.CurLevel, mBgMidSprites[i].sprite != null);
                    if (mBgMidSprites[i].sprite == null) return false;
                }
            }
            return true;
        }
    }
}

