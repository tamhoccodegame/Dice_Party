using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerBlinking : NetworkBehaviour
{
    [Header("Animation & Invincibility")]
    public Animator animator;
    public float invincibleDuration = 2f;
    public float blinkInterval = 0.15f;

    [Networked] private NetworkBool isInvincible { get; set; }
    [Networked] private NetworkBool isBlinking { get; set; }

    private List<SkinnedMeshRenderer> skinnedMeshes = new List<SkinnedMeshRenderer>();

    private void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        skinnedMeshes.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
    }

    public void OnHitByObstacle(Vector3 hitPoint)
    {
        if (!HasStateAuthority) return; // Host xử lý

        if (isInvincible)
        {
            Debug.Log("[⚡ IMMUNE] Player is invincible");
            return;
        }

        RPC_PlayHurtAnim();
        Debug.Log("[😵 HIT] Player took damage at " + hitPoint);
        Audio_Manager.Instance.Play2D("Hurt");

        Coin_Manager.Instance.DropCoins(Object.InputAuthority, hitPoint);

        RPC_StartInvisibly();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StartInvisibly()
    {
        StartCoroutine(InvincibilityRoutine());
    }


    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        isBlinking = true;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invincibleDuration)
        {
            visible = !visible;
            RPC_SetVisible(visible); // Gửi cho toàn bộ client

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        RPC_SetVisible(true); // Đảm bảo hiện lại
        isInvincible = false;
        isBlinking = false;

        Debug.Log("[🛡️ DONE] Invincibility ended.");
    }

    private void SetAllMeshesVisible(bool visible)
    {
        foreach (var mesh in skinnedMeshes)
        {
            if (mesh != null)
                mesh.enabled = visible;
        }
    }

    public bool IsInvincible() => isInvincible;

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_PlayHurtAnim()
    {
        animator.SetTrigger("isHurt");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SetVisible(bool visible)
    {
        SetAllMeshesVisible(visible);
    }
}
