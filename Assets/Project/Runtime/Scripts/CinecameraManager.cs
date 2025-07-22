using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CinecameraManager : MonoBehaviour
{
    public static CinecameraManager instance;

    public CinemachineCamera primaryvCam;
    public CinemachineCamera[] vCams;

    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        vCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
    }

    public void TriggerCamera(CinemachineCamera cam)
    {
        foreach(var c in vCams)
        {
            c.enabled = c == cam;
        }
        primaryvCam.Follow = cam.transform.parent;
        primaryvCam.LookAt = cam.transform.parent;
    }

    public void ResetCamera()
    {
        foreach (var c in vCams)
        {
            c.enabled = c == primaryvCam;
        }
    }
}
