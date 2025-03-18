using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float strafeSpeed = 5f; // Tốc độ di chuyển ngang
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float rotationSpeed = 200f;
    public Transform cameraTransform; // Kéo camera vào đây nếu bạn muốn di chuyển theo hướng camera

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("PlayerMovement script requires a CharacterController component on the same GameObject.");
            enabled = false;
            return;
        }

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            if (cameraTransform == null)
            {
                Debug.LogWarning("CameraTransform not assigned. Player movement might not be relative to camera direction.");
            }
        }
    }

    void Update()
    {
        // Kiểm tra xem Player có đang chạm đất không
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Đặt lại vận tốc rơi khi chạm đất
        }

        // Lấy input cho di chuyển
        float forwardInput = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);
        float horizontalInput = Input.GetKey(KeyCode.D) ? 1f : (Input.GetKey(KeyCode.A) ? -1f : 0f);

        Vector3 moveDirection = Vector3.zero;

        // Tính toán hướng di chuyển dựa trên input và hướng hiện tại của Player
        moveDirection += transform.forward * forwardInput;
        moveDirection += transform.right * horizontalInput;

        moveDirection = moveDirection.normalized * moveSpeed * Time.deltaTime;
        characterController.Move(moveDirection);

        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Áp dụng trọng lực
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Xoay người dựa trên phím A và D (nếu không di chuyển lên/xuống)
        if (forwardInput == 0)
        {
            if (horizontalInput > 0)
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
            else if (horizontalInput < 0)
            {
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Nếu đang di chuyển lên/xuống, xoay theo hướng di chuyển (tùy chọn)
            if (horizontalInput != 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 10f); // Nhân thêm hệ số để xoay nhanh hơn
            }
        }
    }

    // Xử lý va chạm với các vật thể khác
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject hitObject = hit.gameObject;

        // Kiểm tra xem vật thể va chạm có component BreakableWindow không
        BreakableWindow breakableWindow = hitObject.GetComponent<BreakableWindow>();

        if (breakableWindow != null)
        {
            // Kiểm tra xem tile này có được đánh dấu là có thể vỡ (ví dụ: màu đỏ)
            MeshRenderer renderer = hitObject.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material != null && renderer.material.color == Color.red)
            {
                // Gọi hàm breakWindow trên component BreakableWindow
                breakableWindow.breakWindow();

                // Bạn có thể thêm hiệu ứng hoặc logic khác khi chạm vào kính vỡ ở đây
                Debug.Log("Player chạm vào kính vỡ!");
            }
            else if (breakableWindow != null)
            {
                // Nếu bạn muốn có hành động nào đó khi chạm vào kính không vỡ (ví dụ: dừng lại)
                // Bạn có thể thêm code ở đây
                Debug.Log("Player chạm vào kính không vỡ.");
            }
        }
    }
}