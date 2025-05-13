using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientApplier : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] Image image;
    [SerializeField] Color startColor = Color.cyan;
    [SerializeField] Color endColor = Color.magenta;

    void Start()
    {
        Texture2D gradientTex = GradientGenerator.CreateHorizontalGradient(startColor, endColor, 512, 1);
        rawImage.texture = gradientTex;

        Sprite gradientSprite = Sprite.Create(gradientTex, new Rect(0, 0, gradientTex.width, gradientTex.height), new Vector2(0.5f, 0.5f));
        image.sprite = gradientSprite;
    }
}
