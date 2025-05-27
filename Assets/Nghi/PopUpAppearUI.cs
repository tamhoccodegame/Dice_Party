using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpAppearUI : MonoBehaviour
{
    [System.Serializable]
    public class AppearItem
    {
        public GameObject target;
        public float delay = 0.2f;
        public AnimationType animation = AnimationType.Scale;
        public float duration = 0.3f;
        public Vector3 fromPositionOffset = new Vector3(0, -50f, 0);
        public float fromAlpha = 0f;
    }

    public enum AnimationType
    {
        Scale,
        MoveFromOffset,
        FadeIn,
        ScaleAndFade,
        MoveAndFade
    }

    [Header("Config")]
    public List<AppearItem> appearSequence = new List<AppearItem>();
    public bool playOnStart = true;

    void Start()
    {
        if (playOnStart)
        {
            HideAllItems();
            StartSequence();
        }
    }

    public void StartSequence()
    {
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

    void PrepareItem(AppearItem item)
    {
        var t = item.target.transform;

        // Setup ban đầu tuỳ theo Animation
        switch (item.animation)
        {
            case AnimationType.Scale:
                t.localScale = Vector3.zero;
                break;
            case AnimationType.MoveFromOffset:
                t.localPosition += item.fromPositionOffset;
                break;
            case AnimationType.FadeIn:
                SetAlpha(item.target, item.fromAlpha);
                break;
            case AnimationType.ScaleAndFade:
                t.localScale = Vector3.zero;
                SetAlpha(item.target, item.fromAlpha);
                break;
            case AnimationType.MoveAndFade:
                t.localPosition += item.fromPositionOffset;
                SetAlpha(item.target, item.fromAlpha);
                break;
        }

        item.target.SetActive(true);
    }

    void PlayAnimation(AppearItem item)
    {
        var t = item.target.transform;

        switch (item.animation)
        {
            case AnimationType.Scale:
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutBack();
                break;
            case AnimationType.MoveFromOffset:
                LeanTween.moveLocal(t.gameObject, t.localPosition - item.fromPositionOffset, item.duration).setEaseOutCubic();
                break;
            case AnimationType.FadeIn:
                LeanTween.value(item.target, item.fromAlpha, 1f, item.duration)
                    .setOnUpdate((float val) => SetAlpha(item.target, val));
                break;
            case AnimationType.ScaleAndFade:
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutBack();
                LeanTween.value(item.target, item.fromAlpha, 1f, item.duration)
                    .setOnUpdate((float val) => SetAlpha(item.target, val));
                break;
            case AnimationType.MoveAndFade:
                LeanTween.moveLocal(t.gameObject, t.localPosition - item.fromPositionOffset, item.duration).setEaseOutCubic();
                LeanTween.value(item.target, item.fromAlpha, 1f, item.duration)
                    .setOnUpdate((float val) => SetAlpha(item.target, val));
                break;
        }
    }

    void SetAlpha(GameObject obj, float alpha)
    {
        var canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = alpha;
    }

    void HideAllItems()
    {
        foreach (var item in appearSequence)
        {
            if (item.target != null)
            {
                item.target.SetActive(false);
            }
        }
    }
}
