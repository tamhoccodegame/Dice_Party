using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    public Vector3 camOffset; // offset để giữ khoảng cách camera
    private bool isCameraMoving;
    public Transform currentTarget;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // Nếu đã có target và camera đang không trong trạng thái chuyển
        if (currentTarget != null && !isCameraMoving)
        {
            Vector3 targetPos = currentTarget.position + camOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
        }
    }

    public void StartFollowTarget(Transform target)
    {
        if (currentTarget == target) return;

        currentTarget = target;
        StartCoroutine(ChangeFollowTarget(target));
    }

    IEnumerator ChangeFollowTarget(Transform target)
    {
        SetCamIsMoving(true);

        Vector3 startPos = transform.position;
        Vector3 endPos = target.position + camOffset;

        float duration = 1.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        SetCamIsMoving(false);
    }

    void SetCamIsMoving(bool enabled)
    {
        isCameraMoving = enabled;
    }
}
