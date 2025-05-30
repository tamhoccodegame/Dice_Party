using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MNGVongXoayController : NetworkBehaviour
{
    private CharacterController controller;
    private Animator animator;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float verticalVelocity;

    [Networked] private Vector2 moveInput { get; set; }
    [Networked] private bool jumpRequest { get; set; }

    public string currentAnim;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!Object.HasInputAuthority) return;

        // Collect input on client
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool jump = Input.GetKeyDown(KeyCode.Space);

        // Send input to host
        RPC_SendInput(input, jump);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SendInput(Vector2 input, bool jump)
    {
        moveInput = input;
        if (jump) jumpRequest = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Runner.DeltaTime;
        }

        // Jump
        if (jumpRequest && controller.isGrounded)
        {
            ChangeAnim("Jump");
            verticalVelocity = jumpForce;
        }
        jumpRequest = false; // reset jump request

        // Movement
        Vector3 movement = new Vector3(moveInput.x, verticalVelocity, moveInput.y);
        controller.Move(movement * moveSpeed * Runner.DeltaTime);

        // Rotation
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        if (moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
        }

        // Animation
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
        if (VongXoayManager.instance.isGameOver) return;

        if (Object.HasInputAuthority)
            VongXoayManager.instance.RequestUpdateLive(Runner.LocalPlayer);
    }
}
