
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class UIAnimationItem
{
    public GameObject target;
    public float delay = 0.1f;
    public float duration = 0.5f;
    public AnimationType animationIn = AnimationType.Fade;
    public AnimationType animationOut = AnimationType.Fade;
    public Direction moveDirection = Direction.Center;
    public bool animateTMPText = true;
    public float tmpTextDelay = 0.1f;
}

public enum AnimationType
{
    None,
    Blink,
    Fade,
    Pop,
    Zoom,
    Move,
    Slide,
    Fly,
    Push,
    Reveal,
    Wipe,
    Drop,
    Bounce
}

public enum Direction
{
    Center,
    TopToBottom,
    BottomToTop,
    LeftToRight,
    RightToLeft
}

public class UltimateUIAnimator : MonoBehaviour
{
    public List<UIAnimationItem> items = new List<UIAnimationItem>();
    public bool playOnStart = true;

    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        CacheOriginalPositions();

        if (playOnStart)
            PlayInAnimations();
    }

    public void PlayInAnimations()
    {
        foreach (var item in items)
        {
            if (item.target == null) continue;

            PrepareItem(item, true);

            StartCoroutine(PlayAnimation(item, true));
        }
    }

    public void PlayOutAnimations()
    {
        foreach (var item in items)
        {
            if (item.target == null) continue;

            PrepareItem(item, false);

            StartCoroutine(PlayAnimation(item, false));
        }
    }

    private void CacheOriginalPositions()
    {
        foreach (var item in items)
        {
            if (item.target && !originalPositions.ContainsKey(item.target))
                originalPositions[item.target] = item.target.transform.localPosition;
        }
    }

    private void PrepareItem(UIAnimationItem item, bool isIn)
    {
        var cg = item.target.GetComponent<CanvasGroup>();
        if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
        cg.alpha = isIn ? 0f : 1f;

        var t = item.target.transform;
        t.localScale = Vector3.one;

        if (item.animationIn == AnimationType.Move || item.animationIn == AnimationType.Slide || item.animationIn == AnimationType.Fly)
        {
            Vector3 offset = GetOffset(item.moveDirection, 200f);
            if (isIn) t.localPosition = originalPositions[item.target] + offset;
        }
    }

    private IEnumerator PlayAnimation(UIAnimationItem item, bool isIn)
    {
        yield return new WaitForSeconds(item.delay);

        var cg = item.target.GetComponent<CanvasGroup>();
        var t = item.target.transform;

        AnimationType animType = isIn ? item.animationIn : item.animationOut;
        float endAlpha = isIn ? 1f : 0f;
        Vector3 endPos = originalPositions[item.target];
        float dur = item.duration;

        switch (animType)
        {
            case AnimationType.Fade:
                cg.DOFade(endAlpha, dur);
                break;
            case AnimationType.Pop:
                t.localScale = isIn ? Vector3.zero : Vector3.one;
                t.DOScale(isIn ? Vector3.one : Vector3.zero, dur).SetEase(Ease.OutBack);
                cg.DOFade(endAlpha, dur * 0.9f);
                break;
            case AnimationType.Move:
            case AnimationType.Slide:
            case AnimationType.Fly:
            case AnimationType.Push:
            case AnimationType.Reveal:
            case AnimationType.Wipe:
            case AnimationType.Drop:
            case AnimationType.Bounce:
                Vector3 offset = GetOffset(item.moveDirection, 200f);
                if (!isIn) endPos += offset;
                t.DOLocalMove(endPos, dur).SetEase(Ease.OutCubic);
                cg.DOFade(endAlpha, dur * 0.9f);
                break;
            case AnimationType.Zoom:
                t.localScale = isIn ? Vector3.zero : Vector3.one;
                t.DOScale(isIn ? Vector3.one : Vector3.zero, dur).SetEase(Ease.InOutSine);
                break;
        }

        if (item.animateTMPText)
        {
            yield return new WaitForSeconds(item.tmpTextDelay);
            var tmp = item.target.GetComponentInChildren<TMP_Text>();
            if (tmp)
            {
                tmp.DOFade(endAlpha, dur * 0.8f).SetEase(Ease.InOutSine);
            }
        }
    }

    private Vector3 GetOffset(Direction dir, float dist)
    {
        switch (dir)
        {
            case Direction.TopToBottom: return new Vector3(0, dist, 0);
            case Direction.BottomToTop: return new Vector3(0, -dist, 0);
            case Direction.LeftToRight: return new Vector3(-dist, 0, 0);
            case Direction.RightToLeft: return new Vector3(dist, 0, 0);
            default: return Vector3.zero;
        }
    }
}
