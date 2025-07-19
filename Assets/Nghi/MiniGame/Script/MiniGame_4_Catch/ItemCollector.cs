using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    //[Header("Physics Settings")]
    //public float customGravity = -35f;
    //public float bounceForce = 7f;
    //public float fadeDuration = 1f;

    //[Header("Cup Detection")]
    //public string cupZoneTag = "InsideCup";
    //public Transform caughtItemFollowParent; // Gán tự động khi rơi vào ly

    //private Rigidbody rb;
    //private bool isCaught = false;
    //private bool hasLanded = false;
    //private Material mat;

    //void Start()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    rb.useGravity = false;
    //    rb.mass = 0.3f;
    //    rb.drag = 0f;
    //    rb.angularDrag = 0.1f;

    //    // Lấy material để fade
    //    Renderer rend = GetComponentInChildren<Renderer>();
    //    if (rend != null)
    //    {
    //        mat = rend.material;
    //        var c = mat.color; c.a = 1f;
    //        mat.color = c;
    //    }
    //}

    //void FixedUpdate()
    //{
    //    if (!isCaught && !hasLanded)
    //    {
    //        rb.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);
    //    }

    //    // Nếu đã bị bắt → giữ vị trí tương đối
    //    if (isCaught && caughtItemFollowParent != null)
    //    {
    //        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 0.1f);
    //    }
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    // Bắt được item
    //    if (other.CompareTag(cupZoneTag) && !isCaught)
    //    {
    //        isCaught = true;

    //        // Dính vào ly và follow theo Player
    //        rb.velocity = Vector3.zero;
    //        rb.isKinematic = true;
    //        transform.SetParent(other.transform, true);
    //        caughtItemFollowParent = other.transform;

    //        // Phát VFX bắt được (nếu có)
    //        SpawnCatchVFX();

    //        return;
    //    }
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (isCaught) return; // Nếu đã bắt được thì không xử lý va chạm nữa

    //    if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Cup"))
    //    {
    //        rb.velocity = Vector3.zero;
    //        rb.isKinematic = true;
    //        FadeAndDestroy();
    //        return;
    //    }

    //    if (!hasLanded)
    //    {
    //        hasLanded = true;
    //        rb.velocity = Vector3.up * bounceForce;
    //        rb.angularVelocity = Vector3.zero;
    //        rb.mass = 0.1f;
    //        rb.drag = 3f;
    //        rb.angularDrag = 3f;
    //        Invoke(nameof(FadeAndDestroy), 0.1f);
    //    }
    //}

    //void SpawnCatchVFX()
    //{
    //    // Chỗ này gắn prefab VFX hiệu ứng chạm ly
    //    // Instantiate(catchVFXPrefab, transform.position, Quaternion.identity);
    //}

    //void FadeAndDestroy()
    //{
    //    if (mat != null)
    //    {
    //        mat.DOFade(0f, fadeDuration).OnComplete(() => Destroy(gameObject));
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    //void OnTransformParentChanged()
    //{
    //    // Nếu bị tách ra khỏi ly → rớt ra → tự hủy
    //    if (isCaught && transform.parent == null)
    //    {
    //        isCaught = false;
    //        rb.isKinematic = false;
    //        rb.velocity = Vector3.up * 3f;
    //        Invoke(nameof(FadeAndDestroy), 1f);
    //    }
    //}
}
