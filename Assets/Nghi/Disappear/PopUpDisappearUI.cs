using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpDisappearUI : MonoBehaviour
{
    public enum AnimationType
    {
        None,
        FadeIn, // Thực chất là FadeOut (do đang dùng cho Disappear)
        ScaleAndFade,
        MoveAndFade,
        Pop,
        ZoomIn,
        BounceIn,
        Blink,
        Swing,
        DropBounce,
        FromBackZoom,
        FadeSlide,
        EaseBackIn,
        SmoothScaleFade,
        CenterReveal,
    }

    public enum MoveDirection
    {
        None,
        FromLeft,
        FromRight,
        FromTop,
        FromBottom
    }

    [System.Serializable]
    public class UIElement
    {
        public GameObject target;
        public float delay = 0.2f;
        public float duration = 0.4f;
        public AnimationType animation = AnimationType.MoveAndFade;
        public MoveDirection moveDirection = MoveDirection.FromBottom;
        public float moveDistance = 100f;
        public float fromAlpha = 0f; // có thể xóa nếu không dùng

        [Header("Delay đến khi hiện UI tiếp theo")]
        public float delayBeforeNextShow = 0.2f; // Thêm delay này
    }

    [Header("Cấu hình UI biến mất")]
    public List<UIElement> disappearSequence = new List<UIElement>();
    public bool playOnStart = true;

    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        if (playOnStart)
            StartCoroutine(DelayedPlay());
    }

    IEnumerator DelayedPlay()
    {
        yield return null;
        yield return PlaySequence();
    }

    public IEnumerator PlaySequence()
    {
        foreach (var item in disappearSequence)
        {
            if (item.target == null) continue;

            CacheOriginals(item);

            yield return new WaitForSeconds(item.delay);

            var seq = DOTween.Sequence();
            PlayHideAnimation(item, seq);

            seq.AppendCallback(() =>
            {
                ResetToOriginal(item); // Reset trước khi tắt để tránh bị tắt rồi mới reset
                item.target.SetActive(false);
            });

            yield return seq.WaitForCompletion();
            //!!!!!!!!!!!!!!!
            // Thêm delay tùy chỉnh cho mỗi UI element
            yield return new WaitForSeconds(item.delayBeforeNextShow);
        }
    }

    void CacheOriginals(UIElement item)
    {
        var go = item.target;
        var t = go.transform;

        if (!originalPositions.ContainsKey(go))
            originalPositions[go] = t.localPosition;

        if (!originalScales.ContainsKey(go))
            originalScales[go] = t.localScale;
    }

    void ResetToOriginal(UIElement item)
    {
        var go = item.target;
        var t = go.transform;

        if (originalPositions.TryGetValue(go, out var pos))
            t.localPosition = pos;

        if (originalScales.TryGetValue(go, out var scale))
            t.localScale = scale;

        var cg = go.GetComponent<CanvasGroup>();
        if (cg != null)
            cg.alpha = 1f;

        DOTween.Kill(go);
        LeanTween.cancel(go);
    }

    void PlayHideAnimation(UIElement item, Sequence seq)
    {
        var t = item.target.transform;
        var go = item.target;
        var cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();

        DOTween.Kill(go);
        LeanTween.cancel(go);

        Vector3 offset = GetOffset(item);
        Vector3 startPos = originalPositions[go];

        switch (item.animation)
        {
            case AnimationType.MoveAndFade:
                seq.Append(t.DOLocalMove(startPos + offset, item.duration).SetEase(Ease.InCubic));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InQuad));
                break;

            case AnimationType.FadeIn: // thực chất là FadeOut
            case AnimationType.Blink:
                seq.Append(cg.DOFade(0f, item.duration).SetEase(Ease.OutSine));
                break;

            case AnimationType.ScaleAndFade:
            case AnimationType.Pop:
            case AnimationType.ZoomIn:
            case AnimationType.BounceIn:
            case AnimationType.SmoothScaleFade:
                LeanTween.scale(go, Vector3.zero, item.duration).setEaseInBack();
                seq.Append(cg.DOFade(0f, item.duration).SetEase(Ease.InOutSine));
                break;

            case AnimationType.Swing:
                seq.Append(t.DOLocalRotate(new Vector3(0, 0, 30), item.duration * 0.5f, RotateMode.Fast).SetEase(Ease.InBack));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InQuad));
                break;

            case AnimationType.DropBounce:
                seq.Append(t.DOLocalMoveY(startPos.y - item.moveDistance, item.duration).SetEase(Ease.InBack));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InQuad));
                break;

            case AnimationType.FromBackZoom:
                seq.Append(t.DOLocalMove(startPos + offset * 2, item.duration).SetEase(Ease.InBack));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InQuad));
                break;

            case AnimationType.FadeSlide:
                seq.Append(t.DOLocalMove(startPos + offset, item.duration).SetEase(Ease.InOutSine));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InOutSine));
                break;

            case AnimationType.EaseBackIn:
                seq.Append(t.DOLocalMove(startPos + offset, item.duration).SetEase(Ease.InBack));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InOutCubic));
                break;

            case AnimationType.CenterReveal:
                LeanTween.scaleX(go, 0f, item.duration).setEaseInBack();
                seq.Append(cg.DOFade(0f, item.duration).SetEase(Ease.OutSine));
                break;

            default:
                seq.Append(cg.DOFade(0f, item.duration).SetEase(Ease.OutQuad));
                break;
        }
    }

    Vector3 GetOffset(UIElement item)
    {
        switch (item.moveDirection)
        {
            case MoveDirection.FromLeft: return new Vector3(-item.moveDistance, 0, 0);
            case MoveDirection.FromRight: return new Vector3(item.moveDistance, 0, 0);
            case MoveDirection.FromTop: return new Vector3(0, item.moveDistance, 0);
            case MoveDirection.FromBottom: return new Vector3(0, -item.moveDistance, 0);
            default: return Vector3.zero;
        }
    }
}
