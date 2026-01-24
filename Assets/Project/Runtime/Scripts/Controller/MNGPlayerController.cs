using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class MNGPlayerController : PlayerController
{
    public bool canJump;
    bool canMove = true;

    private CharacterController controller;
    private Animator animator;

    public VisualEffect bloodEffect;

    public string currentAnim;

    private Vector2 movementInput;

    private float verticalVelocity = 0f;
    public float gravity = -20f;
    public float jumpForce = 10f;
    public float moveSpeed = 8f;

    public float speedFactor = 1;

    private bool isGrounded;
    public bool isFalling = false;

    protected PlayerInput playerInput;

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
        if(bloodEffect != null) 
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

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0f;

        Vector3 move = Vector3.zero;

        if (!isFalling)
        {
            if(canMove)
            movementInput = playerInput.actions["Move"].ReadValue<Vector2>();

            if(canMove)
            move = camForward * movementInput.y + camRight * movementInput.x;
            else 
            move = new Vector2(movementInput.x, movementInput.y);
        }

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // giữ cho player dính mặt đất
        }

        // Jump khi bấm Trigger
        if (playerInput.actions["Trigger"].triggered && isGrounded && canJump)
        {
            verticalVelocity = jumpForce;
            ChangeAnim("Jump");
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;

        // Move
        controller.Move(move * (moveSpeed * speedFactor) * Time.deltaTime);
        // Ground check
        isGrounded = controller.isGrounded;

        // Rotate theo hướng di chuyển (chỉ trên mặt phẳng ngang)
        Vector3 horizontalMove = new Vector3(move.x, 0, move.z);
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
        DisableInput();
        movementInput = new Vector2(1, 0);
    }

    public void StopMove()
    {
        movementInput = Vector2.zero;
        EnableInput();
    }

    void EnableInput()
    {
        canMove = true;
    }

    void DisableInput()
    {
        canMove = false;   
    }

    void EnableRagdoll()
    {
        GetComponent<Ragdoll>().EnableRagdoll();
    }

}
