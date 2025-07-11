using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class GlobalVolume : MonoBehaviour
{
    public Volume volume;
    private ColorAdjustments colorAdjustments;

    [Range(0f, 5f)]
    public float fadeDuration = 2f;

    private float timer = 0f;
    private bool isFading = false;

    public Color startColor = Color.black;
    public Color endColor = Color.white;

    void Start()
    {
        if (volume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.colorFilter.value = startColor;
            StartFadeOut();
        }
    }

    public void StartFadeOut()
    {
        timer = 0f;
        isFading = true;
    }

    void Update()
    {
        if (!isFading || colorAdjustments == null) return;

        timer += Time.deltaTime / fadeDuration;
        colorAdjustments.colorFilter.value = Color.Lerp(startColor, endColor, timer);
        if (timer >= 1f)
        {
            isFading = false;
            colorAdjustments.colorFilter.value = endColor;
        }
    }
}
