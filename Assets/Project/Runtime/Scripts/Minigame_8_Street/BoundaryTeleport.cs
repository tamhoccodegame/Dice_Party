using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryTeleport : MonoBehaviour
{
    [Header("Teleport Config")]
    public Transform teleportPoint;   // điểm player sẽ quay về
    public float delay = 1f;          // thời gian delay trước khi teleport
    public float heightOffset = 5f;   // khoảng cách spawn trên cao

    private bool isTeleporting = false;

    private void OnTriggerStay(Collider other)
    {
        if (!isTeleporting && other.CompareTag("Player"))
        {
            StartCoroutine(TeleportAfterDelay(other.gameObject));
        }
    }

    private IEnumerator TeleportAfterDelay(GameObject player)
    {
        isTeleporting = true;  // tránh gọi nhiều lần
        yield return new WaitForSeconds(delay);

        if (player != null && teleportPoint != null)
        {
            // Tắt velocity nếu có Rigidbody
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Teleport lên trên cao so với teleportPoint
            Vector3 targetPos = teleportPoint.position + Vector3.up * heightOffset;
            player.transform.position = targetPos;

            Debug.Log($"Player teleported above {teleportPoint.position} and will fall down.");
            isTeleporting = false;
        }
    }
}
