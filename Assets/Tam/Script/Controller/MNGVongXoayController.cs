using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(NetworkCharacterController))]
[RequireComponent(typeof(CharacterController))]
public class MNGVongXoayController : NetworkBehaviour
{
    private NetworkCharacterController controller;
    private Animator animator;
    [Networked] private string NetworkAnim { get; set; } // Animation sync

    public VisualEffect bloodEffect;

    public string currentAnim;

    VongXoayManager manager;

    public override void Spawned()
    {
        bloodEffect.Stop();
        controller = GetComponent<NetworkCharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
        manager = VongXoayManager.instance;
        Invoke(nameof(ResetGravity), 2f);
    }

    void ResetGravity()
    {

    }

    void Update()
    {
        if (!Object.HasInputAuthority) return;

        if (GetInput(out NetworkInputData data))
        {
            Vector3 direction = data.direction;

            if (data.buttons.IsSet(NetworkInputData.JUMPBUTTON) && controller.Grounded)
            {
                controller.Jump();
                ChangeAnim("Jump");
            }

            // Luôn chạy Move
            controller.Move(direction);

            // Anim xử lý tách biệt
            if (controller.Grounded)
            {
                if (direction.sqrMagnitude > 0.001f)
                    ChangeAnim("Run");
                else
                    ChangeAnim("Idle");
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Client đọc networked animation state
        if (NetworkAnim != currentAnim)
        {
            animator.CrossFade(NetworkAnim, 0.25f);
            currentAnim = NetworkAnim;
        }

        if (!Object.HasStateAuthority) return;

        if (GetInput(out NetworkInputData data))
        {
            Vector3 direction = data.direction;

            if (data.buttons.IsSet(NetworkInputData.JUMPBUTTON) && controller.Grounded)
            {
                controller.Jump();
                ChangeAnim("Jump");
            }

            // Luôn chạy Move
            controller.Move(direction);

            // Anim xử lý tách biệt
            if (controller.Grounded)
            {
                if (direction.sqrMagnitude > 0.001f)
                    ChangeAnim("Run");
                else
                    ChangeAnim("Idle");
            }
        }

    }

    public void ChangeAnim(string animName, float blendTime = 0.25f)
    {
        if (animName == currentAnim) return;
        currentAnim = animName;

        if (HasStateAuthority)
            NetworkAnim = animName;

        animator.CrossFade(animName, blendTime);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_BloodEffect()
    {
        bloodEffect.Play();
    }

    public void Die()
    {
        if (VongXoayManager.instance.isGameOver) return;


        if (Object.HasInputAuthority)
        {
            RPC_BloodEffect();

            VongXoayManager.instance.RequestUpdateLive(Runner.LocalPlayer);

            if (VongXoayManager.instance.playerLives.Get(Runner.LocalPlayer) <= 0)
            {
                RPC_EnableRagdoll();
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_EnableRagdoll()
    {
        GetComponent<Ragdoll>().EnableRagdoll();
    }
}
