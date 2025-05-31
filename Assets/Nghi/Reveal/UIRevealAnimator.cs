using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRevealAnimator : MonoBehaviour
{
    public enum Direction { Center = 0, LeftToRight = 1, RightToLeft = 2 }

    [System.Serializable]
    public class RevealItem
    {
        public Graphic graphic; // Image or TextMeshProUGUI
        public float delay = 0f;
        public float duration = 0.6f;
        public Ease ease = Ease.OutQuart;
        public Direction direction = Direction.Center;
    }

    [Header("Reveal Settings")]
    public bool playOnStart = true;
    public bool hideAtStart = true;

    public List<RevealItem> revealItems = new List<RevealItem>();

    void Start()
    {
        if (hideAtStart)
        {
            foreach (var item in revealItems)
            {
                if (item.graphic == null) continue;

                Material mat = new Material(item.graphic.material);
                mat.SetFloat("_RevealAmount", 0f);
                mat.SetFloat("_Direction", (float)item.direction);
                item.graphic.material = mat;
            }
        }

        if (playOnStart)
            PlayReveal();
    }

    public void PlayReveal()
    {
        foreach (var item in revealItems)
        {
            if (item.graphic == null) continue;

            Material mat = item.graphic.material;
            float from = 0f;
            float to = 1f;

            DOTween.To(() => from, x => {
                from = x;
                mat.SetFloat("_RevealAmount", x);
            }, to, item.duration)
            .SetEase(item.ease)
            .SetDelay(item.delay);
        }
    }
}
