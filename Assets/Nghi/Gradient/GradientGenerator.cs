using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GradientGenerator
{
    public static Texture2D CreateHorizontalGradient(Color colorA, Color colorB, int width = 256, int height = 1)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int x = 0; x < width; x++)
        {
            Color lerped = Color.Lerp(colorA, colorB, (float)x / (width - 1));
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, lerped);
            }
        }

        texture.Apply();
        return texture;
    }

    public static Texture2D CreateVerticalGradient(Color colorA, Color colorB, int width = 1, int height = 256)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < height; y++)
        {
            Color lerped = Color.Lerp(colorA, colorB, (float)y / (height - 1));
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, lerped);
            }
        }

        texture.Apply();
        return texture;
    }
}
