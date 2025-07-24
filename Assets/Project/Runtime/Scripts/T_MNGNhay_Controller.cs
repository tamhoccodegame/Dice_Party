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

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = Vector3.up * jumpForce;
        }

        //// Tăng trọng lực khi rơi xuống
        //if (rb.velocity.y < 0)
        //{
        //    rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        //}
    }
}
