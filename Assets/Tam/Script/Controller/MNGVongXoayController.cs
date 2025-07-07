using Fusion;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(NetworkCharacterController))]
[RequireComponent(typeof(CharacterController))]
public class MNGVongXoayController : NetworkBehaviour
{
    private NetworkCharacterController controller;
    private Animator animator;

    public VisualEffect bloodEffect;

    public string currentAnim;

    public override void Spawned()
    {
        bloodEffect.Stop();
        controller = GetComponent<NetworkCharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();

        if (VongXoayManager.instance != null)
            VongXoayManager.instance.RequestUpdateLive(Object.InputAuthority, Object.Id);

        Invoke(nameof(ResetGravity), 2f);
    }

    void ResetGravity()
    {

    }

    void Update()
    {

    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (GetInput(out NetworkInputData data))
        {
            Vector3 direction = data.direction;

            if (data.buttons.IsSet(NetworkInputData.JUMPBUTTON) && controller.Grounded)
            {
                controller.Jump();

                RPC_ChangeAnim("Jump");
            }

            // Luôn chạy Move
            controller.Move(direction);

            // Anim xử lý tách biệt
            if (controller.Grounded)
            {
                if (direction.sqrMagnitude > 0.001f)
                    RPC_ChangeAnim("Run");
                else
                    RPC_ChangeAnim("Idle");
            }
        }

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ChangeAnim(string animName, float blendTime = 0.25f)
    {
        if (animName == currentAnim) return;
        currentAnim = animName;

        animator.CrossFade(animName, blendTime);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestChangeAnim(string animName, float blendTime = 0.25f)
    {
        RPC_ChangeAnim(animName, blendTime);
    }



    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_BloodEffect()
    {
        bloodEffect.Play();
    }

    [ContextMenu("Die Simu")]
    public void Die()
    {
        if (VongXoayManager.instance.isGameOver) return;

        if (Object.HasInputAuthority)
        {
            Debug.Log("DIEE");
            RPC_BloodEffect();

            int currentLive = VongXoayManager.instance.playerLives.Get(Object.Id);
            VongXoayManager.instance.RequestUpdateLive(Object.InputAuthority, Object.Id);

            StartCoroutine(DelayCheckDie());
        }
    }

    IEnumerator DelayCheckDie()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        if (VongXoayManager.instance.playerLives.Get(Object.Id) <= 0)
        {
            RPC_RequestChangeAnim("Die");
            RPC_DisableInput();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_DisableInput()
    {
        Destroy(this);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_EnableRagdoll()
    {
        GetComponent<Ragdoll>().EnableRagdoll();
    }
}
