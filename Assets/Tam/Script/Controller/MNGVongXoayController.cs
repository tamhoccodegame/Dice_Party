using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterController))]
public class MNGVongXoayController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    public VisualEffect bloodEffect;

    public string currentAnim;

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

    void ResetGravity()
    {

    }

    void Update()
    {

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
