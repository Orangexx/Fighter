using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Utils
{
    public static class AnimationUtils
    {
        public static AnimationClip[] GetAnimationClips(Animator animator)
        {
            return animator.runtimeAnimatorController.animationClips;
        }

        public static Sprite[] GetClipSprites(AnimationClip animationClip)
        {
            var sprites = new Sprite[0];

            if (animationClip != null)
            {
                var editorCurveBinding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
                var objectReferenceKeyframes = AnimationUtility.GetObjectReferenceCurve(animationClip,
                    editorCurveBinding);
                if (objectReferenceKeyframes != null)
                {
                    sprites = objectReferenceKeyframes
                        .Select(objectReferenceKeyframe => objectReferenceKeyframe.value)
                        .OfType<Sprite>().ToArray();
                }
            }
            return sprites;
        }

        public static List<float> GetClipFrameTimes(AnimationClip animationClip)
        {
            var curves = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);
            List<float> times = new List<float>();

            for (int i = 0; i < curves.Length; i++)
                if (curves[i].propertyName.Equals("m_Sprite"))
                {
                    var keyframes = AnimationUtility.GetObjectReferenceCurve(animationClip, curves[i]);

                    for (int j = 0; j < keyframes.Length && j < curves.Length; j++)
                        times.Add(keyframes[j].time);
                }

            return times;
        }
    }

    public static class CommonUtils
    {
        public static void DesAllGO(List<GameObject> list)
        {
            for (int i = list.Count - 1; i > -1; i--)
            {
                Object.Destroy(list[i]);
            }

            list.Clear();
            list = null;
        }
    }
}

