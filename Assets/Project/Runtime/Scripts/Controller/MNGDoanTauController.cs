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
        if (WizardMiniGameManager.instance != null && playerInput != null)
        {
            WizardMiniGameManager.instance.playerObjects.Add(playerInput, gameObject);
        }
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

    [ContextMenu("Die Simu")]
    public void Die()
    {
        if (VongXoayManager.instance.isGameOver) return;

        Debug.Log("DIEE");

        int currentLives = WizardPartyData.instance.playersKey[playerInput];
        int newLives = Mathf.Max(0, currentLives - 1);
        if (newLives > 0)
        {
            WizardPartyData.instance.UpdatePlayerKey(playerInput, newLives);
        }
        else
        {
            ChangeAnim("Die");
            DisableInput();
        }
        WizardMiniGameManager.instance.UpdateHUD();
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
