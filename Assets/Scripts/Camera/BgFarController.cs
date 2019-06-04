using UnityEngine;

namespace Fighter
{
    public class BgFarController : MonoBehaviour
    {
        public Material MatScroll;
        public float RatioMove1 = 0;

        private float lastX;
        private float offsetX;
        private int matPropID1;

        void Awake()
        {
            lastX = GlobalManager.Instance.MainCamera.transform.position.x;
            offsetX = 0;
            matPropID1 = Shader.PropertyToID("_ScrollX");
        }
        void LateUpdate()
        {
            if (GlobalManager.Instance.MainCamera == null)
                return;
            MatScroll.SetFloat(matPropID1, offsetX += (GlobalManager.Instance.MainCamera.transform.position.x - lastX) / RatioMove1);
            lastX = GlobalManager.Instance.MainCamera.transform.position.x;
        }
    }
}

