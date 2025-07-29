using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltAI : MonoBehaviour
{
    [Header("Tilt Settings")]
    public float maxTilt = 20f;          // Độ nghiêng tối đa (độ)
    public float tiltLerpSpeed = 1.5f;   // Tốc độ mượt nghiêng
    public float holdTimeMin = 1f;       // Thời gian giữ nghiêng min
    public float holdTimeMax = 3f;       // Thời gian giữ nghiêng max
    public float jitterStrength = 2f;    // Độ rung nhẹ giả tự nhiên

    [Header("Board Limit Settings")]
    public Transform horizontalRoller;   // Roller ngang
    public Transform verticalRoller;     // Roller dọc
    public Vector2 boardSize;            // Kích thước board (x = ngang, y = dọc)
    public float edgeThreshold = 0.8f;   // Ngưỡng % để coi là "gần cạnh" (0.8 = 80%)

    // Vector tilt [-1,1] (x = trái/phải, y = trước/sau)
    public Vector2 TiltInput { get; private set; }

    private Vector3 targetTilt;
    private float holdTimer;
    private float currentHoldTime;

    // Để tránh lặp lại hướng cũ
    private Vector3 lastTargetTilt;

    void Start()
    {
        SetNewTargetTilt(forceRandom: true);
    }

    void Update()
    {
        // Nếu Roller gần cạnh → đổi hướng ngay lập tức
        if (IsRollerNearEdge(horizontalRoller, "Horizontal") || IsRollerNearEdge(verticalRoller, "Vertical"))
        {
            SetNewTargetTilt(forceRandom: true);
        }

        // Nghiêng mượt về target
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            Quaternion.Euler(targetTilt),
            Time.deltaTime * tiltLerpSpeed
        );

        // Đếm thời gian giữ
        holdTimer += Time.deltaTime;
        if (holdTimer >= currentHoldTime)
        {
            SetNewTargetTilt(forceRandom: false);
        }

        UpdateTiltInput();
    }

    void SetNewTargetTilt(bool forceRandom)
    {
        holdTimer = 0f;
        currentHoldTime = Random.Range(holdTimeMin, holdTimeMax);

        Vector3 baseTilt;

        // Nếu bắt buộc đổi khác hướng cũ → tránh lặp
        do
        {
            baseTilt = new Vector3(
                Random.Range(-maxTilt, maxTilt),
                0,
                Random.Range(-maxTilt, maxTilt)
            );
        }
        while (!forceRandom && Vector3.Angle(baseTilt, lastTargetTilt) < 25f);

        // Thêm jitter nhỏ
        Vector3 jitter = new Vector3(
            Random.Range(-jitterStrength, jitterStrength),
            0,
            Random.Range(-jitterStrength, jitterStrength)
        );

        targetTilt = baseTilt + jitter;
        lastTargetTilt = targetTilt;

        // Log debug hướng mới
        Debug.Log($"[TiltAI] Đổi hướng nghiêng mới: {targetTilt}");
    }

    bool IsRollerNearEdge(Transform roller, string rollerName)
    {
        if (roller == null) return false;

        // Tính local pos của roller so với board
        Vector3 localPos = transform.InverseTransformPoint(roller.position);

        // Giới hạn board
        float halfX = boardSize.x * 0.5f;
        float halfZ = boardSize.y * 0.5f;

        // Kiểm tra gần cạnh
        bool nearX = Mathf.Abs(localPos.x) > halfX * edgeThreshold;
        bool nearZ = Mathf.Abs(localPos.z) > halfZ * edgeThreshold;

        if (nearX || nearZ)
        {
            Debug.Log($"[TiltAI] Roller {rollerName} gần cạnh! LocalPos = {localPos}");
            return true;
        }

        return false;
    }

    void UpdateTiltInput()
    {
        // Lấy Euler hiện tại
        Vector3 euler = transform.localEulerAngles;

        // Convert [-180,180]
        if (euler.x > 180) euler.x -= 360;
        if (euler.z > 180) euler.z -= 360;

        // Map sang [-1,1]
        float tiltX = Mathf.Clamp(euler.z / maxTilt, -1f, 1f);
        float tiltY = Mathf.Clamp(euler.x / maxTilt, -1f, 1f);

        TiltInput = new Vector2(tiltX, tiltY);
    }

    // Debug Gizmos: vẽ vùng gần cạnh
    void OnDrawGizmosSelected()
    {
        // Vẽ vùng board
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(boardSize.x, 0.1f, boardSize.y));

        // Vẽ vùng cảnh báo (80% board)
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(boardSize.x * edgeThreshold, 0.1f, boardSize.y * edgeThreshold));
    }

    //[Header("Tilt Settings")]
    //public float maxTilt = 20f;          // Độ nghiêng tối đa (độ)
    //public float tiltLerpSpeed = 1.5f;   // Tốc độ mượt nghiêng

    //// Vector tilt [-1,1] (x = trái/phải, y = trước/sau)
    //public Vector2 TiltInput { get; private set; }

    //private Vector3 targetTilt;

    //void Start()
    //{
    //    SetNewTargetTilt();
    //}

    //void Update()
    //{
    //    // Nghiêng mượt về target
    //    transform.localRotation = Quaternion.Slerp(
    //        transform.localRotation,
    //        Quaternion.Euler(targetTilt),
    //        Time.deltaTime * tiltLerpSpeed
    //    );

    //    // Nếu gần đạt → random tilt mới
    //    if (Quaternion.Angle(transform.localRotation, Quaternion.Euler(targetTilt)) < 1f)
    //        SetNewTargetTilt();

    //    UpdateTiltInput();
    //}

    //void SetNewTargetTilt()
    //{
    //    targetTilt = new Vector3(
    //        Random.Range(-maxTilt, maxTilt),
    //        0,
    //        Random.Range(-maxTilt, maxTilt)
    //    );
    //}

    //void UpdateTiltInput()
    //{
    //    // Lấy Euler hiện tại của board
    //    Vector3 euler = transform.localEulerAngles;

    //    // Chuyển về [-180,180]
    //    if (euler.x > 180) euler.x -= 360;
    //    if (euler.z > 180) euler.z -= 360;

    //    // Map sang [-1,1]
    //    float tiltX = Mathf.Clamp(euler.z / maxTilt, -1f, 1f);
    //    float tiltY = Mathf.Clamp(euler.x / maxTilt, -1f, 1f);

    //    TiltInput = new Vector2(tiltX, tiltY);
    //}

    //[Header("Arena Tilt Settings")]
    //public float tiltAngle = 20f;      // Độ nghiêng tối đa
    //public float tiltSpeed = 2f;       // Tốc độ nghiêng
    //public float holdTime = 1f;        // Thời gian giữ mỗi hướng
    //public float jitterStrength = 0.2f; // Nhiễu nhẹ (tự nhiên)

    //[Header("References")]
    //public Rigidbody horizontalRoller; // Thanh ngang
    //public Rigidbody verticalRoller;   // Thanh dọc

    //[Header("Physics")]
    //public float extraDownForce = 30f; // Lực ép roller xuống board

    //private Vector2[] directions = new Vector2[]
    //{
    //    Vector2.right,   // → phải
    //    Vector2.up,      // ↑ trên
    //    Vector2.left,    // ← trái
    //    Vector2.down     // ↓ dưới
    //};

    //private int currentIndex = 0;
    //private float timer = 0f;

    //void Start()
    //{
    //    Collider c1 = horizontalRoller.GetComponent<Collider>();
    //    Collider c2 = verticalRoller.GetComponent<Collider>();
    //}
    //void FixedUpdate()
    //{
    //    // 1. Đếm thời gian → đổi hướng
    //    timer += Time.fixedDeltaTime;
    //    if (timer >= holdTime)
    //    {
    //        currentIndex = (currentIndex + 1) % directions.Length;
    //        timer = 0f;
    //    }

    //    // 2. Hướng tilt hiện tại
    //    Vector2 tiltDirection = directions[currentIndex];

    //    // 3. Thêm jitter nhẹ
    //    Vector2 noise = new Vector2(
    //        Random.Range(-jitterStrength, jitterStrength),
    //        Random.Range(-jitterStrength, jitterStrength)
    //    );
    //    Vector2 finalDir = (tiltDirection + noise).normalized;

    //    // 4. Tính góc tilt
    //    Vector3 targetEuler = new Vector3(-finalDir.y * tiltAngle, 0f, finalDir.x * tiltAngle);
    //    Quaternion targetRot = Quaternion.Euler(targetEuler);

    //    // 5. Nghiêng board mượt
    //    transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, tiltSpeed * Time.fixedDeltaTime);

    //    // 6. Thêm lực ép xuống để roller không bay
    //    Vector3 downForce = -transform.up * extraDownForce;
    //    horizontalRoller.AddForce(downForce, ForceMode.Acceleration);
    //    verticalRoller.AddForce(downForce, ForceMode.Acceleration);
    //}
}
