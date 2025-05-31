using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpAppearUI_Extended : MonoBehaviour
{
    public enum AnimationType
    {
        None,
        FadeIn,
        ScaleAndFade,
        MoveAndFade,
        Pop,
        ZoomIn,
        BounceIn,
        SlideIn,
        FlyIn,
        Push,
        Reveal,
        Blink,
        Swing,
        DropBounce,
        FromBackZoom,
        FadeSlide,
        EaseBackIn,
        SmoothScaleFade,
        CenterReveal
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
        public float fromAlpha = 0f;
    }

    [Header("Cấu hình UI xuất hiện")]
    public List<UIElement> appearSequence = new List<UIElement>();
    public bool playOnStart = true;

    private void Start()
    {
        if (playOnStart)
        {
            HideAllItemsInstant();
            StartCoroutine(PlaySequence(appearSequence, true));
        }
    }

    public void PlayAppear()
    {
        HideAllItemsInstant();
        StartCoroutine(PlaySequence(appearSequence, true));
    }

    public void PlayDisappear(System.Action onComplete = null)
    {
        StartCoroutine(PlaySequence(appearSequence, false, onComplete));
    }

    IEnumerator PlaySequence(List<UIElement> sequence, bool appear, System.Action onComplete = null)
    {
        if (!appear)
            sequence.Reverse();

        foreach (var item in sequence)
        {
            if (item.target == null) continue;

            if (appear)
                PrepareItem(item);
            
            yield return new WaitForSeconds(item.delay);
            PlayAnimation(item, appear);
            yield return new WaitForSeconds(item.duration);
        }

        if (!appear)
            sequence.Reverse();

        onComplete?.Invoke();
    }

    void PrepareItem(UIElement item)
    {
        var t = item.target.transform;
        var cg = item.target.GetComponent<CanvasGroup>();
        if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
        cg.alpha = item.fromAlpha;
        Vector3 offset = GetOffset(item);

        if (IsMoveAnimation(item.animation) && item.moveDirection != MoveDirection.None)
            t.localPosition += offset;

        if (item.animation.ToString().Contains("Scale") ||
            item.animation == AnimationType.Pop ||
            item.animation == AnimationType.ZoomIn ||
            item.animation == AnimationType.BounceIn ||
            item.animation == AnimationType.CenterReveal ||
            item.animation == AnimationType.SmoothScaleFade)
            t.localScale = Vector3.zero;

        item.target.SetActive(true);
    }

    void PlayAnimation(UIElement item, bool appear)
    {
        var t = item.target.transform;
        var cg = item.target.GetComponent<CanvasGroup>();
        float targetAlpha = appear ? 1f : 0f;
        Vector3 moveOffset = GetOffset(item);

        switch (item.animation)
        {
            case AnimationType.FadeIn:
                cg.DOFade(targetAlpha, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.ScaleAndFade:
                LeanTween.scale(t.gameObject, appear ? Vector3.one : Vector3.zero, item.duration).setEaseOutBack();
                cg.DOFade(targetAlpha, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.MoveAndFade:
                if (appear)
                    t.DOLocalMove(t.localPosition - moveOffset, item.duration).SetEase(Ease.OutCubic);
                else
                    t.DOLocalMove(t.localPosition + moveOffset, item.duration).SetEase(Ease.InCubic);
                cg.DOFade(targetAlpha, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.FadeSlide:
                if (appear)
                    t.DOLocalMove(t.localPosition - moveOffset, item.duration).SetEase(Ease.InOutSine);
                else
                    t.DOLocalMove(t.localPosition + moveOffset, item.duration).SetEase(Ease.InOutSine);
                cg.DOFade(targetAlpha, item.duration).SetEase(Ease.InOutSine);
                break;

            case AnimationType.SmoothScaleFade:
                LeanTween.scale(t.gameObject, appear ? Vector3.one : Vector3.zero, item.duration).setEaseOutQuad();
                cg.DOFade(targetAlpha, item.duration).SetEase(Ease.InOutSine);
                break;

            case AnimationType.CenterReveal:
                LeanTween.scaleX(t.gameObject, appear ? 1f : 0f, item.duration).setEaseOutCubic();
                cg.DOFade(targetAlpha, item.duration).SetEase(Ease.OutSine);
                break;

            default:
                cg.DOFade(targetAlpha, item.duration).SetEase(Ease.OutQuad);
                break;
        }

        if (!appear)
            StartCoroutine(DisableAfter(item.target, item.duration));
    }

    IEnumerator DisableAfter(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
    }

    bool IsMoveAnimation(AnimationType anim)
    {
        return anim == AnimationType.MoveAndFade ||
               anim == AnimationType.SlideIn ||
               anim == AnimationType.FlyIn ||
               anim == AnimationType.Push ||
               anim == AnimationType.Reveal ||
               anim == AnimationType.FadeSlide ||
               anim == AnimationType.EaseBackIn ||
               anim == AnimationType.FromBackZoom ||
               anim == AnimationType.DropBounce;
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

    void HideAllItemsInstant()
    {
        foreach (var item in appearSequence)
        {
            if (item.target != null)
            {
                var cg = item.target.GetComponent<CanvasGroup>();
                if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
                cg.alpha = 0f;
                item.target.SetActive(true);
            }
        }
    }
}