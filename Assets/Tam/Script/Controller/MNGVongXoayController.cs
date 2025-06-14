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
    [Networked] private string NetworkAnim { get; set; } // Animation sync

    public string currentAnim;

    VongXoayManager manager;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
        manager = VongXoayManager.instance;
    }

    void Update()
    {
        if (!Object.HasInputAuthority) return;

        if (manager != null && manager.Object.IsValid && manager.isGameStarted)
        {
            // Collect input on client
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            bool jump = Input.GetKeyDown(KeyCode.Space);

            // Send input to host
            RPC_SendInput(input, jump);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SendInput(Vector2 input, bool jump)
    {
        moveInput = input;
        if (jump) jumpRequest = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
        {
            // Client đọc networked animation state
            if (NetworkAnim != currentAnim)
            {
                animator.CrossFade(NetworkAnim, 0.25f);
                currentAnim = NetworkAnim;
            }
            return;
        }

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
            if (moveDirection.magnitude > 0)
                ChangeAnim("Run");
            else
                ChangeAnim("Idle");
        }
    }

    public void ChangeAnim(string animName, float blendTime = 0.25f)
    {
        if (animName == currentAnim) return;
        currentAnim = animName;

        if (Object.HasStateAuthority)
            NetworkAnim = animName;

        animator.CrossFade(animName, blendTime);
    }

    public void Die()
    {
        if (VongXoayManager.instance.isGameOver) return;

        if (Object.HasInputAuthority)
        {
            VongXoayManager.instance.RequestUpdateLive(Runner.LocalPlayer);

            if (VongXoayManager.instance.playerLives.Get(Runner.LocalPlayer) <= 0)
            {
                RPC_EnableRagdoll();
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_EnableRagdoll()
    {
        GetComponent<Ragdoll>().EnableRagdoll();
    }
}
