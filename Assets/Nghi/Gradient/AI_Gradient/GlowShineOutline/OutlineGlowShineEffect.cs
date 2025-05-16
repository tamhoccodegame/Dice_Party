using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class OutlineGlowShineEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private float duration = 2f;
    [SerializeField] private bool autoPlayOnHover = true;

    private Graphic graphic;
    private float shineValue = 0f;
    private Material runtimeMat;

    void Awake()
    {
        graphic = GetComponent<Graphic>();
        if (outlineMaterial != null && Application.isPlaying)
        {
            runtimeMat = Instantiate(outlineMaterial); // Clone runtime material để không ảnh hưởng asset
            graphic.material = runtimeMat;
            runtimeMat.SetFloat("_ShinePosition", -1f); // Tắt shine lúc đầu
        }
    }

    public void PlayShine()
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 0f, 1f, duration)
            .setLoopClamp()
            .setOnUpdate((float val) =>
            {
                shineValue = val;
                if (runtimeMat != null)
                    runtimeMat.SetFloat("_ShinePosition", shineValue);
            });
    }

    public void StopShine()
    {
        LeanTween.cancel(gameObject);
        if (runtimeMat != null)
            runtimeMat.SetFloat("_ShinePosition", -1f); // Reset
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (autoPlayOnHover) PlayShine();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (autoPlayOnHover) StopShine();
    }
}
