using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class MNGPlayerController : PlayerController
{
    public bool canJump;
    bool autoRun = false;

    private CharacterController controller;
    private Animator animator;

    public VisualEffect bloodEffect;

    public string currentAnim;

    private Vector2 movementInput;
    public Vector3 movement;

    private float verticalVelocity = 0f;
    public float gravity = -20f;
    public float jumpForce = 10f;
    public float moveSpeed = 8f;

    public float speedFactor = 1;

    private bool isGrounded;
    public bool isFalling = false;

    public PlayerInput playerInput;

    public override PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    public override void SetInput(PlayerInput input)
    {
        playerInput = input;
    }

    public virtual void Awake()
    {
        if (bloodEffect != null)
            bloodEffect.Stop();

        //WizardMiniGameManager.instance.playerObjects.Add(playerInput, gameObject);
        
    }

    private void OnEnable()
    {
        if (bloodEffect != null)
            bloodEffect.Stop();
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        
    }

    protected virtual void Update()
    {
        if (playerInput == null) return;

        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);

        if (camForward.sqrMagnitude < 0.01f)
        {
            // Camera đang gần như nhìn thẳng xuống
            camForward = Camera.main.transform.up;
            camForward.y = 0;
        }

        camForward.Normalize();

        Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;

        movement = Vector3.zero;

        if (!isFalling)
        {
            if (WizardMiniGameManager.instance.isGameStarted)
                movementInput = playerInput.actions["Move"].ReadValue<Vector2>();

            if (!autoRun)
                movement = camForward * movementInput.y + camRight * movementInput.x;
            else
                movement = transform.forward * 0.8f;
        }

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // giữ cho player dính mặt đất
        }

        // Jump khi bấm Trigger
        if (playerInput.actions["Trigger"].triggered && isGrounded && canJump)
        {
            if (!WizardMiniGameManager.instance.isGameStarted) return;
            verticalVelocity = jumpForce;
            ChangeAnim("Jump");
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        movement.y = verticalVelocity;

        // Move
        controller.Move(movement * (moveSpeed * speedFactor) * Time.deltaTime);
        // Ground check
        isGrounded = controller.isGrounded;

        // Rotate theo hướng di chuyển (chỉ trên mặt phẳng ngang)
        Vector3 horizontalMove = new Vector3(movement.x, 0, movement.z);
        if (horizontalMove.magnitude > 0.1f)
        {
            Quaternion newRotation = Quaternion.LookRotation(horizontalMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 5 * Time.deltaTime);
        }

        // Animation
        if (isGrounded && !isFalling)
        {
            if (horizontalMove.magnitude > 0.1f)
                ChangeAnim("Run");
            else
                ChangeAnim("Idle");
        }
    }

    public void ChangeAnim(string animName, float blendTime = 0.25f)
    {
        if (animName == currentAnim) return;
        currentAnim = animName;

        animator.CrossFade(animName, blendTime);
    }

    public void BloodEffect()
    {
        bloodEffect.Play();
    }

    public void MoveForward()
    {
        autoRun = true;
    }

    public void StopMove()
    {
        autoRun = false;
    }

    void EnableRagdoll()
    {
        GetComponent<Ragdoll>().EnableRagdoll();
    }

}
