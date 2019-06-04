using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFramework;
using System.Threading.Tasks;

namespace Fighter
{
    [QFramework.QMonoSingletonPath("[Manager]/AudioManager")]
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private const string PATH = "Resources/Audios/";
        private const int MAX_EFFECTSOURCES = 3;
        private AudioSource[] mEffectSources;
        private AudioSource mBGMSource;
        private Dictionary<string, AudioClip> mDic_name_clip = new Dictionary<string, AudioClip>();
        private ResLoader mResloder = ResLoader.Allocate();
        public void Init()
        {
            _InitAudioSource();
            _InitAuidoClip();
        }

        public void PlayEffect(string name)
        {
            if (!mDic_name_clip.ContainsKey(name))
                return;

            for (int i = 0; i < mEffectSources.Length; i++)
            {
                if (!mEffectSources[i].isPlaying)
                {
                    mEffectSources[i].clip = mDic_name_clip[name];
                    mEffectSources[i].Play();
                }
            }
        }

        public void PlayBGM(string name)
        {
            if (mDic_name_clip.ContainsKey(name))
            {
                mBGMSource.clip = mDic_name_clip[name];
                mBGMSource.Play();
            }
        }

        public void StopBGM()
        {
            mBGMSource.Stop();
        }

        public void SetEffectVolume(float volume)
        {
            if (volume > 1 || volume < 0) return;
            for (int i = 0; i < mEffectSources.Length; i++)
            {
                mEffectSources[i].volume = volume;
            }
        }

        public void SetBGMVolume(float volume)
        {
            if (volume > 1 || volume < 0) return;
            mBGMSource.volume = volume;
        }

        public float GetBGMVolume()
        {
            return mBGMSource.volume;
        }

        public float GetEffectVolume()
        {
            return mEffectSources[0].volume;
        }

        private void _InitAudioSource()
        {
            var sources = GetComponentsInChildren<AudioSource>();

            if (sources == null || sources.Length < MAX_EFFECTSOURCES + 1)
            {
                sources = new AudioSource[MAX_EFFECTSOURCES + 1];
                for (int i = 0; i < sources.Length; i++)
                {
                    var newGameObject = new GameObject("AudioSource(" + i + ")");
                    newGameObject.transform.SetParent(transform, false);
                    var audio = newGameObject.AddComponent<AudioSource>();
                    audio.enabled = true;
                    audio.loop = false;
                    audio.volume = 1;
                    sources[i] = audio;
                }
            }

            mBGMSource = sources[0];
            mBGMSource.loop = true;
            mEffectSources = new AudioSource[MAX_EFFECTSOURCES];
            for (int i = 0; i < mEffectSources.Length; i++)
                mEffectSources[i] = sources[i + 1];
        }
        private void _InitAuidoClip()
        {
            foreach (var name in GlobalManager.Instance.GameDevSetting.AudioNames)
            {
                var audio = mResloder.LoadSync<AudioClip>(PATH + name);
                if (!mDic_name_clip.ContainsKey(name))
                    mDic_name_clip[name] = audio;
            }
        }
    }

}

