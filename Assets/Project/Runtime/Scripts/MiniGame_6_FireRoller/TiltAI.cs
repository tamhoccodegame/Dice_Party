using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltAI : MonoBehaviour
{
    [Header("Arena Tilt Settings")]
    public float tiltAngle = 20f;      // Độ nghiêng tối đa
    public float tiltSpeed = 2f;       // Tốc độ nghiêng
    public float holdTime = 1f;        // Thời gian giữ mỗi hướng
    public float jitterStrength = 0.2f; // Nhiễu nhẹ (tự nhiên)

    [Header("References")]
    public Rigidbody horizontalRoller; // Thanh ngang
    public Rigidbody verticalRoller;   // Thanh dọc

    [Header("Physics")]
    public float extraDownForce = 30f; // Lực ép roller xuống board

    private Vector2[] directions = new Vector2[]
    {
        Vector2.right,   // → phải
        Vector2.up,      // ↑ trên
        Vector2.left,    // ← trái
        Vector2.down     // ↓ dưới
    };

    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        Collider c1 = horizontalRoller.GetComponent<Collider>();
        Collider c2 = verticalRoller.GetComponent<Collider>();

        if (c1 != null && c2 != null)
            Physics.IgnoreCollision(c1, c2, true);
    }


    void FixedUpdate()
    {
        // 1. Đếm thời gian → đổi hướng
        timer += Time.fixedDeltaTime;
        if (timer >= holdTime)
        {
            currentIndex = (currentIndex + 1) % directions.Length;
            timer = 0f;
        }

        // 2. Hướng tilt hiện tại
        Vector2 tiltDirection = directions[currentIndex];

        // 3. Thêm jitter nhẹ
        Vector2 noise = new Vector2(
            Random.Range(-jitterStrength, jitterStrength),
            Random.Range(-jitterStrength, jitterStrength)
        );
        Vector2 finalDir = (tiltDirection + noise).normalized;

        // 4. Tính góc tilt
        Vector3 targetEuler = new Vector3(-finalDir.y * tiltAngle, 0f, finalDir.x * tiltAngle);
        Quaternion targetRot = Quaternion.Euler(targetEuler);

        // 5. Nghiêng board mượt
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, tiltSpeed * Time.fixedDeltaTime);

        // 6. Thêm lực ép xuống để roller không bay
        Vector3 downForce = -transform.up * extraDownForce;
        horizontalRoller.AddForce(downForce, ForceMode.Acceleration);
        verticalRoller.AddForce(downForce, ForceMode.Acceleration);
    }
}
