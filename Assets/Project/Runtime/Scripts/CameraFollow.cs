using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    public Vector3 camOffset; // offset để giữ khoảng cách camera
    private bool isCameraMoving;
    public Transform currentTarget;
    public Transform proxyTarget;
    public Quaternion initProxyRotation;

    public Transform[] DebugTargets;
    public int currentTargetIndex = 0;

    public CinemachineCamera defaultCm;
    public CinemachineCamera juctionCm;
    public CinemachineCamera rollCm;
    public CinemachineCamera zoomCm;

    public enum CameraState
    {
        Default,
        Juction,
        Roll,
        Zoom,
    }

    public CameraState camState;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        defaultCm.Follow = proxyTarget;
        zoomCm.Follow = proxyTarget;
        rollCm.Follow = proxyTarget;
        juctionCm.Follow = proxyTarget;
        initProxyRotation = proxyTarget.transform.rotation;
    }

    private void Update()
    {
        if(currentTarget != null)
        {
            Vector3 newProxyPosition = currentTarget.position;
            newProxyPosition.y = proxyTarget.position.y;
            proxyTarget.transform.position = newProxyPosition;
        }
    }

    public void StartFollowTarget(Transform target)
    {
        if (currentTarget == target) return;

        currentTarget = target;
        StartCoroutine(ChangeFollowTarget(target));
    }

    [ContextMenu("Change Target")]
    public void TestChangeTargetCamera()
    {
        currentTargetIndex++;
        currentTargetIndex %= DebugTargets.Length;
        StartFollowTarget(DebugTargets[currentTargetIndex]);
    }

    public void SwitchCamera(CameraState camState)
    {
        this.camState = camState;

        defaultCm.Priority = 10;
        juctionCm.Priority = 10;
        rollCm.Priority = 10;
        zoomCm.Priority = 10;

        switch (camState)
        {
            case CameraState.Default:
                defaultCm.Priority = 20;
                break;
            case CameraState.Juction:
                juctionCm.Priority = 20;
                break;
            case CameraState.Roll:
                rollCm.Priority = 20;
                break;
            case CameraState.Zoom:
                zoomCm.Priority = 20;
                break;
        }
    }

    IEnumerator ChangeFollowTarget(Transform target)
    {
        SetCamIsMoving(true);

        Vector3 startPos = proxyTarget.position;
        Vector3 endPos = target.position;
        endPos.y = proxyTarget.position.y;

        float duration = 2f;
        float elapsed = 0f;
         
        while (elapsed < duration)
        {
            proxyTarget.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        proxyTarget.position = endPos;

        SwitchCamera(CameraState.Roll);
        SetCamIsMoving(false);
    }

    void SetCamIsMoving(bool enabled)
    {
        isCameraMoving = enabled;
    }
}
