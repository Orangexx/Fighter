using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
    public class RoundTimer : MonoBehaviour
    {
        public SpriteRenderer[] Digits;
        public Sprite[] DigitSprites;
        public int StartTime;
        public UnityEvent OnZero;
        public bool IsPlaying { get; set; }

        private float m_Time;
        private int m_LastTime;
        private int m_Base;

        void Awake()
        {
            m_Base = DigitSprites.Length;
        }

        void OnEnable()
        {
            ResetTimer();
        }

        void LateUpdate()
        {
            if(IsPlaying)
            {
                if (m_Time > 0)
                    m_Time -= Time.deltaTime;
                else
                    m_Time = 0f;

                var seconds = Mathf.CeilToInt(m_Time);

                if (m_LastTime != seconds)
                {
                    UpdateDigits(m_LastTime = seconds);

                    if (m_LastTime == 0 && OnZero != null)
                        OnZero.Invoke();
                }
            }
        }

        public void ResetTimer()
        {
            m_Time = StartTime;
            UpdateDigits(Mathf.CeilToInt(m_Time));
            IsPlaying = true;
        }

        void UpdateDigits(int number)
        {
            if (m_Base == 0) return;

            for (int i = 0, j = 1; i < Digits.Length; i++, j *= 10)
            {
                Digits[i].sprite = DigitSprites[Mathf.FloorToInt(number / j) % m_Base];
            }
        }
    }
}