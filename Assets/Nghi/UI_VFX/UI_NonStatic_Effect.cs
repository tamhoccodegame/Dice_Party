using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public enum AnimationType
{
    None,
    ShakeHorizontal,
    ShakeVertical,
    PingPongX,
    PingPongY,
    Swing
}

public enum PlayMode
{
    None,
    OnStart,
    OnHover,
    OnClick
}

public class UI_NonStatic_Effect : MonoBehaviour
{
    [System.Serializable]
    public class UIEffectItem
    {
        public RectTransform target;
        public PlayMode playMode = PlayMode.None;
        public AnimationType animationType = AnimationType.None;
        public float duration = 0.5f;
        public float delay = 0f;
        public bool loop = false;

        // Specific parameters per effect type
        public float shakeStrength = 10f;
        public int shakeVibrato = 10;
        public float pingPongDistance = 30f;

        [HideInInspector] public Tween currentTween; // Store the running tween
    }

    public List<UIEffectItem> effectItems = new List<UIEffectItem>();

    private void Start()
    {
        foreach (var item in effectItems)
        {
            if (item.playMode == PlayMode.OnStart)
            {
                PlayEffect(item);
            }
            else if (item.playMode == PlayMode.OnHover || item.playMode == PlayMode.OnClick)
            {
                AddEventTriggers(item);
            }
        }
    }

    private void PlayEffect(UIEffectItem item)
    {
        // Kill previous tween if exists
        item.currentTween?.Kill();

        switch (item.animationType)
        {
            case AnimationType.ShakeHorizontal:
                item.currentTween = item.target.DOShakeAnchorPos(item.duration, new Vector2(item.shakeStrength, 0), item.shakeVibrato)
                    .SetDelay(item.delay)
                    .SetLoops(item.loop ? -1 : 0, LoopType.Restart);
                break;

            case AnimationType.ShakeVertical:
                item.currentTween = item.target.DOShakeAnchorPos(item.duration, new Vector2(0, item.shakeStrength), item.shakeVibrato)
                    .SetDelay(item.delay)
                    .SetLoops(item.loop ? -1 : 0, LoopType.Restart);
                break;

            case AnimationType.PingPongX:
                item.currentTween = item.target.DOAnchorPosX(item.target.anchoredPosition.x + item.pingPongDistance, item.duration / 2)
                    .SetLoops(item.loop ? -1 : 2, LoopType.Yoyo)
                    .SetDelay(item.delay)
                    .SetEase(Ease.InOutSine);
                break;

            case AnimationType.PingPongY:
                item.currentTween = item.target.DOAnchorPosY(item.target.anchoredPosition.y + item.pingPongDistance, item.duration / 2)
                    .SetLoops(item.loop ? -1 : 2, LoopType.Yoyo)
                    .SetDelay(item.delay)
                    .SetEase(Ease.InOutSine);
                break;

            case AnimationType.Swing:
                item.target.localRotation = Quaternion.Euler(0, 0, 30);
                item.currentTween = item.target.DOLocalRotate(Vector3.zero, item.duration, RotateMode.Fast)
                    .SetEase(Ease.OutElastic)
                    .SetDelay(item.delay)
                    .SetLoops(item.loop ? -1 : 0, LoopType.Restart);
                break;
        }
    }

    private void AddEventTriggers(UIEffectItem item)
    {
        EventTrigger trigger = item.target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = item.target.gameObject.AddComponent<EventTrigger>();
        }

        if (item.playMode == PlayMode.OnHover)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((data) =>
            {
                if (item.loop)
                {
                    // Play loop only once and keep looping
                    if (item.currentTween == null || !item.currentTween.IsActive())
                        PlayEffect(item);
                }
                else
                {
                    PlayEffect(item);
                }
            });
            trigger.triggers.Add(entry);
        }

        if (item.playMode == PlayMode.OnClick)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((data) =>
            {
                if (item.loop)
                {
                    if (item.currentTween == null || !item.currentTween.IsActive())
                        PlayEffect(item);
                }
                else
                {
                    PlayEffect(item);
                }
            });
            trigger.triggers.Add(entry);
        }
    }
}
