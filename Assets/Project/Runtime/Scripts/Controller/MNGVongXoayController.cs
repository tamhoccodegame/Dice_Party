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

    private bool isGrounded;

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
