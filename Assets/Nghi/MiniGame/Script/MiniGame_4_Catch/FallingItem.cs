using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
public class FallingItem : MonoBehaviour
{
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
    public float bounceUpwardFactor = 1.0f;

    [Header("Fade Settings")]
    public float fadeDelay = 0.5f; // fade sau khi rớt đất 0.5s
    public float fadeDuration = 1.0f;

    [Header("In-Cup Settings")]
    public float cupDrag = 2f;
    public float cupAngularDrag = 4f;
    public float cupMass = 0.8f;
    public LayerMask inCupLayer;

    private Rigidbody rb;
    private bool hasLanded = false;
    private bool inCup = false;
    private Material mat;

    public GameObject VFX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 0f;
        rb.angularDrag = 0.1f;
        rb.mass = 0.4f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Xoay nhẹ tự nhiên
        Vector3 spin = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-1f, 1f)
        ).normalized * (baseAngularSpeed + Random.Range(0f, angularRandomness));
        rb.angularVelocity = spin;

        // Lấy vật liệu để fade
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            mat = rend.material;
            Color color = mat.color;
            color.a = 1f;
            mat.color = color;
        }
    }

    void FixedUpdate()
    {
        if (!inCup && !hasLanded)
        {
            // Trọng lực tùy chỉnh
            rb.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);

            // Giới hạn tốc độ rơi
            if (rb.velocity.magnitude > maxFallSpeed)
                rb.velocity = rb.velocity.normalized * maxFallSpeed;

            // Giới hạn xoay
            if (rb.angularVelocity.magnitude > maxAngularVelocity)
                rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity;
        }

        // Rớt khỏi ly xuống đất
        if (inCup && transform.position.y < 0.1f)
        {
            inCup = false;
            FadeOutAndDestroy();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InsideCup") && !inCup)
        {
            inCup = true;
            hasLanded = true;

            // Làm item nặng, khó bị xô
            rb.mass = 5f;
            rb.drag = 3f;
            rb.angularDrag = 4f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            gameObject.layer = LayerMask.NameToLayer("InCupItem");

            // VFX
            if (VFX != null)
                Instantiate(VFX, transform.position, Quaternion.identity);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        // Nếu đã trong ly → vẫn va chạm nhẹ, không phá vật lý
        if (inCup)
        {
            // Chỉ giảm lực va, không văng loạn
            rb.velocity *= 0.2f;
            rb.angularVelocity *= 0.2f;
            return;
        }

        if (collision.collider.CompareTag("Player"))
        {
            // Không văng, chỉ hấp thụ lực nếu Player va trúng
            rb.velocity *= 0.3f;
            rb.angularVelocity *= 0.3f;
            return;
        }

        if (collision.collider.CompareTag("Cup") && !inCup)
        {
            ContactPoint contact = collision.contacts[0];

            // Hướng phản xạ
            Vector3 reflectDir = Vector3.Reflect(rb.velocity, contact.normal).normalized;

            // Bật lên nhẹ để tạo cảm giác vật lý Parabol
            float bounceStrength = bounceForce * 0.5f + Random.Range(-0.5f, 0.5f);
            rb.velocity = reflectDir * bounceStrength;

            // Tiếp tục rơi với trọng lực
            hasLanded = false;

            return;
        }


        if (collision.collider.CompareTag("Ground") && !hasLanded)
        {
            hasLanded = true;

            float bounceStrength = bounceForce + Random.Range(-bounceRandomness, bounceRandomness);
            Vector3 bounceDir = Vector3.up * bounceStrength * bounceUpwardFactor;

            rb.velocity = bounceDir;
            rb.angularVelocity = Random.insideUnitSphere * (baseAngularSpeed / 2f);
            rb.drag = 2f;
            rb.angularDrag = 3f;

            Invoke(nameof(FadeOutAndDestroy), fadeDelay);
        }
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








    //[Header("Gravity & Fall Settings")]
    //public float customGravity = -35f;
    //public float maxFallSpeed = 50f;

    //[Header("Rotation While Falling")]
    //public float baseAngularSpeed = 3f;
    //public float angularRandomness = 2f;
    //public float maxAngularVelocity = 8f;

    //[Header("Bounce Settings")]
    //public float bounceForce = 7f;
    //public float bounceRandomness = 1.5f;
    //public float bounceUpwardFactor = 1.0f;

    //[Header("Fade Settings")]
    //public float fadeDelay = 0.1f;
    //public float fadeDuration = 1.0f;

    //[Header("In-Cup Settings")]
    //public float cupDrag = 1f;
    //public float cupAngularDrag = 2f;
    //public LayerMask inCupLayer;

    //private Rigidbody rb;
    //private bool hasLanded = false;
    //private bool inCup = false;
    //private Material mat;

    //public GameObject VFX;

    //void Start()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    rb.useGravity = false;
    //    rb.drag = 0f;
    //    rb.angularDrag = 0.1f;
    //    rb.mass = 0.4f;
    //    rb.interpolation = RigidbodyInterpolation.Interpolate;
    //    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

    //    // Random xoay ban đầu
    //    Vector3 spin = new Vector3(
    //        Random.Range(-1f, 1f),
    //        Random.Range(-0.5f, 0.5f),
    //        Random.Range(-1f, 1f)
    //    ).normalized * (baseAngularSpeed + Random.Range(0f, angularRandomness));
    //    rb.angularVelocity = spin;

    //    // Lấy vật liệu để fade
    //    Renderer rend = GetComponentInChildren<Renderer>();
    //    if (rend != null)
    //    {
    //        mat = rend.material;
    //        Color color = mat.color;
    //        color.a = 1f;
    //        mat.color = color;
    //    }
    //}

    //void FixedUpdate()
    //{
    //    if (!inCup && !hasLanded)
    //    {
    //        // Trọng lực tuỳ chỉnh
    //        rb.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);

    //        // Giới hạn tốc độ rơi
    //        if (rb.velocity.magnitude > maxFallSpeed)
    //            rb.velocity = rb.velocity.normalized * maxFallSpeed;

    //        // Giới hạn xoay
    //        if (rb.angularVelocity.magnitude > maxAngularVelocity)
    //            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity;
    //    }

    //    // Nếu trong ly mà rớt ra ngoài thì fade
    //    if (inCup && transform.position.y < 0.1f)
    //    {
    //        inCup = false;
    //        FadeOutAndDestroy();
    //    }
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    // Nếu rơi trúng lòng ly
    //    if (other.CompareTag("InsideCup") && !inCup)
    //    {
    //        inCup = true;
    //        rb.drag = cupDrag;
    //        rb.angularDrag = cupAngularDrag;
    //        gameObject.layer = Mathf.RoundToInt(Mathf.Log(inCupLayer.value, 2)); // Đổi layer
    //        hasLanded = true;
    //        Debug.Log("Caught!");
    //        // Play VFX nếu cần
    //        Instantiate(VFX, transform.position, Quaternion.identity);
    //    }
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (inCup) return; // Nếu đã trong ly thì không xử lý bounce nữa

    //    if (collision.collider.CompareTag("Player"))
    //    {

    //        return;
    //    }

    //    // Chạm ly (có thể là thành ly) → cho bật nhẹ lên, tiếp tục vật lý
    //    if (collision.collider.CompareTag("Cup"))
    //    {
    //        // Cho bật lên nhẹ như trúng mép
    //        ContactPoint contact = collision.contacts[0];
    //        Vector3 reflectDir = Vector3.Reflect(rb.velocity, contact.normal).normalized;
    //        float bounceStrength = bounceForce * 0.5f + Random.Range(-1f, 1f);

    //        rb.velocity = reflectDir * bounceStrength;

    //        return;
    //    }


    //    //if (!hasLanded)
    //    //{
    //    //    hasLanded = true;

    //    //    float bounceStrength = bounceForce + Random.Range(-bounceRandomness, bounceRandomness);
    //    //    Vector3 bounceDir = Vector3.up * bounceStrength * bounceUpwardFactor;

    //    //    rb.velocity = bounceDir;
    //    //    rb.angularVelocity = Vector3.zero;
    //    //    rb.drag = 2f;
    //    //    rb.angularDrag = 3f;
    //    //    rb.mass = 0.1f;

    //    //    Invoke(nameof(FadeOutAndDestroy), fadeDelay);
    //    //}


    //    if (collision.collider.CompareTag("Ground"))
    //    {
    //        hasLanded = true;

    //        float bounceStrength = bounceForce + Random.Range(-bounceRandomness, bounceRandomness);
    //        Vector3 bounceDir = Vector3.up * bounceStrength * bounceUpwardFactor;

    //        rb.velocity = bounceDir;
    //        rb.angularVelocity = Random.insideUnitSphere * (baseAngularSpeed / 2f);
    //        rb.drag = 2f;
    //        rb.angularDrag = 3f;

    //        Invoke(nameof(FadeOutAndDestroy), fadeDelay);
    //    }

    //}

    //void FadeOutAndDestroy()
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









    //[Header("Gravity & Fall Settings")]
    //public float customGravity = -35f;
    //public float maxFallSpeed = 50f;

    //[Header("Rotation While Falling")]
    //public float baseAngularSpeed = 3f;
    //public float angularRandomness = 2f;
    //public float maxAngularVelocity = 8f;

    //[Header("Bounce Settings")]
    //public float bounceForce = 7f;
    //public float bounceRandomness = 1.5f;
    //public float bounceUpwardFactor = 1.0f; // Hệ số chỉnh độ hướng lên

    //[Header("Fade Settings")]
    //public float fadeDelay = 0.1f;
    //public float fadeDuration = 1.0f;

    //private Rigidbody rb;
    //private bool hasLanded = false;
    //private Material mat;

    //void Start()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    rb.useGravity = false;
    //    rb.drag = 0f;
    //    rb.angularDrag = 0.1f;
    //    rb.mass = 0.4f;
    //    rb.interpolation = RigidbodyInterpolation.Interpolate;
    //    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

    //    // Random angular velocity để vật thể xoay nhẹ
    //    Vector3 spin = new Vector3(
    //        Random.Range(-1f, 1f),
    //        Random.Range(-0.5f, 0.5f),
    //        Random.Range(-1f, 1f)
    //    ).normalized * (baseAngularSpeed + Random.Range(0f, angularRandomness));

    //    rb.angularVelocity = spin;

    //    // Get material for fade
    //    Renderer rend = GetComponentInChildren<Renderer>();
    //    if (rend != null)
    //    {
    //        mat = rend.material;
    //        Color color = mat.color;
    //        color.a = 1f;
    //        mat.color = color;
    //    }
    //}

    //void FixedUpdate()
    //{
    //    if (!hasLanded)
    //    {
    //        // Apply custom gravity
    //        rb.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);

    //        // Limit fall speed
    //        if (rb.velocity.magnitude > maxFallSpeed)
    //            rb.velocity = rb.velocity.normalized * maxFallSpeed;

    //        // Clamp angular velocity
    //        if (rb.angularVelocity.magnitude > maxAngularVelocity)
    //            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity;
    //    }
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (hasLanded) return;

    //    // Va vào ly hoặc Player → dừng liền
    //    if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Cup"))
    //    {
    //        rb.velocity = Vector3.zero;
    //        rb.angularVelocity = Vector3.zero;
    //        rb.isKinematic = true;
    //        FadeOutAndDestroy();
    //        return;
    //    }

    //    hasLanded = true;

    //    // Tính lực nảy bật lên
    //    float bounceStrength = bounceForce + Random.Range(-bounceRandomness, bounceRandomness);
    //    Vector3 bounceDir = Vector3.up * bounceStrength * bounceUpwardFactor;

    //    rb.velocity = bounceDir;

    //    // Giảm xoay sau khi nảy
    //    rb.angularVelocity = Random.insideUnitSphere * (baseAngularSpeed / 2f);
    //    rb.drag = 2f;
    //    rb.angularDrag = 3f;

    //    // Nhẹ nhàng hơn sau khi chạm
    //    rb.mass = 0.1f;

    //    Invoke(nameof(FadeOutAndDestroy), fadeDelay);
    //}

    //void FadeOutAndDestroy()
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
}
