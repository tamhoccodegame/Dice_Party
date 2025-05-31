
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class UIAppearFadeEffectV3 : MonoBehaviour
{
    public enum FadeDirection { Center, LeftToRight, RightToLeft }

    [System.Serializable]
    public class FadeItem
    {
        public GameObject target;
        public FadeDirection direction = FadeDirection.Center;
        public float delay = 0.0f;
        [Range(0.1f, 3f)] public float duration = 0.5f;
    }

    [System.Serializable]
    public class TextFadeItem
    {
        public TMP_Text text;
        public FadeDirection direction = FadeDirection.Center;
        public float delay = 0.1f;
        [Range(0.1f, 3f)] public float duration = 0.5f;
    }

    [Header("UI GameObject Fade Settings")]
    public List<FadeItem> uiFadeList = new List<FadeItem>();

    [Header("Text Fade Settings")]
    public List<TextFadeItem> textFadeList = new List<TextFadeItem>();

    [Header("General Settings")]
    public bool playOnAwake = true;

    void Start()
    {
        if (playOnAwake)
        {
            PlayFadeEffects();
        }
    }

    public void PlayFadeEffects()
    {
        foreach (var item in uiFadeList)
        {
            if (item.target != null)
                StartCoroutine(FadeUI(item));
        }

        foreach (var item in textFadeList)
        {
            if (item.text != null)
                StartCoroutine(FadeText(item));
        }
    }

    IEnumerator FadeUI(FadeItem item)
    {
        CanvasGroup group = item.target.GetComponent<CanvasGroup>();
        if (group == null) group = item.target.AddComponent<CanvasGroup>();

        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;

        float time = 0f;
        float duration = item.duration;
        float delay = item.delay;

        // Chờ delay
        yield return new WaitForSeconds(delay);

        // Bắt đầu tween alpha
        DOTween.To(() => group.alpha, x => group.alpha = x, 1f, duration)
            .SetEase(Ease.OutCubic);

        // Hiệu ứng hướng: tạo cảm giác "quét ngang"
        float maskProgress = 0f;
        float stepTime = 0.01f;
        Vector2 pivot = Vector2.one * 0.5f;

        if (item.target.TryGetComponent<RectTransform>(out RectTransform rect))
            pivot = rect.pivot;

        while (time < duration)
        {
            maskProgress = time / duration;
            float dirValue = GetMaskDirection(item.direction, maskProgress);
            group.alpha = dirValue;
            time += Time.deltaTime;
            yield return null;
        }

        group.alpha = 1;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    IEnumerator FadeText(TextFadeItem item)
    {
        TMP_Text text = item.text;
        text.ForceMeshUpdate();

        int charCount = text.textInfo.characterCount;
        float duration = item.duration;
        float delay = item.delay;

        // Alpha về 0
        for (int i = 0; i < charCount; i++)
        {
            int matIndex = text.textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = text.textInfo.characterInfo[i].vertexIndex;

            if (!text.textInfo.characterInfo[i].isVisible) continue;

            Color32[] colors = text.textInfo.meshInfo[matIndex].colors32;
            for (int j = 0; j < 4; j++)
                colors[vertexIndex + j].a = 0;
        }
        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        yield return new WaitForSeconds(delay);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            for (int i = 0; i < charCount; i++)
            {
                if (!text.textInfo.characterInfo[i].isVisible) continue;

                float fadeValue = GetMaskDirection(item.direction, (float)i / charCount + t * 0.2f);
                fadeValue = Mathf.Clamp01(fadeValue);

                int matIndex = text.textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = text.textInfo.characterInfo[i].vertexIndex;

                Color32[] colors = text.textInfo.meshInfo[matIndex].colors32;
                for (int j = 0; j < 4; j++)
                    colors[vertexIndex + j].a = (byte)(fadeValue * 255);
            }

            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo hiện ra hoàn toàn
        for (int i = 0; i < charCount; i++)
        {
            if (!text.textInfo.characterInfo[i].isVisible) continue;

            int matIndex = text.textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = text.textInfo.characterInfo[i].vertexIndex;

            Color32[] colors = text.textInfo.meshInfo[matIndex].colors32;
            for (int j = 0; j < 4; j++)
                colors[vertexIndex + j].a = 255;
        }

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    float GetMaskDirection(FadeDirection dir, float t)
    {
        t = Mathf.Clamp01(t);

        switch (dir)
        {
            case FadeDirection.Center:
                return Mathf.Clamp01(1f - Mathf.Abs(t * 2f - 1f)); // fade từ giữa ra
            case FadeDirection.LeftToRight:
                return t; // fade trái → phải
            case FadeDirection.RightToLeft:
                return 1f - t; // fade phải → trái
            default:
                return t;
        }
    }
}
