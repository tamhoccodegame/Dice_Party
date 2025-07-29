using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    public Vector3 camOffset;
    private Transform targetCam; // Vị trí camera cần đến

    private bool isCameraMoving; // Không dùng Networked nữa
    public Transform currentTarget;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        //if (targetCam == null) return;

        Vector3 direction = (currentTarget.position - transform.position).normalized;

        Quaternion newRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 5 * Time.deltaTime);
    }
    public void StartFollowTarget(Transform targetId)
    {
        if (targetCam != null && currentTarget == targetId) return;
        currentTarget = targetId;

        StartCoroutine(ChangeFollowTarget(targetId));
    }

    IEnumerator ChangeFollowTarget(Transform targetId)
    {
        SetCamIsMoving(true);
        //Vector3 oldTarget = transform.position;
        ////Vector3 newTarget = Runner.FindObject(targetId).transform.position + camOffset;

        //float elapsedTime = 0f;
        //float duration = 1.5f;

        //while (elapsedTime < duration)
        //{
        //    transform.position = Vector3.Lerp(oldTarget, newTarget, elapsedTime / duration);
        //    elapsedTime += Runner.DeltaTime;
        //    yield return null;
        //}

        //NetworkId newTargetId = Runner.FindObject(targetId).Id;
        //transform.position = newTarget;

        //RPC_ChangeCameraPosition(newTargetId);
        SetCamIsMoving(false);
        yield return null;

    }

    void SetCamIsMoving(bool enabled)
    {
        isCameraMoving = enabled;
    }

    void ChangeCameraPosition(int newTargetId)
    {
        //targetCam = Runner.FindObject(newTargetId).transform;
    }
}
