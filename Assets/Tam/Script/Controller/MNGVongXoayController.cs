using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MNGVongXoayController : NetworkBehaviour
{
    private CharacterController controller;
    private Animator animator;

    public float moveSpeed;
    public float rotationSpeed;
    public float jumpForce;
    public float gravity = -9.81f;
    public float verticalVelocity;

    private float horizontalInput;
    private float verticalInput;

    private bool isJumpPressed = false;
    public string currentAnim;

    private void Start()
    {
        
    }

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded && !isJumpPressed)
        {
            isJumpPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Đảm bảo dính mặt đất
        }
        else
        {
            verticalVelocity += gravity * Runner.DeltaTime;
        }

        // Jump
        if (isJumpPressed && controller.isGrounded)
        {
            ChangeAnim("Jump");
            verticalVelocity = jumpForce;
            isJumpPressed = false; // Reset sau khi nhảy
        }

       Vector3 movement = new Vector3(horizontalInput, verticalVelocity, verticalInput);

        controller.Move(movement * moveSpeed * Runner.DeltaTime);

        // Xoay hướng
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput);
        if (moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
        }

        if (controller.isGrounded)
        {
            if (moveDirection.magnitude > 0) ChangeAnim("Run");
            else ChangeAnim("Idle");
        }
    }

    public void ChangeAnim(string animName, float blendTime = 0.25f)
    {
        if (animName == currentAnim) return;
        currentAnim = animName;
        animator.CrossFade(animName, blendTime);
    }

    public void Die()
    {
        if(Object.HasInputAuthority)
        VongXoayManager.instance.RequestUpdateLive(Runner.LocalPlayer);
    }
}
