using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupAnimator : MonoBehaviour
{
    [System.Serializable]
    public class FadeItem
    {
        public GameObject target;
        public float delay = 0f;
        public float duration = 0.5f;
        public Ease ease = Ease.OutExpo;
        public EffectType effectType = EffectType.Center;
    }

    [System.Serializable]
    public class TMPFadeItem
    {
        public TextMeshProUGUI targetText;
        public float delay = 0.1f;
        public float duration = 0.4f;
        public Ease ease = Ease.OutExpo;
        public EffectType effectType = EffectType.Center;
    }

    public enum EffectType
    {
        Center,
        LeftToRight,
        RightToLeft
    }

    [Header("Global Settings")]
    public bool playOnStart = true;
    public bool hideAtStart = true;

    [Header("UI GameObjects Fade List")]
    public List<FadeItem> fadeItems = new List<FadeItem>();

    [Header("TMP Text Fade List")]
    public List<TMPFadeItem> tmpFadeItems = new List<TMPFadeItem>();

    void Start()
    {
        if (hideAtStart)
        {
            foreach (var item in fadeItems)
            {
                if (item.target == null) continue;

                CanvasGroup cg = GetOrAddCanvasGroup(item.target);
                cg.alpha = 0f;

                RectTransform rt = item.target.GetComponent<RectTransform>();
                rt.localScale = new Vector3(0f, 1f, 1f);
            }

            foreach (var item in tmpFadeItems)
            {
                if (item.targetText == null) continue;

                CanvasGroup cg = GetOrAddCanvasGroup(item.targetText.gameObject);
                cg.alpha = 0f;

                RectTransform rt = item.targetText.rectTransform;
                rt.localScale = new Vector3(0f, 1f, 1f);
            }
        }

        if (playOnStart)
        {
            PlayFadeSequence();
        }
    }

    public void PlayFadeSequence()
    {
        // GameObject UI elements
        foreach (var item in fadeItems)
        {
            if (item.target == null) continue;

            CanvasGroup cg = GetOrAddCanvasGroup(item.target);
            RectTransform rt = item.target.GetComponent<RectTransform>();

            Vector2 originalPos = rt.anchoredPosition;
            Vector2 offsetPos = originalPos;
            float width = rt.rect.width;

            switch (item.effectType)
            {
                case EffectType.LeftToRight:
                    offsetPos = new Vector2(originalPos.x - width * 0.5f, originalPos.y);
                    rt.anchoredPosition = offsetPos;
                    break;
                case EffectType.RightToLeft:
                    offsetPos = new Vector2(originalPos.x + width * 0.5f, originalPos.y);
                    rt.anchoredPosition = offsetPos;
                    break;
            }

            cg.alpha = 0f;
            rt.localScale = new Vector3(0f, 1f, 1f);

            cg.DOFade(1f, item.duration).SetEase(item.ease).SetDelay(item.delay);
            rt.DOScaleX(1f, item.duration).SetEase(item.ease).SetDelay(item.delay);

            if (item.effectType != EffectType.Center)
            {
                rt.DOAnchorPos(originalPos, item.duration).SetEase(item.ease).SetDelay(item.delay);
            }
        }

        // TMP elements
        foreach (var item in tmpFadeItems)
        {
            if (item.targetText == null) continue;

            GameObject go = item.targetText.gameObject;
            CanvasGroup cg = GetOrAddCanvasGroup(go);
            RectTransform rt = item.targetText.rectTransform;

            Vector2 originalPos = rt.anchoredPosition;
            Vector2 offsetPos = originalPos;
            float width = rt.rect.width;

            switch (item.effectType)
            {
                case EffectType.LeftToRight:
                    offsetPos = new Vector2(originalPos.x - width * 0.5f, originalPos.y);
                    rt.anchoredPosition = offsetPos;
                    break;
                case EffectType.RightToLeft:
                    offsetPos = new Vector2(originalPos.x + width * 0.5f, originalPos.y);
                    rt.anchoredPosition = offsetPos;
                    break;
            }

            cg.alpha = 0f;
            rt.localScale = new Vector3(0f, 1f, 1f);

            cg.DOFade(1f, item.duration).SetEase(item.ease).SetDelay(item.delay);
            rt.DOScaleX(1f, item.duration).SetEase(item.ease).SetDelay(item.delay);

            if (item.effectType != EffectType.Center)
            {
                rt.DOAnchorPos(originalPos, item.duration).SetEase(item.ease).SetDelay(item.delay);
            }
        }
    }

    private CanvasGroup GetOrAddCanvasGroup(GameObject go)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }
}
