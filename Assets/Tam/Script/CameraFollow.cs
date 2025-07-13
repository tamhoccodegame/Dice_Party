using Fusion;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : NetworkBehaviour
{
    public static CameraFollow instance; 

    public Vector3 camOffset;
    private Transform targetCam; // Vị trí camera cần đến
    private float cameraLerpSpeed = 4f; // Tốc độ Lerp (tùy chỉnh)

    private bool isCameraMoving; // Không dùng Networked nữa
    private NetworkId? currentTargetId = null;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (targetCam == null) return;
        if (!isCameraMoving)
        {
            Vector3 desiredPosition = targetCam.position + camOffset;
            if (Vector3.Distance(transform.position, desiredPosition) > 0.3f)
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * cameraLerpSpeed);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartFollowTarget(NetworkId targetId)
    {
        if (targetCam != null && currentTargetId == targetId) return;
        currentTargetId = targetId;

        StartCoroutine(ChangeFollowTarget(targetId));
    }

    IEnumerator ChangeFollowTarget(NetworkId targetId)
    {
        if(HasStateAuthority)
        RPC_SetIsCamMoving(true);
        Vector3 oldTarget = transform.position;
        Vector3 newTarget = Runner.FindObject(targetId).transform.position + camOffset;

        float elapsedTime = 0f;
        float duration = 1.5f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(oldTarget, newTarget, elapsedTime / duration);
            elapsedTime += Runner.DeltaTime;
            yield return null;
        }

        NetworkId newTargetId = Runner.FindObject(targetId).Id;
        transform.position = newTarget;

        if (HasStateAuthority)
        {
            RPC_ChangeCameraPosition(newTargetId);
            RPC_SetIsCamMoving(false);
        }
            
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SetIsCamMoving(bool enabled)
    {
        isCameraMoving = enabled;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ChangeCameraPosition(NetworkId newTargetId)
    {
        targetCam = Runner.FindObject(newTargetId).transform;
    }
}
