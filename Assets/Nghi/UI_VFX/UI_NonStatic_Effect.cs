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
    Swing,
    PressScale
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

        public float shakeStrength = 10f;
        public int shakeVibrato = 10;
        public float pingPongDistance = 30f;

        public bool useTargetB = false;
        public RectTransform targetB;
        public AnimationType animationTypeB = AnimationType.None;
        public float durationB = 0.5f;
        public float delayB = 0f;
        public bool loopB = false;
        public float shakeStrengthB = 10f;
        public int shakeVibratoB = 10;
        public float pingPongDistanceB = 30f;

        [HideInInspector] public Tween currentTween;
        [HideInInspector] public Tween currentTweenB;


        [Header("Press Scale Settings")]
        public float minScale = 0.9f;
        public float maxScale = 1.2f;
        public int pressLoopCount = 2;   // số lần scale to nhỏ
        public float pressDuration = 0.3f; // thời gian 1 lần to hoặc nhỏ

    }

    public List<UIEffectItem> effectItems = new List<UIEffectItem>();

    private void Start()
    {
        foreach (var item in effectItems)
        {
            if (item.target == null)
            {
                Debug.LogWarning($"UI_NonStatic_Effect: Missing Target A in effectItems on '{gameObject.name}'");
                continue;
            }

            if (item.useTargetB && item.targetB != null)
            {
                item.targetB.gameObject.SetActive(false); // Hide TargetB at start
            }

            if (item.playMode == PlayMode.OnStart)
            {
                PlayEffect(item);
                if (item.useTargetB && item.targetB != null)
                {
                    PlayEffectB(item);
                }
            }
            else if (item.playMode == PlayMode.OnHover || item.playMode == PlayMode.OnClick)
            {
                AddEventTriggers(item);
            }
        }
    }

    private void PlayEffect(UIEffectItem item)
    {
        item.currentTween?.Kill();

        switch (item.animationType)
        {
            case AnimationType.ShakeHorizontal:
                item.currentTween = item.target.DOShakeAnchorPos(item.duration, new Vector2(item.shakeStrength, 0), item.shakeVibrato)
                    .SetDelay(item.delay).SetLoops(item.loop ? -1 : 0, LoopType.Restart);
                break;
            case AnimationType.ShakeVertical:
                item.currentTween = item.target.DOShakeAnchorPos(item.duration, new Vector2(0, item.shakeStrength), item.shakeVibrato)
                    .SetDelay(item.delay).SetLoops(item.loop ? -1 : 0, LoopType.Restart);
                break;
            case AnimationType.PingPongX:
                item.currentTween = item.target.DOAnchorPosX(item.target.anchoredPosition.x + item.pingPongDistance, item.duration / 2)
                    .SetLoops(item.loop ? -1 : 2, LoopType.Yoyo).SetDelay(item.delay).SetEase(Ease.InOutSine);
                break;
            case AnimationType.PingPongY:
                item.currentTween = item.target.DOAnchorPosY(item.target.anchoredPosition.y + item.pingPongDistance, item.duration / 2)
                    .SetLoops(item.loop ? -1 : 2, LoopType.Yoyo).SetDelay(item.delay).SetEase(Ease.InOutSine);
                break;
            case AnimationType.Swing:
                item.target.localRotation = Quaternion.Euler(0, 0, 30);
                item.currentTween = item.target.DOLocalRotate(Vector3.zero, item.duration, RotateMode.Fast)
                    .SetEase(Ease.OutElastic).SetDelay(item.delay).SetLoops(item.loop ? -1 : 0, LoopType.Restart);
                break;
            case AnimationType.PressScale:
                {
                    // Reset scale về 1
                    item.target.localScale = Vector3.one;

                    // Tạo tween scale to nhỏ liên tục
                    item.currentTween = DOTween.Sequence()
                        .Append(item.target.DOScale(item.maxScale, item.pressDuration).SetEase(Ease.OutQuad))
                        .Append(item.target.DOScale(item.minScale, item.pressDuration).SetEase(Ease.InQuad))
                        .SetLoops(item.pressLoopCount * 2, LoopType.Yoyo) // nhân đôi vì to nhỏ là 1 chu kỳ
                        .SetDelay(item.delay);

                    if (item.loop)
                        item.currentTween.SetLoops(-1, LoopType.Yoyo);
                }
                break;

        }
    }

    private void PlayEffectB(UIEffectItem item)
    {
        item.currentTweenB?.Kill();

        switch (item.animationTypeB)
        {
            case AnimationType.ShakeHorizontal:
                item.currentTweenB = item.targetB.DOShakeAnchorPos(item.durationB, new Vector2(item.shakeStrengthB, 0), item.shakeVibratoB)
                    .SetDelay(item.delayB).SetLoops(item.loopB ? -1 : 0, LoopType.Restart);
                break;
            case AnimationType.ShakeVertical:
                item.currentTweenB = item.targetB.DOShakeAnchorPos(item.durationB, new Vector2(0, item.shakeStrengthB), item.shakeVibratoB)
                    .SetDelay(item.delayB).SetLoops(item.loopB ? -1 : 0, LoopType.Restart);
                break;
            case AnimationType.PingPongX:
                item.currentTweenB = item.targetB.DOAnchorPosX(item.targetB.anchoredPosition.x + item.pingPongDistanceB, item.durationB / 2)
                    .SetLoops(item.loopB ? -1 : 2, LoopType.Yoyo).SetDelay(item.delayB).SetEase(Ease.InOutSine);
                break;
            case AnimationType.PingPongY:
                item.currentTweenB = item.targetB.DOAnchorPosY(item.targetB.anchoredPosition.y + item.pingPongDistanceB, item.durationB / 2)
                    .SetLoops(item.loopB ? -1 : 2, LoopType.Yoyo).SetDelay(item.delayB).SetEase(Ease.InOutSine);
                break;
            case AnimationType.Swing:
                item.targetB.localRotation = Quaternion.Euler(0, 0, 30);
                item.currentTweenB = item.targetB.DOLocalRotate(Vector3.zero, item.durationB, RotateMode.Fast)
                    .SetEase(Ease.OutElastic).SetDelay(item.delayB).SetLoops(item.loopB ? -1 : 0, LoopType.Restart);
                break;
            case AnimationType.PressScale:
                {
                    item.targetB.localScale = Vector3.one;

                    item.currentTweenB = DOTween.Sequence()
                        .Append(item.targetB.DOScale(item.maxScale, item.pressDuration).SetEase(Ease.OutQuad))
                        .Append(item.targetB.DOScale(item.minScale, item.pressDuration).SetEase(Ease.InQuad))
                        .SetLoops(item.pressLoopCount * 2, LoopType.Yoyo)
                        .SetDelay(item.delayB);

                    if (item.loopB)
                        item.currentTweenB.SetLoops(-1, LoopType.Yoyo);
                }
                break;

        }
    }

    private void AddEventTriggers(UIEffectItem item)
    {
        if (item.target == null)
        {
            Debug.LogWarning($"UI_NonStatic_Effect: Target A is null in one of the effectItems on GameObject '{gameObject.name}'");
            return;
        }

        EventTrigger trigger = item.target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = item.target.gameObject.AddComponent<EventTrigger>();
        }

        if (item.playMode == PlayMode.OnHover)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
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

                if (item.useTargetB && item.targetB != null)
                {
                    if (!item.targetB.gameObject.activeSelf)
                        item.targetB.gameObject.SetActive(true);

                    if (item.loopB)
                    {
                        if (item.currentTweenB == null || !item.currentTweenB.IsActive())
                            PlayEffectB(item);
                    }
                    else
                    {
                        PlayEffectB(item);
                    }
                }
            });
            trigger.triggers.Add(entry);
        }

        if (item.playMode == PlayMode.OnClick)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
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

                if (item.useTargetB && item.targetB != null)
                {
                    if (!item.targetB.gameObject.activeSelf)
                        item.targetB.gameObject.SetActive(true);

                    if (item.loopB)
                    {
                        if (item.currentTweenB == null || !item.currentTweenB.IsActive())
                            PlayEffectB(item);
                    }
                    else
                    {
                        PlayEffectB(item);
                    }
                }
            });
            trigger.triggers.Add(entry);
        }


        //if (item.playMode == PlayMode.OnHover)
        //{
        //    EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        //    entry.callback.AddListener((data) =>
        //    {
        //        if (item.loop)
        //        {
        //            if (item.currentTween == null || !item.currentTween.IsActive())
        //                PlayEffect(item);
        //        }
        //        else
        //        {
        //            PlayEffect(item);
        //        }

        //        if (item.useTargetB && item.targetB != null)
        //        {
        //            if (item.loopB)
        //            {
        //                if (item.currentTweenB == null || !item.currentTweenB.IsActive())
        //                    PlayEffectB(item);
        //            }
        //            else
        //            {
        //                PlayEffectB(item);
        //            }
        //        }
        //    });
        //    trigger.triggers.Add(entry);
        //}

        //if (item.playMode == PlayMode.OnClick)
        //{
        //    EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        //    entry.callback.AddListener((data) =>
        //    {
        //        if (item.loop)
        //        {
        //            if (item.currentTween == null || !item.currentTween.IsActive())
        //                PlayEffect(item);
        //        }
        //        else
        //        {
        //            PlayEffect(item);
        //        }

        //        if (item.useTargetB && item.targetB != null)
        //        {
        //            if (item.loopB)
        //            {
        //                if (item.currentTweenB == null || !item.currentTweenB.IsActive())
        //                    PlayEffectB(item);
        //            }
        //            else
        //            {
        //                PlayEffectB(item);
        //            }
        //        }
        //    });
        //    trigger.triggers.Add(entry);
        //}
    }

}
