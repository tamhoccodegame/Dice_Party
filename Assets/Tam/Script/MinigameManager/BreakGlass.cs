using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakGlass : NetworkBehaviour
{
    [Networked] public bool isBreakable { get; set; } = false;

    public GameObject breakEffect;

    public void SetBreakable(bool isBreakable)
    {
        if(Object.HasStateAuthority)
        this.isBreakable = isBreakable;
    }

    public void TryBreak()
    {
        if (!isBreakable) return;

        if (!Object.HasStateAuthority) return; // ✅ chỉ host được gọi phá

        RPC_Break(); // Gửi yêu cầu phá cho tất cả
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Break()
    {
        Instantiate(breakEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
