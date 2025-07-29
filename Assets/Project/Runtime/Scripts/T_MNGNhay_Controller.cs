//using UnityEngine;
//using UnityEngine.InputSystem;

//public class T_MNGNhay_Controller : PlayerController
//{
//    private PlayerInput playerInput;
//    private Rigidbody rb;
//    private Animator animator;

//    public float jumpForce = 12f;
//    public Transform groundCheck;
//    public LayerMask groundLayer;
//    public float groundCheckRadius = 0.1f;
//    private bool isGrounded;

//    public override void SetInput(PlayerInput input)
//    {
//        this.playerInput = input;
//        NhayLopManager.instance.playerObjects.Add(playerInput, gameObject);
//    }

//    void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        animator = GetComponent<Animator>();
//    }

//    void Update()
//    {
//        if (playerInput == null) return;

//        isGrounded = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer).Length > 0;

//        if (playerInput.actions["Trigger"].triggered && isGrounded)
//        {
//            rb.velocity = Vector3.up * jumpForce;
//            animator.Play("Jump");
//        }

//        if (isGrounded && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
//        {
//            animator.Play("Idle");
//        }
//    }

//    public override PlayerInput GetPlayerInput()
//    {
//        return playerInput;
//    }
//}
