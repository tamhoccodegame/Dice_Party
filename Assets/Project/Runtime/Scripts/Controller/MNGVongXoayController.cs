using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(CharacterController))]
public class MNGVongXoayController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    public VisualEffect bloodEffect;

    public string currentAnim;

    public PlayerInput input;
    private Vector2 movementInput;


    public void Awake()
    {
        bloodEffect.Stop();
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();

        if (VongXoayManager.instance != null)
            //VongXoayManager.instance.RequestUpdateLive(Object.InputAuthority, Object.Id);

        Invoke(nameof(ResetGravity), 2f);
    }

    public void SetInput(PlayerInput input)
    {
        this.input = input;

        this.input.actions["Move"].performed += OnMove;
        this.input.actions["Move"].canceled += OnMove;

        Custom customData = PlayerManager.instance.GetComponentInChildren<CustomData>().GetCustom(input);
        GetComponent<PlayerSetup>().UpdateCustom(customData.hairIndex, customData.colorIndex, customData.bodyPartIndex);
    }

    private void OnDisable()
    {
        this.input.actions["Move"].performed -= OnMove;
        this.input.actions["Move"].canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    void ResetGravity()
    {

    }
    void Update()
    {
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        controller.Move(move * 8f * Time.deltaTime);

        if(move.magnitude > 0.1f)
        {
            Quaternion newRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 5 * Time.deltaTime);
        }

        if(move.magnitude > 0.1f)
        {
            ChangeAnim("Run");
        }
        else
        {
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
            
    }

    void DisableInput()
    {
        Destroy(this);
    }

    void EnableRagdoll()
    {
        GetComponent<Ragdoll>().EnableRagdoll();
    }
}
