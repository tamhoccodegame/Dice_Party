using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpHeight = 1.2f;
    public float gravity = -30f;
    public float fallMultiplier = 3.5f;
    public float accelerationTime = 0.07f;
    public float rotationSpeed = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("Jump Assist")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    private CharacterController controller;
    private Animator animator;
    private Transform cam;

    private Vector3 horizontalVelocity;
    private Vector3 velocitySmooth;
    private float verticalVelocity;

    private Vector2 currentAnimVelocity;
    private float velocityXSmooth;
    private float velocityZSmooth;

    private bool isGrounded;
    private bool wasGrounded;
    private bool isJumping;

    private float coyoteTimer;
    private float jumpBufferTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        cam = Camera.main.transform;

        if (!animator) Debug.LogError("Animator not found!");
        if (!cam) Debug.LogError("Main Camera not tagged or missing!");
    }

    void Update()
    {
        UpdateGroundCheck();

        // Buffer input Jump
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        UpdateCoyoteTime();
        HandleJump();

        HandleMovement();
        HandleRotation();
        HandleAnimation();
    }

    void UpdateGroundCheck()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
            isJumping = false;

            if (!wasGrounded)
                animator.ResetTrigger("isJump");
        }
    }

    void UpdateCoyoteTime()
    {
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;
    }

    void HandleJump()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f && !isJumping)
        {
            isJumping = true;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("isJump");
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;
        moveDir.Normalize();

        float targetSpeed = (isRunning && inputDir != Vector3.zero) ? runSpeed : walkSpeed;
        Vector3 targetVelocity = moveDir * targetSpeed;

        horizontalVelocity = Vector3.SmoothDamp(horizontalVelocity, targetVelocity, ref velocitySmooth, accelerationTime);

        // Gravity
        if (!isGrounded)
        {
            float gravityMultiplier = (verticalVelocity < 0f) ? fallMultiplier : 1f;
            verticalVelocity += gravity * gravityMultiplier * Time.deltaTime;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 finalMove = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(finalMove * Time.deltaTime);
    }

    void HandleRotation()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 targetDir = camForward * inputDir.z + camRight * inputDir.x;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleAnimation()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(horizontalVelocity);
        float normalizedX = Mathf.SmoothDamp(currentAnimVelocity.x, localVelocity.x / runSpeed, ref velocityXSmooth, 0.05f);
        float normalizedZ = Mathf.SmoothDamp(currentAnimVelocity.y, localVelocity.z / runSpeed, ref velocityZSmooth, 0.05f);

        currentAnimVelocity = new Vector2(normalizedX, normalizedZ);

        animator.SetFloat("VelocityX", currentAnimVelocity.x);
        animator.SetFloat("VelocityZ", currentAnimVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", verticalVelocity);
    }


    //[Header("Movement Settings")]
    //public float walkSpeed = 2f;
    //public float runSpeed = 5f;
    //public float jumpHeight = 1.5f;
    //public float gravity = -9.81f;
    //public float accelerationTime = 0.1f;
    //public float rotationSpeed = 10f;

    //[Header("Ground Check")]
    //public Transform groundCheck;
    //public float groundDistance = 0.3f;
    //public LayerMask groundMask;

    //private CharacterController controller;
    //private Animator animator;
    //private Transform cam;

    //private Vector3 velocity;
    //private Vector3 velocitySmooth;
    //private float verticalVelocity;

    //private Vector2 currentAnimVelocity;
    //private float velocityXSmooth;
    //private float velocityZSmooth;

    //private bool isGrounded;
    //private bool wasGrounded;
    //private bool isJumping;

    //void Start()
    //{
    //    controller = GetComponent<CharacterController>();
    //    animator = GetComponentInChildren<Animator>();
    //    cam = Camera.main.transform;

    //    if (animator == null) Debug.LogError("Animator not found!");
    //    if (cam == null) Debug.LogError("Main Camera not tagged or missing!");
    //}

    //void Update()
    //{
    //    UpdateGroundCheck();
    //    HandleMovement();
    //    HandleRotation();
    //    HandleAnimation();
    //}

    //void UpdateGroundCheck()
    //{
    //    wasGrounded = isGrounded;
    //    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    //    if (isGrounded && verticalVelocity < 0)
    //    {
    //        verticalVelocity = -2f;
    //        isJumping = false;

    //        // Nếu vừa mới chạm đất
    //        if (!wasGrounded)
    //        {
    //            animator.ResetTrigger("isJump");
    //        }
    //    }
    //}

    //void HandleMovement()
    //{
    //    float horizontal = Input.GetAxisRaw("Horizontal");
    //    float vertical = Input.GetAxisRaw("Vertical");
    //    bool isRunning = Input.GetKey(KeyCode.LeftShift);
    //    bool isJumpPressed = Input.GetKeyDown(KeyCode.Space);

    //    Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

    //    Vector3 camForward = cam.forward;
    //    Vector3 camRight = cam.right;
    //    camForward.y = 0;
    //    camRight.y = 0;
    //    camForward.Normalize();
    //    camRight.Normalize();

    //    Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;
    //    moveDir.Normalize();

    //    float targetSpeed = (isRunning && inputDir != Vector3.zero) ? runSpeed : walkSpeed;
    //    Vector3 targetVelocity = moveDir * targetSpeed;

    //    velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref velocitySmooth, accelerationTime);

    //    // Jump
    //    if (isGrounded && isJumpPressed && !isJumping)
    //    {
    //        isJumping = true;
    //        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    //        animator.SetTrigger("isJump"); // Nhảy vào JumpStart
    //    }

    //    // Gravity
    //    verticalVelocity += gravity * Time.deltaTime;

    //    Vector3 finalMove = velocity + Vector3.up * verticalVelocity;
    //    controller.Move(finalMove * Time.deltaTime);
    //}

    ////void HandleRotation()
    ////{
    ////    Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);
    ////    if (flatVelocity.magnitude > 0.1f)
    ////    {
    ////        Quaternion targetRotation = Quaternion.LookRotation(flatVelocity);
    ////        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    ////    }
    ////}

    //void HandleRotation()
    //{
    //    float horizontal = Input.GetAxisRaw("Horizontal");
    //    float vertical = Input.GetAxisRaw("Vertical");

    //    Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

    //    if (inputDir.magnitude >= 0.1f)
    //    {
    //        Vector3 camForward = cam.forward;
    //        Vector3 camRight = cam.right;
    //        camForward.y = 0f;
    //        camRight.y = 0f;
    //        camForward.Normalize();
    //        camRight.Normalize();

    //        Vector3 targetDirection = camForward * inputDir.z + camRight * inputDir.x;

    //        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    //    }
    //}


    //void HandleAnimation()
    //{
    //    // Blend cho 2DMoveBlendTree
    //    Vector3 localVelocity = transform.InverseTransformDirection(velocity);
    //    float normalizedX = Mathf.SmoothDamp(currentAnimVelocity.x, localVelocity.x / runSpeed, ref velocityXSmooth, 0.1f);
    //    float normalizedZ = Mathf.SmoothDamp(currentAnimVelocity.y, localVelocity.z / runSpeed, ref velocityZSmooth, 0.1f);

    //    currentAnimVelocity = new Vector2(normalizedX, normalizedZ);

    //    animator.SetFloat("VelocityX", currentAnimVelocity.x);
    //    animator.SetFloat("VelocityZ", currentAnimVelocity.y);

    //    // Animation Jump các state riêng
    //    animator.SetBool("IsGrounded", isGrounded);
    //    animator.SetFloat("VerticalSpeed", verticalVelocity);
    //}

}
