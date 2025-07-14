using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapActivator : NetworkBehaviour
{
    public float activationDistance = 20f;
    private Transform player;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            RPC_SetActive(false);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Init(Transform playerRef)
    {
        player = playerRef;
    }

    public void CheckActivation()
    {
        if (!HasStateAuthority) return;

        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool shouldBeActive = distance <= activationDistance;

        if (shouldBeActive && !gameObject.activeSelf)
            RPC_SetActive(true);
        else if (!shouldBeActive && gameObject.activeSelf)
            RPC_SetActive(false);
    }
}
