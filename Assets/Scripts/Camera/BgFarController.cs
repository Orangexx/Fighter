using UnityEngine;

public class BgFarController : MonoBehaviour
{
    public Material MatScroll;
    public float RatioMove1 = 0;

    private float lastX;
    private float offsetX;
    private int matPropID1;

    void Awake()
    {
        var target = GlobalManager.Instance.GetCharactorPos();
        lastX = target.x;
        offsetX = 0;
        matPropID1 = Shader.PropertyToID("_ScrollX");
    }
    void LateUpdate()
    {
        MatScroll.SetFloat(matPropID1, offsetX += (GlobalManager.Instance.GetCharactorPos().x - lastX) / RatioMove1);
        lastX = GlobalManager.Instance.GetCharactorPos().x;
    }
}