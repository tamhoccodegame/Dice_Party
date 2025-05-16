using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GradientButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References (gán vào Inspector)")]
    public RawImage background;          // RawImage chứa shader gradient
    public Image outlineGlow;            // Outline glow hiệu ứng ánh sáng
    public TextMeshProUGUI label;

    [Header("Gradient Colors")]
    public Color colorA = new Color(0.2f, 0.6f, 1f);
    public Color colorB = new Color(1f, 0.2f, 0.6f);

    [Header("Settings")]
    public float gradientSpeed = 0.5f;
    public float outlineFadeTime = 0.25f;
    public float returnSpeedMultiplier = 0.5f; // Thời gian trở về tỷ lệ với offset hiện tại

    // Internal
    private Material matInstance;
    private float uvOffset = 0f;
    private bool isHover = false;
    private CanvasGroup glowGroup;
    private Tween offsetTween;

    void Awake()
    {
        if (background == null || outlineGlow == null || label == null)
        {
            Debug.LogError("[GradientButtonEffect] Missing references.");
            enabled = false;
            return;
        }

        Shader sh = Shader.Find("Unlit/GradientUnlitShader");
        if (sh == null)
        {
            Debug.LogError("[GradientButtonEffect] Shader not found.");
            enabled = false;
            return;
        }

        matInstance = new Material(sh);
        matInstance.SetColor("_ColorA", colorA);
        matInstance.SetColor("_ColorB", colorB);
        matInstance.SetFloat("_Offset", 0f);
        background.material = matInstance;

        glowGroup = outlineGlow.GetComponent<CanvasGroup>();
        if (glowGroup == null)
            glowGroup = outlineGlow.gameObject.AddComponent<CanvasGroup>();
        glowGroup.alpha = 0f;
        outlineGlow.raycastTarget = false;
    }

    void Update()
    {
        if (isHover)
        {
            uvOffset += Time.deltaTime * gradientSpeed;
            matInstance.SetFloat("_Offset", uvOffset);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;

        // Dừng tween nếu đang chạy
        offsetTween?.Kill();

        // Glow in
        glowGroup.DOFade(1f, outlineFadeTime).SetEase(Ease.OutQuad);

        // Bounce effect
        transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 4, 1f)
                 .SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;

        // Glow out
        glowGroup.DOFade(0f, outlineFadeTime).SetEase(Ease.OutQuad);

        // Tween trở về 0 từ từ tùy theo khoảng cách
        float distance = Mathf.Abs(uvOffset);
        float duration = Mathf.Clamp(distance * returnSpeedMultiplier, 0.2f, 3f); // max 3s cho đẹp

        offsetTween = DOTween.To(() => uvOffset, x =>
        {
            uvOffset = x;
            matInstance.SetFloat("_Offset", uvOffset);
        }, 0f, duration).SetEase(Ease.OutCubic);
    }
    //[Header("UI References (gán vào Inspector)")]
    //public RawImage background;          // Bắt buộc RawImage
    //public Image outlineGlow;            // Image dùng để glow (canvasGroup điều khiển alpha)
    //public TextMeshProUGUI label;        // Text của nút

    //[Header("Gradient Colors")]
    //public Color colorA = new Color(0.2f, 0.6f, 1f);
    //public Color colorB = new Color(1f, 0.2f, 0.6f);

    //[Header("Settings")]
    //public float gradientSpeed = 0.5f;
    //public float outlineFadeTime = 0.25f;

    //// Internal
    //private Material matInstance;
    //private float uvOffset = 0f;
    //private bool isHover = false;
    //private CanvasGroup glowGroup;

    //void Awake()
    //{
    //    // Validate refs
    //    if (background == null || outlineGlow == null || label == null)
    //    {
    //        Debug.LogError("[AnimatedGradientButton] Thiếu reference trong Inspector!");
    //        enabled = false;
    //        return;
    //    }

    //    // Clone material từ shader, gán vào background
    //    Shader sh = Shader.Find("Unlit/GradientUnlitShader");
    //    if (sh == null)
    //    {
    //        Debug.LogError("[AnimatedGradientButton] Không tìm thấy shader Unlit/GradientScroll");
    //        enabled = false;
    //        return;
    //    }
    //    matInstance = new Material(sh);
    //    matInstance.SetColor("_ColorA", colorA);
    //    matInstance.SetColor("_ColorB", colorB);
    //    matInstance.SetFloat("_Offset", 0f);
    //    background.material = matInstance;

    //    // Chuẩn bị CanvasGroup cho outline
    //    glowGroup = outlineGlow.GetComponent<CanvasGroup>();
    //    if (glowGroup == null)
    //        glowGroup = outlineGlow.gameObject.AddComponent<CanvasGroup>();
    //    glowGroup.alpha = 0f;

    //    // Ensure outlineGlow không block raycast
    //    outlineGlow.raycastTarget = false;
    //}

    //void Update()
    //{
    //    // Chỉ update offset khi hover
    //    if (isHover)
    //    {
    //        uvOffset += Time.deltaTime * gradientSpeed;
    //        matInstance.SetFloat("_Offset", uvOffset);
    //    }
    //}

    //// Khi chuột vào
    //public void OnPointerEnter(PointerEventData evt)
    //{
    //    isHover = true;
    //    // Fade in glow
    //    glowGroup.DOFade(1f, outlineFadeTime).SetEase(Ease.OutQuad);
    //    // Optional: bounce effect
    //    transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 4, 1f)
    //             .SetEase(Ease.OutBack);
    //}

    //// Khi chuột rời
    //public void OnPointerExit(PointerEventData evt)
    //{
    //    isHover = false;
    //    // Fade out glow
    //    glowGroup.DOFade(0f, outlineFadeTime).SetEase(Ease.OutQuad);
    //}
}
