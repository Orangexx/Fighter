using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
    public class RoundResult : MonoBehaviour
    {
        public Image Image;
        public Color StartColor;
        public Color ActiveColor;


        void OnEnable()
        {
            SetColor(StartColor);
        }

        public void Activate()
        {
            SetColor(ActiveColor);
        }

        public void SetColor(Color color)
        {
            Image.color = color;
        }
    }

}
