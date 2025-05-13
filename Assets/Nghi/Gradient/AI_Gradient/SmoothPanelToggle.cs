using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SmoothPanelToggle : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 0.4f;
    public float scaleFrom = 0.95f; // Scale nhẹ cho animation đẹp
    public LeanTweenType tweenType = LeanTweenType.easeInOutExpo;

    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private bool isVisible = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;

        if (gameObject.activeSelf)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            isVisible = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isVisible = false;
        }
    }

    public void ToggleVisibility(bool show)
    {
        if (show)
            Show();
        else
            Hide();
    }

    public void Show()
    {
        if (isVisible) return;

        gameObject.SetActive(true); // Bắt buộc để visible trước tween

        canvasGroup.alpha = 0;
        transform.localScale = originalScale * scaleFrom;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Tween vào
        LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration).setEase(tweenType);
        LeanTween.scale(gameObject, originalScale, fadeDuration).setEase(tweenType);

        // Cho tương tác sau tween
        LeanTween.delayedCall(fadeDuration * 0.7f, () =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            isVisible = true;
        });
    }

    public void Hide()
    {
        if (!isVisible) return;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Tween out
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration).setEase(tweenType);
        LeanTween.scale(gameObject, originalScale * scaleFrom, fadeDuration).setEase(tweenType).setOnComplete(() =>
        {
            gameObject.SetActive(false);
            transform.localScale = originalScale; // Reset lại scale
        });

        isVisible = false;
    }

    public void HideInstant()
    {
        if (!isVisible) return;

        // Cancel mọi tween đang chạy
        LeanTween.cancel(gameObject);

        // Tắt tương tác UI
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Tắt GameObject
        gameObject.SetActive(false);

        isVisible = false;
    }
}
