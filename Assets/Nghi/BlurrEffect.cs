using UnityEngine;

public class BlurrEffect : MonoBehaviour
{
    public Material blurMaterial;
    private float targetBlur = 1.5f;
    private float blurSpeed = 3.0f;

    void Start()
    {
        blurMaterial.SetFloat("_BlurSize", 0f); // Bắt đầu với blur 0
    }

    void Update()
    {
        float currentBlur = blurMaterial.GetFloat("_BlurSize");
        float newBlur = Mathf.Lerp(currentBlur, targetBlur, Time.deltaTime * blurSpeed);
        blurMaterial.SetFloat("_BlurSize", newBlur);
    }

    public void ShowBlur()
    {
        targetBlur = 2.5f; // Độ mờ khi mở panel
    }

    public void HideBlur()
    {
        targetBlur = 0f; // Khi tắt panel, giảm mờ dần
    }
}
