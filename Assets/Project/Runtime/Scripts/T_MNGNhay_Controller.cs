using UnityEngine;
using UnityEngine.InputSystem;

public class T_MNGNhay_Controller : PlayerController
{
    private PlayerInput playerInput;
    private Rigidbody rb;

    public float jumpForce = 12f;
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
    }

    public override PlayerInput GetPlayerInput()
    {
        return playerInput;
    }
}
