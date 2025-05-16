using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour
{
    [Header("Car Settings")]
    public float moveSpeed = 10f;               // Tốc độ di chuyển cơ bản
    public float boostedSpeed = 20f;            // Tốc độ khi nhấn Shift
    public float turnSpeed = 50f;               // Tốc độ xoay
    public float acceleration = 5f;             // Thời gian làm mượt tốc độ

    [Header("Wheels")]
    public Transform frontLeftWheel;            // Bánh trước trái
    public Transform frontRightWheel;           // Bánh trước phải
    public Transform rearLeftWheel;             // Bánh sau trái
    public Transform rearRightWheel;            // Bánh sau phải

    [Header("Steering")]
    public Transform frontLeftSteerPivot;       // Pivot rẽ bánh trước trái
    public Transform frontRightSteerPivot;      // Pivot rẽ bánh trước phải
    public float maxSteerAngle = 30f;           // Góc rẽ tối đa
    public float wheelSpinSpeed = 360f;         // Tốc độ quay bánh

    private float currentSpeed = 0f;            // Tốc độ hiện tại
    private float velocity = 0f;                // Dùng cho SmoothDamp
    private Rigidbody rb;                       // Rigidbody của xe
    private bool isBoosting = false;            // Cờ kiểm tra tăng tốc

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Lấy Rigidbody

        // Chống xe bị nghiêng khi va chạm
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Làm mượt vật lý
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Ngăn vật thể xuyên qua ground khi di chuyển nhanh
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }


    private void Update()
    {
        HandleMovement();   // Điều khiển xe
        RotateWheels();     // Quay bánh xe
        SteerWheels();      // Rẽ bánh xe
    }

    void FixedUpdate()
    {
        // Xoá nghiêng trục Z và X mỗi khung
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z = 0;
        currentRotation.x = 0;
        transform.eulerAngles = currentRotation;

        // Giữ xe không chìm xuống: kiểm tra nếu xe dưới mặt đất thì đẩy lên
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                float distanceToGround = hit.distance;
                float minHeight = 0.5f; // khoảng cách tối thiểu từ thân xe đến mặt đất

                if (distanceToGround < minHeight)
                {
                    // Đẩy xe nhẹ lên để tránh chìm
                    rb.MovePosition(rb.position + Vector3.up * (minHeight - distanceToGround));
                }
            }
        }
    }


    void HandleMovement()
    {
        float vertical = Input.GetAxis("Vertical"); // Nhận input W/S
        float horizontal = Input.GetAxis("Horizontal"); // Nhận input A/D

        // Kiểm tra có tăng tốc không
        isBoosting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float targetSpeed = vertical * (isBoosting ? boostedSpeed : moveSpeed);

        // Làm mượt tốc độ thay đổi
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocity, acceleration * Time.deltaTime);

        // Di chuyển xe bằng MovePosition
        Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        // Tính góc xoay theo input A/D
        float turn = horizontal * turnSpeed * Time.deltaTime;

        // Tạo quaternion quay mới từ góc
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Quay xe bằng MoveRotation để không bị rung
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    void RotateWheels()
    {
        // Quay bánh xe dựa theo tốc độ
        float rotationAmount = currentSpeed * wheelSpinSpeed * Time.deltaTime;

        frontLeftWheel.Rotate(Vector3.right, rotationAmount);
        frontRightWheel.Rotate(Vector3.right, rotationAmount);
        rearLeftWheel.Rotate(Vector3.right, rotationAmount);
        rearRightWheel.Rotate(Vector3.right, rotationAmount);
    }

    void SteerWheels()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float steerAngle = maxSteerAngle * horizontal;

        // Rẽ bánh trước mượt
        Quaternion targetRotation = Quaternion.Euler(0, steerAngle, 0);
        frontLeftSteerPivot.localRotation = Quaternion.Lerp(frontLeftSteerPivot.localRotation, targetRotation, Time.deltaTime * 5f);
        frontRightSteerPivot.localRotation = Quaternion.Lerp(frontRightSteerPivot.localRotation, targetRotation, Time.deltaTime * 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Không làm gì - Rigidbody và Collider sẽ tự xử lý va chạm
            // Bạn có thể thêm hiệu ứng hoặc âm thanh tại đây nếu cần
        }
    }
}
