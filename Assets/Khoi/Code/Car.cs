using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Car Settings")]
    public float moveSpeed = 10f;               // Tốc độ di chuyển cơ bản
    public float boostedSpeed = 20f;            // Tốc độ khi nhấn Shift
    public float turnSpeed = 50f;               // Tốc độ xoay khi rẽ trái/phải
    public float acceleration = 5f;             // Thời gian để đạt đến tốc độ mong muốn
    public float turnSmoothTime = 0.2f;         // Thời gian làm mượt khi chuyển hướng

    [Header("Wheels")]
    public Transform frontLeftWheel;            // Transform của bánh trước bên trái
    public Transform frontRightWheel;           // Transform của bánh trước bên phải
    public Transform rearLeftWheel;             // Transform của bánh sau bên trái
    public Transform rearRightWheel;            // Transform của bánh sau bên phải

    [Header("Steering Settings")]
    public Transform frontLeftSteerPivot;       // Pivot để rẽ của bánh trước trái
    public Transform frontRightSteerPivot;      // Pivot để rẽ của bánh trước phải
    public float maxSteerAngle = 30f;           // Góc rẽ tối đa của bánh trước
    public float wheelSpinSpeed = 360f;         // Tốc độ quay của bánh xe

    private float currentSpeed;                 // Tốc độ hiện tại của xe
    private float velocity = 0f;                // Biến tạm dùng cho SmoothDamp
    private bool isBoosting = false;            // Kiểm tra có đang tăng tốc không
    private Rigidbody rb;                       // Rigidbody của xe

    private void Start()
    {
        rb = GetComponent<Rigidbody>();         // Lấy component Rigidbody
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Mượt khi di chuyển
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Tránh xuyên collider
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Không bị xoay lệch trục
    }

    private void Update()
    {
        HandleMovement();   // Điều khiển xe
        RotateWheels();     // Xoay bánh xe
        SteerWheels();      // Rẽ bánh xe
    }

    private void FixedUpdate()
    {
        // Giữ cho xe luôn thăng bằng, không nghiêng
        Vector3 euler = transform.eulerAngles;
        euler.z = 0;
        euler.x = 0;
        transform.eulerAngles = euler;

        // Gắn xe về sát mặt đất nếu đang hơi bay
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, hit.point.y + 0.1f, 10f * Time.fixedDeltaTime); // Giữ cao 0.1f so với mặt đất
                transform.position = pos;
            }
        }
    }

    void HandleMovement()
    {
        float vertical = Input.GetAxis("Vertical"); // Lấy phím W/S
        float horizontal = Input.GetAxis("Horizontal"); // Lấy phím A/D

        // Kiểm tra có đang nhấn boost không
        isBoosting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float targetSpeed = vertical * (isBoosting ? boostedSpeed : moveSpeed);

        // Làm mượt tốc độ
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocity, acceleration * Time.deltaTime);

        // Xoay xe theo hướng
        transform.Rotate(Vector3.up, horizontal * turnSpeed * Time.deltaTime);

        // Di chuyển về phía trước
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    void RotateWheels()
    {
        float rotationAmount = currentSpeed * wheelSpinSpeed * Time.deltaTime;

        // Quay bánh xe
        frontLeftWheel.Rotate(Vector3.right, rotationAmount);
        frontRightWheel.Rotate(Vector3.right, rotationAmount);
        rearLeftWheel.Rotate(Vector3.right, rotationAmount);
        rearRightWheel.Rotate(Vector3.right, rotationAmount);
    }

    void SteerWheels()
    {
        float horizontal = Input.GetAxis("Horizontal"); // Lấy phím A/D
        float steerAngle = maxSteerAngle * horizontal;

        // Rẽ bánh trước
        Quaternion targetRotation = Quaternion.Euler(0, steerAngle, 0);
        frontLeftSteerPivot.localRotation = Quaternion.Lerp(frontLeftSteerPivot.localRotation, targetRotation, Time.deltaTime * 5f);
        frontRightSteerPivot.localRotation = Quaternion.Lerp(frontRightSteerPivot.localRotation, targetRotation, Time.deltaTime * 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Va chạm mặt đất, reset lực nếu cần
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Chặn rớt xuống
        }
    }
}
