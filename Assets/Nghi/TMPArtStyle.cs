using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class TMPArtStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //***
    [Header("FILL COLOR SETTINGS")]
    public bool useGradientFill = true;
    public Color topColor = Color.white;
    public Color bottomColor = Color.gray;

    [Header("OUTLINE SETTINGS")]
    public bool useGradientOutline = true;
    public Color outlineColor1 = Color.red;
    public Color outlineColor2 = Color.blue;
    [Range(0f, 1f)] public float outlineThickness = 0.2f;
    public float outlineGradientSpeed = 2f;

    [Header("BLINK SETTINGS")]
    public bool blinkOnHover = true;
    public bool blinkOnAwake = false;  // NEW ✅
    public float blinkSpeed = 4f;
    public Color blinkColor = Color.yellow;

    private TMP_Text tmp;
    private Material tmpMaterialInstance;
    private bool isHovered = false;
    private bool isBlinking = false;
    private float blinkTimer = 0f;
    private float outlineLerpT = 0f;
    private bool blinkState = false;
    private Color originalColor;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();

        // Clone material riêng để không ảnh hưởng TMP khác
        tmpMaterialInstance = new Material(tmp.fontSharedMaterial);
        tmp.fontMaterial = tmpMaterialInstance;

        originalColor = topColor;

        ApplyStyle();

        // Nếu tick Blink On Awake
        if (blinkOnAwake) isBlinking = true;
    }

    void OnValidate()
    {
        if (tmp == null) tmp = GetComponent<TMP_Text>();
        if (tmpMaterialInstance == null && tmp != null)
        {
            tmpMaterialInstance = new Material(tmp.fontSharedMaterial);
            tmp.fontMaterial = tmpMaterialInstance;
        }
        ApplyStyle();
    }

    void Update()
    {
        // Outline Gradient
        if (useGradientOutline)
        {
            outlineLerpT += Time.deltaTime * outlineGradientSpeed;
            Color lerpedOutline = Color.Lerp(outlineColor1, outlineColor2, (Mathf.Sin(outlineLerpT) + 1f) / 2f);
            tmpMaterialInstance.SetColor(ShaderUtilities.ID_OutlineColor, lerpedOutline);
        }

        // Blink
        if (isBlinking)
        {
            blinkTimer += Time.deltaTime * blinkSpeed;
            blinkState = Mathf.PingPong(blinkTimer, 1f) > 0.5f;
            tmp.color = blinkState ? blinkColor : topColor;
        }
        else
        {
            tmp.color = topColor;
        }
    }


    void ApplyStyle()
    {
        // Fill Gradient
        if (useGradientFill)
        {
            tmp.enableVertexGradient = true;
            tmp.colorGradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);
        }
        else
        {
            tmp.enableVertexGradient = false;
            tmp.color = topColor;
        }

        // Outline
        tmpMaterialInstance.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineThickness);
        tmpMaterialInstance.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (blinkOnHover) isBlinking = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (blinkOnHover) isBlinking = false;
        tmp.color = topColor;
    }
}
