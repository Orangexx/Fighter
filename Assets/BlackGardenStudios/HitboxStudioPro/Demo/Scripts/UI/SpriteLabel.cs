using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
    public class SpriteLabel : MonoBehaviour
    {
        public Sprite[] SpriteAlphabet;
        public string Label;
        public Color Color = Color.white;
        public bool OffsetByLength;

        private Vector3 m_Offset = Vector3.zero;

        public void SetLabel(string label, int order = 10)
        {
            DestroyLabel();

            var chars = label.ToUpper().ToCharArray();
            var scale = transform.localScale;

            if (OffsetByLength)
                transform.position += m_Offset;

            for (int i = 0; i < chars.Length; i++)
            {
                var charObject = new GameObject();
                var charRenderer = charObject.AddComponent<SpriteRenderer>();
                var charTransform = charObject.transform;

                charTransform.SetParent(transform);
                charTransform.localPosition = new Vector3(0.45f * i * scale.x, 0f);
                charTransform.localScale = scale;
                charRenderer.sortingOrder = order;
                charRenderer.sprite = SpriteAlphabet[chars[i] - 'A'];
                charRenderer.color = Color;
            }

            Label = label;

            if (OffsetByLength)
                transform.position -= m_Offset = new Vector3(0.45f * (chars.Length - 1) * scale.x, 0f);
        }

        void OnEnable()
        {
            SetLabel(Label);
        }

        void DestroyLabel()
        {
            int childCount = transform.childCount;

            for (int i = 0; i < childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}
