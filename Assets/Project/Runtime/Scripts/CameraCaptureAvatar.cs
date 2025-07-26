using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraCaptureAvatar : MonoBehaviour
{
    public List<Camera> characterCameras;
    public int resolutionWidth = 512;
    public int resolutionHeight = 512;

    public void CaptureAllCharacters()
    {
        StartCoroutine(CaptureAllCoroutine());
    }

    private IEnumerator CaptureAllCoroutine()
    {
        for(int i = 0; i < PlayerManager.instance.players.Count; i++)
        {
            yield return StartCoroutine(CaptureCamera(characterCameras[i]));
        }
    }

    private IEnumerator CaptureCamera(Camera cc)
    {
        RenderTexture prevRT = cc.targetTexture;
        RenderTexture rt = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        cc.targetTexture = rt;

        cc.Render();

        RenderTexture.active = rt;
        Texture2D image = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGBA32, false);
        image.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        image.Apply();

        cc.targetTexture = prevRT;
        RenderTexture.active = prevRT;
        Destroy(rt);

        string fileName = $"Player_{characterCameras.IndexOf(cc) + 1}_Avatar" + ".png";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(filePath, image.EncodeToPNG());

        yield return null;
    }
}
