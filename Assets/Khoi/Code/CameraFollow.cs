using Unity.Cinemachine;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public CinemachineCamera FollowCamera;
    public void AssignCamera(Transform target)
    {
        FollowCamera.Follow = target;
        FollowCamera.LookAt = target;
    }
}
