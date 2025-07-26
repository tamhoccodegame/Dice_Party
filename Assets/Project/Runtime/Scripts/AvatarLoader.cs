using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AvatarLoader : MonoBehaviour
{
    public static AvatarLoader instance;

    private void Awake()
    {
        instance = this;
    }

    public Sprite GetAvatarSprite(int playerIndex)
    {
        string avatarPath = Path.Combine(Application.persistentDataPath, $"Player_{playerIndex + 1}_Avatar.png");
        
        if(File.Exists(avatarPath))
        {
            byte[] imageBytes = File.ReadAllBytes(avatarPath);

            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(imageBytes);

            Sprite avatarSprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f));

            return avatarSprite;
        }
        else
        {
            return null;
        }

      
    }

}
