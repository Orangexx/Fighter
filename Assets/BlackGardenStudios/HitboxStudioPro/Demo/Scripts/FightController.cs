using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
    public class FightController : MonoBehaviour
    {
#pragma warning disable 0649
        [Serializable]
        private class Player
        {
            public PlayerController character;
            public Lifebar lifebar;
            public SpriteLabel name;
            public RoundResult[] result;

            private Vector3 startposition;
            private int wins;
            private float maxhealth;

            public void Init()
            {
                maxhealth = character.Health;
                startposition = character.transform.position;
                wins = 0;
            }

            public bool Win()
            {
                result[wins++].Activate();

                return wins == result.Length;
            }

            public void ResetRound()
            {
                character.SuppressInputs = false;
                character.Health = maxhealth;
                character.transform.position = startposition;
                character.Rebind();
            }

            public void ResetMatch()
            {
                ResetRound();
                wins = 0;
                name.SetLabel(character.ActivePalette.Name);
            }
        }

        [SerializeField]
        private Player[] Players;

        public RoundTimer Timer;
        public UnityEvent OnMatchEnd;
        public SpriteRenderer BlackFade;
        public CanvasGroup CanvasFade;

        private WaitForSeconds WaitBetweenRounds = new WaitForSeconds(4f);

        void Awake()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                var j = i;

                Players[i].Init();
                Players[i].lifebar.OnZero += () => EndRound(j);
            }
        }

        void OnEnable()
        {
            StartCoroutine(FadeIn());
            ResetMatch();
        }

        private void ResetRound()
        {
            Timer.ResetTimer();

            for (int i = 0; i < Players.Length; i++)
                Players[i].ResetRound();
        }

        private void ResetMatch()
        {
            Timer.ResetTimer();

            for (int i = 0; i < Players.Length; i++)
                Players[i].ResetMatch();
        }

        private void EndRound(int result)
        {
            bool matchOver = false;

            Timer.IsPlaying = false;
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].character.SuppressInputs = true;
                if (i != result && result >= 0)
                    matchOver |= Players[i].Win();
            }

            StartCoroutine(EndRoundWait(matchOver));
        }

        public void JudgeRound()
        {
            int equal = 0;
            int maxIdx = 0;
            float min = float.MaxValue;

            for (int i = 0; i < Players.Length; i++)
            {
                var health = Players[i].character.Health;

                if (health < min)
                {
                    min = health;
                    maxIdx = i;
                }
                else if (health == min)
                    equal++;
            }

            EndRound((equal == Players.Length - 1) ? -1 : maxIdx);
        }

        private IEnumerator EndRoundWait(bool matchOver)
        {
            yield return WaitBetweenRounds;
            Fade(0.5f);
            yield return false;
            yield return false;
            yield return false;
            yield return false;
            Fade(1f);
            yield return false;
            yield return false;
            yield return false;
            yield return false;
            if (matchOver == true)
                OnMatchEnd.Invoke();
            else
                ResetRound();
            yield return false;
            yield return false;
            yield return false;
            yield return false;
            Fade(0.5f);
            yield return false;
            yield return false;
            yield return false;
            yield return false;
            Fade(0f);
        }

        private IEnumerator FadeIn()
        {
            Fade(1f);
            yield return false;
            yield return false;
            yield return false;
            yield return false;
            Fade(0.5f);
            yield return false;
            yield return false;
            yield return false;
            yield return false;
            Fade(0f);
        }

        private void Fade(float alpha)
        {
            BlackFade.color = new Color(0f, 0f, 0f, alpha);
            CanvasFade.alpha = 1f - alpha;
        }
    }
}