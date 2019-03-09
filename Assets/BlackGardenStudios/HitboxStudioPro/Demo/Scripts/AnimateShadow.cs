using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
    public class AnimateShadow : MonoBehaviour
    {
        public Animator ShadowAnimator;
        public SpriteRenderer ShadowRenderer, CharacterRenderer;
        public Transform FollowTarget;
        public bool X, Y;
        private int paramID = Animator.StringToHash("time");

        void LateUpdate()
        {
            var target = FollowTarget.position;
            var position = transform.position;

            if (X)
                position.x = target.x;

            if (Y)
                position.y = target.y;

            transform.position = position;
            ShadowAnimator.SetFloat(paramID, Mathf.Clamp01((target.y - position.y) / 7f));
            ShadowRenderer.flipX = CharacterRenderer.flipX;
        }
    }
}
