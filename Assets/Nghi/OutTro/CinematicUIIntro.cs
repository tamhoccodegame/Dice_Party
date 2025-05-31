
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CinematicUIIntro : MonoBehaviour
{
    [Header("General Settings")]
    public bool playOnStart = true;

    [Header("Entry Movement")]
    public float startZOffset = -300f;
    public float moveToOriginalDuration = 1.2f;
    public Ease moveEase = Ease.OutCubic;

    [Header("Stay In Center")]
    public float stayDuration = 2f;

    [Header("Exit Movement")]
    public Vector2 slideOffset = new Vector2(-200f, 0f); // Nép qua trái
    public float slideDuration = 0.8f;
    public Ease slideEase = Ease.InOutCubic;

    [Header("Fade Settings")]
    public bool fadeOnEntry = true;
    public float fadeDuration = 1f;

    private CanvasGroup canvasGroup;
    private Vector3 originalLocalPos;

    void Awake()
    {
        if (!Application.isPlaying) return;

        // Lưu lại vị trí ban đầu
        originalLocalPos = transform.localPosition;

        if (playOnStart)
        {
            RunEffect();
        }
    }

    public void RunEffect()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0;

        // Đặt vị trí gần ống kính camera (trục Z lùi gần)
        Vector3 startPos = originalLocalPos + new Vector3(0f, 0f, startZOffset);
        transform.localPosition = startPos;

        // Tween Move về vị trí gốc
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            if (fadeOnEntry)
            {
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1f, fadeDuration);
            }
        });

        seq.Append(transform.DOLocalMove(originalLocalPos, moveToOriginalDuration).SetEase(moveEase));
        seq.AppendInterval(stayDuration);
        seq.Append(transform.DOLocalMove(originalLocalPos + (Vector3)slideOffset, slideDuration).SetEase(slideEase));
    }
}
