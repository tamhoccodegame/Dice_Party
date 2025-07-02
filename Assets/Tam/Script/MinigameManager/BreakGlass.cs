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
        if(HasStateAuthority)
        this.isBreakable = isBreakable;
    }

    public void TryBreak()
    {
        if (!isBreakable) return;
        RPC_Break();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Break()
    {
        Instantiate(breakEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
