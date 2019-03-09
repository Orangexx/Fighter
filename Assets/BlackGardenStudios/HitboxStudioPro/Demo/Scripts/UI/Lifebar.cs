using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
    public class Lifebar : MonoBehaviour
    {
        public PlayerController Player;
        public RectTransform Fill;
        public int Direction = 1;
        public Action OnZero;

        private float m_LastHealth;
        private float m_MaxHealth;

        private void Awake()
        {
            m_MaxHealth = Player.Health;
        }

        private void OnEnable()
        {
            m_LastHealth = m_MaxHealth;
            UpdateBar(1f);
        }

        private void LateUpdate()
        {
            if(Player.Health != m_LastHealth)
            {
                m_LastHealth = Player.Health;

                UpdateBar(m_LastHealth / m_MaxHealth);

                if (m_LastHealth <= 0f)
                {
                    StartCoroutine(SlowTime());

                    if (OnZero != null)
                        OnZero();
                }
            }
        }
        
        private void UpdateBar(float progress)
        {
            if (Direction == 1)
                Fill.offsetMax = new Vector2(Direction * ((1f - progress) * -750f), 0f);
            else
                Fill.offsetMin = new Vector2(Direction * ((1f - progress) * -750f), 0f);
        }

        private IEnumerator SlowTime()
        {
            Time.timeScale = 0.25f;
            yield return new WaitForSecondsRealtime(3f);
            Time.timeScale = 1f;
        }
    }

}
