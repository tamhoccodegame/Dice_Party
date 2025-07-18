using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterController))]
public class MNGVongXoayController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    public VisualEffect bloodEffect;

    public string currentAnim;

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

    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    void ResetGravity()
    {

    }

    void Update()
    {
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        controller.Move(move * 3 * Time.deltaTime);
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

            //int currentLive = VongXoayManager.instance.playerLives.Get(Object.Id);
            //VongXoayManager.instance.RequestUpdateLive(Object.InputAuthority, Object.Id);

        ChangeAnim("Die");
        DisableInput();
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
