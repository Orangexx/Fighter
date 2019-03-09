using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
    public class CharacterSelect : MonoBehaviour
    {
#pragma warning disable 0649
        [Serializable]
        private class Player
        {
            public Character preview;
            public Character character;
            public InputHistory input;
            public SpriteLabel name;
            public SpriteLabel readyLabel;
            public int paletteIdx;
            [HideInInspector]
            public bool ready;

            public void Change(int delta, int skip)
            {
                do
                {
                    paletteIdx = (paletteIdx + delta) % preview.PaletteGroup.Palettes.Length;
                    if (paletteIdx < 0) paletteIdx = preview.PaletteGroup.Palettes.Length - 1;
                }
                while (paletteIdx == skip);

                var palette = preview.PaletteGroup.Palettes[paletteIdx];
                name.SetLabel(palette.Name, 50);
                preview.SetPalette(palette);
            }

            public void Confirm()
            {
                readyLabel.gameObject.SetActive(ready = true);
                character.SetPalette(character.PaletteGroup.Palettes[paletteIdx]);
            }

            public void Reset()
            {
                readyLabel.gameObject.SetActive(ready = false);
            }
        }

        [SerializeField]
        private Player Player1, Player2;

        [SerializeField]
        private AudioSource m_AudioSource;
        [SerializeField]
        private AudioClip m_SelectClip, m_NextClip;
        [SerializeField]
        private SpriteRenderer m_BlackFade;

        public UnityEvent OnReady;

        private bool m_Locked;

        private void Start()
        {
            Player1.input.InputMod = "1";
            Player2.input.InputMod = "2";
            Player1.Change(0, -1);
            Player2.Change(0, -1);
        }

        private void OnEnable()
        {
            StartCoroutine(Fade(1f, 0f, false));
            Player1.Reset();
            Player2.Reset();
            m_Locked = false;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                Application.Quit();
        }

        public void Player1InputChange(INPUTFLAG input)
        {
            PlayerInputChange(Player1, input, Player2);
        }

        public void Player2InputChange(INPUTFLAG input)
        {
            PlayerInputChange(Player2, input, Player1);
        }

        private void PlayerInputChange(Player player, INPUTFLAG input, Player other)
        {

            if (m_Locked) return;

            switch (input)
            {
                case INPUTFLAG.LEFT:
                    if(!player.ready)
                    {
                        m_AudioSource.Stop();
                        m_AudioSource.clip = m_NextClip;
                        player.Change(-1, other.paletteIdx);
                    }
                    break;
                case INPUTFLAG.RIGHT:
                    if (!player.ready)
                    {
                        m_AudioSource.Stop();
                        m_AudioSource.clip = m_NextClip;
                        player.Change(1, other.paletteIdx);
                    }
                    break;
                case INPUTFLAG.LIGHT:
                    if (player.ready)
                    {
                        m_AudioSource.Stop();
                        m_AudioSource.clip = m_NextClip;
                        player.Reset();
                    }
                    break;
                case INPUTFLAG.HEAVY:
                    if (!player.ready)
                    {
                        m_AudioSource.Stop();
                        m_AudioSource.clip = m_SelectClip;
                        player.Confirm();
                        if (other.ready)
                        {
                            m_Locked = true;
                            StartCoroutine(Fade(0f, 1f, true));
                        }
                    }
                    break;
                default:
                    return;
            }

            m_AudioSource.Play();
        }

        private IEnumerator Fade(float start, float end, bool ready)
        {
            m_BlackFade.color = new Color(0f, 0f, 0f, start);
            yield return false;
            yield return false;
            yield return false;
            yield return false;
            m_BlackFade.color = new Color(0f, 0f, 0f, 0.5f);
            yield return false;
            yield return false;
            yield return false;
            yield return false;
            m_BlackFade.color = new Color(0f, 0f, 0f, end);
            yield return false;
            if(ready)
                OnReady.Invoke();
        }
    }
}