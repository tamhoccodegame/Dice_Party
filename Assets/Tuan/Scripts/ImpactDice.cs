using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ImpactDice : MonoBehaviour
{
    public float jumpPower = 2f;
    public float jumpDuration = 1.5f;
    public float rotationDuration = 1.5f;
    public float shakeStrength = 0.2f;
    public float shakeDuration = 0.3f;


    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    public void TriggerImpact()
    {
        Sequence s = DOTween.Sequence();
        float halfDuration = jumpDuration / 2f;

        s.Append(transform.DOMoveY(originalPosition.y + jumpPower, halfDuration).SetEase(Ease.OutQuad));

        Vector3 randomRotation = new Vector3(360, 360, 0);
        s.Join(transform.DORotate(transform.eulerAngles + randomRotation, rotationDuration, RotateMode.FastBeyond360)
                      .SetEase(Ease.OutSine));
        s.Append(transform.DOMoveY(originalPosition.y, halfDuration).SetEase(Ease.InQuad));
        s.Append(transform.DOShakePosition(shakeDuration, shakeStrength, 10, 90, false, true));
    }
}
