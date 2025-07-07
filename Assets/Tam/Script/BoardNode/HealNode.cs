using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealNode : BoardNode
{
    public override void ProcessNode(PlayerRef playerRef, NetworkId playerObject)
    {
        if (HasStateAuthority)
        {
            RPC_HealEffect(playerRef, playerObject);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_HealEffect(PlayerRef playerRef, NetworkId playerObject)
    {
        StartCoroutine(ProcessCoroutine(playerRef, playerObject));
    }

    IEnumerator ProcessCoroutine(PlayerRef playerRef, NetworkId playerObject)
    {
        if (nodeEffect != null) nodeEffect.Play();

        TurnManager.instance.RequestUpdateHealth(playerRef, 20);

        yield return new WaitForSecondsRealtime(0.5f);

        EndTurn(playerRef);
    }
}
