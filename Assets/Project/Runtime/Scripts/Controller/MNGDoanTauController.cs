using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(CharacterController))]
public class MNGDoanTauController : PlayerController
{
    private CharacterController controller;
    private Animator animator;

    public string currentAnim;

    public PlayerInput playerInput;
    private Vector2 movementInput;

    private float verticalVelocity = 0f;
    public float gravity = -20f;
    public float jumpForce = 10f;

    private bool isGrounded;

    public Transform groundCheck;

    public void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        
    }

    public override void SetInput(PlayerInput input)
    {
        this.playerInput = input;
    }

    void Update()
    {
        if (playerInput == null) return;

        movementInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);

        // Ground check
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // giữ cho player dính mặt đất
        }

        move.y = verticalVelocity;

        // Move
        controller.Move(move * 8f * Time.deltaTime);

        // Rotate theo hướng di chuyển (chỉ trên mặt phẳng ngang)
        Vector3 horizontalMove = new Vector3(move.x, 0, move.z);
        if (horizontalMove.magnitude > 0.1f)
        {
            Quaternion newRotation = Quaternion.LookRotation(horizontalMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 5 * Time.deltaTime);
        }

        // Animation
        if (isGrounded)
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

 
    void DisableInput()
    {
        this.enabled = false;
    }

    void EnableRagdoll()
    {
        GetComponent<Ragdoll>().EnableRagdoll();
    }

    public override PlayerInput GetPlayerInput()
    {
        return playerInput;
    }
}
