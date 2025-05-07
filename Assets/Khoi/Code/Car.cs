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

    private float currentSpeed;                 // Tốc độ hiện tại của xe (mượt hơn)
    private float velocity = 0f;                // Biến tạm dùng cho SmoothDamp
    private bool isBoosting = false;            // Cờ kiểm tra có đang tăng tốc không
    private Rigidbody rb;                       // Rigidbody của xe

    private void Start()
    {
        // Gán rigidbody từ object xe
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleMovement();   // Điều khiển xe
        RotateWheels();     // Xoay bánh xe
        SteerWheels();      // Rẽ bánh xe
    }

    void HandleMovement()
    {
        // Nhận input tăng/giảm tốc độ (W/S)
        float vertical = Input.GetAxis("Vertical");

        // Kiểm tra nếu nhấn Shift thì tăng tốc
        isBoosting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float targetSpeed = vertical * (isBoosting ? boostedSpeed : moveSpeed);

        // Làm mượt tốc độ xe
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocity, acceleration * Time.deltaTime);

        // Nhận input rẽ trái/phải (A/D)
        float horizontal = Input.GetAxis("Horizontal");

        // Xoay thân xe
        transform.Rotate(Vector3.up, horizontal * turnSpeed * Time.deltaTime);

        // Di chuyển xe theo hướng đang quay
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

       
    }

    void RotateWheels()
    {
        // Tính toán góc quay của bánh xe dựa theo tốc độ hiện tại
        float rotationAmount = currentSpeed * wheelSpinSpeed * Time.deltaTime;

        // Xoay từng bánh xe theo chiều chạy
        frontLeftWheel.Rotate(Vector3.right, rotationAmount);
        frontRightWheel.Rotate(Vector3.right, rotationAmount);
        rearLeftWheel.Rotate(Vector3.right, rotationAmount);
        rearRightWheel.Rotate(Vector3.right, rotationAmount);
    }

    void SteerWheels()
    {
        // Lấy input rẽ trái/phải (A/D)
        float horizontal = Input.GetAxis("Horizontal");

        // Tính góc cần rẽ
        float steerAngle = maxSteerAngle * horizontal;

        // Tạo góc quay mượt cho trục bánh trước
        Quaternion targetRotation = Quaternion.Euler(0, steerAngle, 0);
        frontLeftSteerPivot.localRotation = Quaternion.Lerp(frontLeftSteerPivot.localRotation, targetRotation, Time.deltaTime * 5f);
        frontRightSteerPivot.localRotation = Quaternion.Lerp(frontRightSteerPivot.localRotation, targetRotation, Time.deltaTime * 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Lấy hướng va chạm trung bình từ các điểm tiếp xúc
            Vector3 averageNormal = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts)
            {
                averageNormal += contact.normal;
            }
            averageNormal.Normalize();

            // Đẩy ngược theo mặt tiếp xúc, loại bỏ trục Y để không bị chìm
            Vector3 horizontalPushDir = Vector3.ProjectOnPlane(-averageNormal, Vector3.up).normalized;

            // Xác định lực đẩy dựa vào có boost hay không
            float forceMagnitude = isBoosting ? 20f : 10f;

            // Áp dụng lực đẩy về phía ngược lại
            rb.AddForce(horizontalPushDir * forceMagnitude, ForceMode.Impulse);
        }
    }

}
