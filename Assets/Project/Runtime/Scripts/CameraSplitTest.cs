using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSplitTest : MonoBehaviour
{
    public float a, b, c, d;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.rect = new Rect(a, b, c, d);
    }

}
