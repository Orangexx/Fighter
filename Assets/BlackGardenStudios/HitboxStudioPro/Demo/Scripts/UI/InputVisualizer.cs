using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
#pragma warning disable 0649
    public class InputVisualizer : MonoBehaviour
    {
        private enum UI_SIDE
        {
            LEFT = -1,
            RIGHT = 1
        }
        static readonly int MAX_INPUTS = 10;

        [Serializable]
        public struct VisualInput
        {
            public INPUTFLAG[] inputs;
            public Sprite sprite;
            public INPUTFLAG combinedInput
            {
                get
                {
                    if (inputs.Length == 1)
                        return inputs[0];
                    else
                    {
                        INPUTFLAG x = 0;

                        for (int i = 0; i < inputs.Length; i++)
                        {
                            x |= inputs[i];
                        }

                        return x;
                    }
                }
            }
        }

        [SerializeField]
        private UI_SIDE m_Orientation;
        public VisualInput[] InputSprites;
        public Image template;
        private LinkedList<Image> Images = new LinkedList<Image>();

        void Start()
        {
            GetComponentInParent<InputHistory>().Event.AddListener((INPUTFLAG input) =>
            {
            //We don't display the release of buttons
            if ((input & INPUTFLAG.NEGATIVE) > 0) return;

                for (int i = 0; i < InputSprites.Length; i++)
                {
                    if (InputSprites[i].inputs.Length < 2) continue;
                    if (input == InputSprites[i].combinedInput)
                    {
                        var image = Instantiate(template, transform);
                        image.sprite = InputSprites[i].sprite;
                        Images.AddFirst(image);
                        image.gameObject.SetActive(true);
                        RepositionSprites();
                    }
                }

                for (int i = 0; i < InputSprites.Length; i++)
                {
                    if (InputSprites[i].inputs.Length > 1) continue;

                    if (input == InputSprites[i].inputs[0])
                    {
                        var image = Instantiate(template, transform);
                        image.sprite = InputSprites[i].sprite;
                        Images.AddFirst(image);
                        image.gameObject.SetActive(true);
                        RepositionSprites();
                    }
                }
            });
        }

        void RepositionSprites()
        {
            while (Images.Count > MAX_INPUTS)
            {
                var image = Images.Last.Value;
                Destroy(image.gameObject);
                Images.RemoveLast();
            }

            var count = 0;
            foreach (var image in Images)
            {
                image.rectTransform.localPosition = 
                    new Vector3((950f * (int)m_Orientation) / 2f + (16f * -1f *(int)m_Orientation), 
                                (32f * count++) - 32f);
            }
        }
    }
}