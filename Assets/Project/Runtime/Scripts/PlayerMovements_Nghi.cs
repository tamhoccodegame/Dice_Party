using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements_Nghi : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpHeight = 1.2f;
    public float gravity = -30f;
    public float accelerationTime = 0.05f;
    public float rotationSpeed = 15f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRayLength = 0.45f;
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
    private bool hasTriggeredJumpAir;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private PlayerInteractMoneyController bagController;

    private bool IsMovementLocked => bagController != null && bagController.isFalling;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        cam = Camera.main.transform;

        bagController = GetComponent<PlayerInteractMoneyController>(); // 👈 lấy nếu có

        if (!animator) Debug.LogError("Animator not found!");
        if (!cam) Debug.LogError("Main Camera not found!");
    }

    void Update()
    {
        if (IsMovementLocked)
        {
            // vẫn cho apply gravity để nhân vật rớt tự nhiên
            verticalVelocity += gravity * Time.deltaTime;

            // Move chỉ với gravity (không nhận input ngang)
            Vector3 finalMove = Vector3.up * verticalVelocity;
            controller.Move(finalMove * Time.deltaTime);

            return; // ⛔ skip mọi xử lý input/rotation/animation
        }

        CacheInputs();
        UpdateGroundCheck();
        UpdateCoyoteTime();
        HandleJump();
        HandleMovement();
        HandleRotation();
        HandleAnimation();
    }

    void CacheInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
    }

    void UpdateGroundCheck()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundRayLength, groundMask);

        if (!wasGrounded && isGrounded)
        {
            Debug.Log("🟢 Landed - Trigger JumpEnd");

            verticalVelocity = -2f;
            isJumping = false;
            hasTriggeredJumpAir = false;

            animator.ResetTrigger("JumpStart");
            animator.ResetTrigger("JumpAir");
            animator.SetTrigger("JumpEnd");
        }
    }

    void UpdateCoyoteTime()
    {
        coyoteTimer = isGrounded ? coyoteTime : coyoteTimer - Time.deltaTime;
    }

    void HandleJump()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f && !isJumping)
        {
            isJumping = true;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.ResetTrigger("JumpEnd");
            animator.SetTrigger("JumpStart");

            Debug.Log("🔼 Jump Triggered");
        }

        // Áp lực trọng lực
        verticalVelocity += gravity * Time.deltaTime;
    }

    //void HandleMovement()
    //{
    //    float horizontal = Input.GetAxisRaw("Horizontal");
    //    float vertical = Input.GetAxisRaw("Vertical");
    //    bool isRunning = Input.GetKey(KeyCode.LeftShift);

    //    Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

    //    Vector3 camForward = cam.forward;
    //    Vector3 camRight = cam.right;
    //    camForward.y = 0f;
    //    camRight.y = 0f;
    //    camForward.Normalize();
    //    camRight.Normalize();

    //    Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;
    //    moveDir.Normalize();

    //    float targetSpeed = (isRunning && inputDir != Vector3.zero) ? runSpeed : walkSpeed;
    //    Vector3 targetVelocity = moveDir * targetSpeed;

    //    horizontalVelocity = Vector3.SmoothDamp(horizontalVelocity, targetVelocity, ref velocitySmooth, accelerationTime);

    //    Vector3 finalMove = horizontalVelocity + Vector3.up * verticalVelocity;
    //    controller.Move(finalMove * Time.deltaTime);


    //    // nhận số bag từ controller
    //    int bagCount = GetComponent<PlayerInteractMoneyController>().carriedBags.Count;

    //    // slow factor: mỗi bag giảm 20% tốc độ
    //    float carrySlowFactor = 1f - (0.2f * bagCount);
    //    carrySlowFactor = Mathf.Clamp(carrySlowFactor, 0.4f, 1f); // không cho chậm quá

    //    float targetSpeed_Bag = (isRunning && inputDir != Vector3.zero) ? runSpeed : walkSpeed;
    //    targetSpeed_Bag *= carrySlowFactor;
    //}

    void HandleMovement()
    {
        if (IsMovementLocked) return; // ⛔ không cho đi ngang

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

        // 👇 mặc định = 0 bag
        int bagCount = 0;

        // 👇 chỉ lấy nếu có bagController
        if (bagController != null)
            bagCount = bagController.carriedBags.Count;

        // 👇 base speed
        float baseSpeed = (isRunning && inputDir != Vector3.zero) ? runSpeed : walkSpeed;

        // 👇 factor theo số bag (opt-in)
        float[] bagSpeedFactor = { 1.3f, 1.0f, 0.7f, 0.4f };
        float carryFactor = bagSpeedFactor[Mathf.Clamp(bagCount, 0, bagSpeedFactor.Length - 1)];

        float targetSpeed = baseSpeed * carryFactor;


        // 👇 apply tốc độ
        Vector3 targetVelocity = moveDir * targetSpeed;

        horizontalVelocity = Vector3.SmoothDamp(horizontalVelocity, targetVelocity, ref velocitySmooth, accelerationTime);

        Vector3 finalMove = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(finalMove * Time.deltaTime);
    }


    void HandleRotation()
    {
        if (IsMovementLocked) return; // ⛔ không cho xoay

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
        animator.SetFloat("VerticalSpeed", verticalVelocity);
        animator.SetBool("IsGrounded", isGrounded);

        if (!isGrounded && verticalVelocity < 0f && isJumping && !hasTriggeredJumpAir)
        {
            animator.SetTrigger("JumpAir");
            hasTriggeredJumpAir = true;
        }


        int bagCount = 0;
        if (bagController != null)
            bagCount = bagController.carriedBags.Count;

        float[] animSpeedFactor = { 1.3f, 1.0f, 0.7f, 0.5f };
        animator.speed = animSpeedFactor[Mathf.Clamp(bagCount, 0, animSpeedFactor.Length - 1)];

    }
}
