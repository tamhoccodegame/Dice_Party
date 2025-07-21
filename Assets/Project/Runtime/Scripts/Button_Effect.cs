using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_Effect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //public TMP_Text textMeshPro;
    //public Material textMaterial;
    public RectTransform buttonTransform;

    public float fadeSpeed = 2f;
    public float blinkSpeed = 1.5f;
    public float blinkDuration = 1f;
    public float moveDistance = 10f; // Khoảng cách di chuyển sang phải
    public float moveSpeed = 0.2f;   // Thời gian di chuyển

    private bool isBlinking = false;
    private bool isFading = false;
    private Coroutine fadeCoroutine;
    private Coroutine blinkCoroutine;
    private Vector2 originalPosition;

    //!!!
    [SerializeField] private Color hoverOutlineColor = Color.white;
    private List<Outline> outlines = new List<Outline>();
    private Dictionary<Outline, Color> originalColors = new Dictionary<Outline, Color>();

    private void Awake()
    {
        // Tự tìm tất cả Outline trong các con của nút
        outlines.AddRange(GetComponentsInChildren<Outline>());

        // Lưu màu gốc
        foreach (var outline in outlines)
        {
            originalColors[outline] = outline.effectColor;
        }
    }

    void Start()
    {
        //if (textMeshPro == null)
        //    textMeshPro = GetComponent<TMP_Text>();

        if (buttonTransform == null)
            buttonTransform = GetComponent<RectTransform>();

        // Lưu vị trí ban đầu của nút
        //originalPosition = buttonTransform.anchoredPosition;

        // Lấy Instance Material để chỉnh riêng cho chữ này
        //textMaterial = textMeshPro.fontMaterial;
        //textMaterial = new Material(textMaterial); // Tạo bản sao
        //textMeshPro.fontMaterial = textMaterial;
        //textMaterial.SetColor("_FaceColor", Color.black);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (!isFading)
        //{
        //    isFading = true;
        //    fadeCoroutine = StartCoroutine(FadeInEffect());
        //}

        //// Dịch nút sang phải một chút
        //LeanTween.moveX(buttonTransform, originalPosition.x + moveDistance, moveSpeed).setEaseOutQuad();

        //// Bật hiệu ứng sáng viền
        //textMaterial.SetColor("_FaceColor", Color.black);
        //textMaterial.SetColor("_OutlineColor", Color.white);
        //textMaterial.SetFloat("_OutlineWidth", 0.1f);

        if (isFading) return;
        isFading = true;

        // 👉 Ghi lại vị trí gốc đúng thời điểm chuột hover (nếu trước đó bị lệch thì vẫn chuẩn)
        originalPosition = buttonTransform.anchoredPosition;

        // 👉 Dịch sang phải từ vị trí gốc hiện tại
        LeanTween.cancel(gameObject);
        //LeanTween.moveX(buttonTransform, originalPosition.x + moveDistance, moveSpeed).setEaseOutExpo();
        LeanTween.scale(buttonTransform, Vector3.one * 1.05f, moveSpeed).setEaseOutExpo();
        // Các hiệu ứng Fade + Line như cũ
        //textMaterial.SetColor("_FaceColor", Color.black);
        //textMaterial.SetColor("_OutlineColor", Color.white);
        //textMaterial.SetFloat("_OutlineWidth", 0.1f);

        fadeCoroutine = StartCoroutine(FadeInEffect());

        foreach (var outline in outlines)
        {
            LeanTween.value(gameObject, outline.effectColor, hoverOutlineColor, 0.15f)
                .setOnUpdate((Color col) =>
                {
                    outline.effectColor = col;
                });
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);

        isFading = false;
        isBlinking = false;

        // Quay về vị trí ban đầu
        //LeanTween.moveX(buttonTransform, originalPosition.x, moveSpeed).setEaseOutQuad();
        LeanTween.scale(buttonTransform, Vector3.one, moveSpeed).setEaseOutQuad();
        // Reset hiệu ứng chữ
        //textMaterial.SetFloat("_FadeProgress", 1);
        //textMaterial.SetFloat("_LineWidth", 0);
        //textMeshPro.color = Color.black;
        foreach (var outline in outlines)
        {
            Color original = originalColors[outline];
            LeanTween.value(gameObject, outline.effectColor, original, 0.15f)
                .setOnUpdate((Color col) =>
                {
                    outline.effectColor = col;
                });
        }
    }

    private IEnumerator FadeInEffect()
    {
        float progress = 0;
        float lineWidth = 0.05f;

        while (progress < 1)
        {
            progress += Time.deltaTime * fadeSpeed;
            //textMaterial.SetFloat("_FadeProgress", progress);
            //textMaterial.SetFloat("_LineWidth", lineWidth);
            yield return null;
        }

        //textMaterial.SetFloat("_FadeProgress", 1);
        //textMaterial.SetFloat("_LineWidth", 0);

        // Bắt đầu Blink
        isBlinking = true;
        blinkCoroutine = StartCoroutine(BlinkEffect());
    }

    private IEnumerator BlinkEffect()
    {
        while (isBlinking)
        {
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1);
            //textMaterial.SetColor("_FaceColor", new Color(0, 0, 0, alpha)); // Nhấp nháy màu đen
            yield return null;
        }

        // Reset về trạng thái ban đầu
        //textMaterial.SetColor("_FaceColor", Color.black);
    }
}
