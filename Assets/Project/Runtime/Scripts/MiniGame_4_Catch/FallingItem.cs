using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static Dreamteck.Splines.ParticleController;
[RequireComponent(typeof(Rigidbody))]

public class FallingItem : MonoBehaviour
{
    public enum ItemType { Normal, Bomb }
    public ItemType itemType = ItemType.Normal;

    [Header("Gravity & Fall Settings")]
    public float customGravity = -35f;
    public float maxFallSpeed = 50f;

    [Header("Rotation While Falling")]
    public float baseAngularSpeed = 3f;
    public float angularRandomness = 2f;
    public float maxAngularVelocity = 8f;

    [Header("Bounce Settings")]
    public float bounceForce = 7f;
    public float bounceRandomness = 1.5f;
    public float bounceUpwardFactor = 1.0f; // Hệ số chỉnh độ hướng lên

    [Header("Fade Settings")]
    public float fadeDelay = 0.1f;
    public float fadeDuration = 1.0f;

    private Rigidbody rb;
    private bool hasLanded = false;
    private Material mat;

    [Header("Cup Visual Settings")]
    public GameObject visualInCupPrefab; // Prefab nhỏ để hiện trong ly

    public GameObject shadowPrefab; // Prefab bóng

    private Transform shadowInstance;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 0f;
        rb.angularDrag = 0.1f;
        rb.mass = 0.4f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Random angular velocity để vật thể xoay nhẹ
        Vector3 spin = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-1f, 1f)
        ).normalized * (baseAngularSpeed + Random.Range(0f, angularRandomness));

        rb.angularVelocity = spin;

        // Get material for fade
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            mat = rend.material;
            Color color = mat.color;
            color.a = 1f;
            mat.color = color;
        }



        // Spawn shadow
        if (shadowPrefab != null)
        {
            shadowInstance = Instantiate(shadowPrefab).transform;

            // Gán target cho bóng
            var shadowFollower = shadowInstance.GetComponent<BlobShadowFollower>();
            shadowFollower.target = this.transform;
        }

    }

    void OnDestroy()
    {
        // Khi item bị hủy → hủy bóng
        if (shadowInstance != null)
            Destroy(shadowInstance.gameObject);
    }

    void FixedUpdate()
    {
        if (!hasLanded)
        {
            // Apply custom gravity
            rb.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);

            // Limit fall speed
            if (rb.velocity.magnitude > maxFallSpeed)
                rb.velocity = rb.velocity.normalized * maxFallSpeed;

            // Clamp angular velocity
            if (rb.angularVelocity.magnitude > maxAngularVelocity)
                rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        if (collision.collider.CompareTag("Cup"))
        {
            var cup = collision.collider.GetComponent<ItemCollector>();

            if (cup != null)// && visualInCupPrefab != null
            {
                Vector3 hitPoint = collision.contacts[0].point;
                //cup.AddItem(this.gameObject, hitPoint);
                cup.AddItem(this.gameObject, hitPoint, false);
                hasLanded = true;

                return; // KHÔNG tiếp tục xử lý nảy nữa
            }
        }
        else if (collision.collider.CompareTag("Ground"))
        {
            var cup = FindObjectOfType<ItemCollector>();
            if (cup != null && itemType == ItemType.Bomb)
            {
                // Ground hit -> isGroundHit = true
                cup.AddItem(this.gameObject, collision.contacts[0].point, true);
            }
            hasLanded = true;

        }

        hasLanded = true;

        // Tính lực nảy bật lên
        float bounceStrength = bounceForce + Random.Range(-bounceRandomness, bounceRandomness);
        Vector3 bounceDir = Vector3.up * bounceStrength * bounceUpwardFactor;

        rb.velocity = bounceDir;

        // Giảm xoay sau khi nảy
        rb.angularVelocity = Random.insideUnitSphere * (baseAngularSpeed / 2f);
        rb.drag = 2f;
        rb.angularDrag = 3f;

        // Nhẹ nhàng hơn sau khi chạm
        rb.mass = 0.5f;

        Invoke(nameof(FadeOutAndDestroy), fadeDelay);
    }




    void FadeOutAndDestroy()
    {
        if (mat != null)
        {
            mat.DOFade(0f, fadeDuration).OnComplete(() => Destroy(gameObject));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
