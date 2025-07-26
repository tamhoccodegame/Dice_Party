using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(CharacterController))]
public class MNGVongXoayController : PlayerController
{
    private CharacterController controller;
    private Animator animator;

    public VisualEffect bloodEffect;

    public string currentAnim;

    public PlayerInput input;
    private Vector2 movementInput;

    private float verticalVelocity = 0f;
    public float gravity = -20f;
    public float jumpForce = 10f;

    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    private bool isGrounded;

    public Transform groundCheck;

    public void Awake()
    {
        bloodEffect.Stop();
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();

        if (VongXoayManager.instance != null)
        {
            //VongXoayManager.instance.RequestUpdateLive(Object.InputAuthority, Object.Id);
        }
    }

    private void Start()
    {
        if (VongXoayManager.instance != null && input != null)
        {
            VongXoayManager.instance.playerObjects.Add(input, gameObject);
        }
    }

    public override void SetInput(PlayerInput input)
    {
        this.input = input;
    }

    void Update()
    {
        if (input == null) return;

        movementInput = input.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);

        // Ground check
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // giữ cho player dính mặt đất
        }

        // Jump khi bấm Trigger
        if (input.actions["Trigger"].triggered && isGrounded)
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

    public void BloodEffect()
    {
        bloodEffect.Play();
    }

    [ContextMenu("Die Simu")]
    public void Die()
    {
        if (VongXoayManager.instance.isGameOver) return;

        Debug.Log("DIEE");
        BloodEffect();

        int currentLives = WizardPartyData.instance.playerLives[input];
        int newLives = Mathf.Max(0, currentLives - 1);
        if (newLives > 0)
        {
            WizardPartyData.instance.UpdatePlayerLive(input, newLives);
        }
        else
        {
            ChangeAnim("Die");
            DisableInput();
        }
        VongXoayManager.instance.UpdateHUD();
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
        return input;
    }
}
