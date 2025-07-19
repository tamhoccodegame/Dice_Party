using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcherZone : MonoBehaviour
{
    public CinemachineCamera primaryCinemachine;
    public CinemachineCamera[] cinemachineCameras;

    public string triggerTag;

    // Start is called before the first frame update
    void Start()
    {
        SwitchToCamera(primaryCinemachine);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag(triggerTag))
        {
            SwitchToCamera(other.GetComponentInChildren<CinemachineCamera>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            SwitchToCamera(other.GetComponentInChildren<CinemachineCamera>());
        }
    }

    void SwitchToCamera(CinemachineCamera targetCam)
    {
        foreach(var cam in cinemachineCameras)
        {
            cam.enabled = cam == targetCam;
        }
    }
}
