using UnityEngine;
using UnityEngine.InputSystem;

public class T_MNGNhay_Controller : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rb;

    public float jumpForce = 12f;
    public float maxJumpTime = 0.3f;
    public float fallMultiplier = 10f;
    public float lowJumpMultiplier = 5f;
    private float jumpTimeCounter;
    private bool isJumping;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;
    private bool isGrounded;

    public void SetPlayerInput(PlayerInput input)
    {
        this.playerInput = input;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (playerInput == null) return;

        isGrounded = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer).Length > 0;

        float triggerValue = playerInput.actions["Jump"].ReadValue<float>();

        // Nếu nhấn trigger và đang đứng dưới đất => nhảy lên
        if (triggerValue > 0.1f && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

        // Nếu đang nhảy và vẫn giữ trigger => tiếp tục nhảy
        if (triggerValue > 0.1f && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // Nếu buông trigger => dừng nhảy
        if (triggerValue <= 0.1f)
        {
            isJumping = false;
        }

        // Tăng trọng lực khi rơi xuống
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Nếu nhảy nhưng thả nút sớm => tăng trọng lực sớm hơn (low jump)
        else if (rb.velocity.y > 0 && !isJumping)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
