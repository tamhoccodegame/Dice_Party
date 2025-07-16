using Fusion;
using System.Collections;
using System.Runtime.InteropServices;
using Unity.Cinemachine;
using UnityEngine;
using static UnityEngine.UI.Image;

public class MNGChayTruongController : NetworkBehaviour
{
    public Camera cam;
    private NetworkCharacterController controller;
    private Animator animator;

    public LayerMask coinMask;

    public string currentAnim;

    public bool isGoal = false;

    public override void Spawned()
    {
        controller = GetComponent<NetworkCharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
        cam = Camera.main;

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
        if (HasStateAuthority)
        {
            // origin: tâm, radius: bán kính
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1.5f, Vector3.up, 0f, coinMask);
            foreach (var hit in hits)
            {
                hit.collider.gameObject.GetComponent<Coins>().EatCoin(Object.Id);
            }
        }


        Vector3 moveDir = Vector3.zero;

        if (GetInput(out NetworkInputData data))
        {
            if (data.buttons.IsSet(NetworkInputData.JUMPBUTTON))
            {
                controller.Jump();
                RPC_ChangeAnim("Jump");
            }

            // Movement
            data.direction.Normalize();
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            moveDir = camRight * data.direction.x + camForward * data.direction.z;

            controller.Move(moveDir);

            // Animation
            if (controller.Grounded)
            {
                if (moveDir.magnitude > 0)
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

    public void Die()
    {
        if (VongXoayManager.instance.isGameOver) return;

        if (Object.HasInputAuthority)
        {
            Debug.Log("DIEE");

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Goal")
        {
            if (Object.HasInputAuthority)
                RPC_RequestSetGoal();
        }
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_RequestSetGoal()
    {
        if (isGoal) return;
        isGoal = true;

        if(HasStateAuthority)
        Coin_Manager.Instance.UpdateGameState();
    }
}
