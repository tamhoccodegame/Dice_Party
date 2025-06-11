using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpAppearOnly_UI : MonoBehaviour
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
        public float fromAlpha = 0f;
    }

    [Header("Cấu hình UI xuất hiện")]
    public List<UIElement> appearSequence = new List<UIElement>();
    public bool playOnStart = true;

    void Start()
    {
        if (playOnStart)
        {
            HideAllItemsInstant();
            StartCoroutine(DelayedPlay());
        }
    }


    IEnumerator DelayedPlay()
    {
        yield return null; // 🔥 Delay 1 frame để LayoutGroup chạy xong!
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        foreach (var item in appearSequence)
        {
            if (item.target == null) continue;

            PrepareItem(item);
            yield return new WaitForSeconds(item.delay);
            PlayAnimation(item);
            yield return new WaitForSeconds(item.duration);
        }
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

    void PrepareItem(UIElement item)
    {
        var t = item.target.transform;

        var cg = item.target.GetComponent<CanvasGroup>();
        if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
        cg.alpha = item.fromAlpha;

        Vector3 offset = GetOffset(item);

        // Di chuyển nếu có hiệu ứng di chuyển
        if (IsMoveAnimation(item.animation) && item.moveDirection != MoveDirection.None)
        {
            t.localPosition += offset;
        }

        // Scale về 0 cho các animation cần scale
        if (item.animation.ToString().Contains("Scale") ||
            item.animation == AnimationType.Pop ||
            item.animation == AnimationType.ZoomIn ||
            item.animation == AnimationType.BounceIn ||
            item.animation == AnimationType.CenterReveal ||
            item.animation == AnimationType.SmoothScaleFade)
        {
            t.localScale = Vector3.zero;
        }

        item.target.SetActive(true);
    }

    void PlayAnimation(UIElement item)
    {
        var t = item.target.transform;
        var cg = item.target.GetComponent<CanvasGroup>();

        switch (item.animation)
        {
            case AnimationType.FadeIn:
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            //case AnimationType.Scale:
            //    LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutBack();
            //    break;

            case AnimationType.ScaleAndFade:
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutBack();
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            //case AnimationType.MoveFromDirection:
            //    t.DOLocalMove(t.localPosition - GetOffset(item), item.duration).SetEase(Ease.OutCubic);
            //    break;

            case AnimationType.MoveAndFade:
                t.DOLocalMove(t.localPosition - GetOffset(item), item.duration).SetEase(Ease.OutCubic);
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.Pop:
                t.localScale = Vector3.zero;
                LeanTween.scale(t.gameObject, Vector3.one * 1.1f, item.duration * 0.6f).setEaseOutBack()
                    .setOnComplete(() => {
                        LeanTween.scale(t.gameObject, Vector3.one, item.duration * 0.4f).setEaseInOutCubic();
                    });
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.ZoomIn:
                t.localScale = Vector3.zero;
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutExpo();
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.BounceIn:
                t.localScale = Vector3.zero;
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutBounce();
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.FlyIn:
            case AnimationType.SlideIn:
            case AnimationType.Push:
            case AnimationType.Reveal:
                // Dùng giống MoveAndFade
                t.DOLocalMove(t.localPosition - GetOffset(item), item.duration).SetEase(Ease.OutBack);
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.Blink:
                cg.DOFade(1f, item.duration * 0.2f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(4, LoopType.Yoyo)
                    .OnComplete(() => cg.alpha = 1f);
                break;

            case AnimationType.Swing:
                t.localRotation = Quaternion.Euler(0, 0, 30);
                t.DOLocalRotate(Vector3.zero, item.duration, RotateMode.Fast).SetEase(Ease.OutElastic);
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.DropBounce:
                t.localPosition += new Vector3(0, item.moveDistance, 0);
                t.DOLocalMoveY(t.localPosition.y - item.moveDistance, item.duration)
                    .SetEase(Ease.OutBounce);
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.FromBackZoom:
                t.localPosition += GetOffset(item) * 2;
                t.DOLocalMove(t.localPosition - GetOffset(item) * 2, item.duration)
                    .SetEase(Ease.InOutBack);
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;


            case AnimationType.FadeSlide:
                t.DOLocalMove(t.localPosition - GetOffset(item), item.duration).SetEase(Ease.InOutSine);
                cg.DOFade(1f, item.duration).SetEase(Ease.InOutSine);
                break;

            case AnimationType.EaseBackIn:
                t.DOLocalMove(t.localPosition - GetOffset(item), item.duration).SetEase(Ease.InOutBack);
                cg.DOFade(1f, item.duration).SetEase(Ease.InOutCubic);
                break;

            case AnimationType.SmoothScaleFade:
                t.localScale = Vector3.zero;
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutQuad();
                cg.DOFade(1f, item.duration).SetEase(Ease.InOutSine);
                break;

            case AnimationType.CenterReveal:
                t.localScale = new Vector3(0f, 1f, 1f);
                LeanTween.scaleX(t.gameObject, 1f, item.duration).setEaseOutCubic();
                cg.DOFade(1f, item.duration * 0.8f).SetEase(Ease.OutSine);
                break;

            default:
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;
        }
    }


    Vector3 GetOffset(UIElement item)
    {
        if (item.moveDirection == MoveDirection.None)
            return Vector3.zero;

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
                item.target.SetActive(true); // Phải bật mới set alpha được
            }
        }
    }
}
