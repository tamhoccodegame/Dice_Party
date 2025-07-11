using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapNode : BoardNode
{
    public Animator theDeath;

    public override void ProcessNode(PlayerRef playerRef, NetworkId playerObject)
    {
        if (HasStateAuthority)
        {
            RPC_TrapEffect(playerRef, playerObject);
        }
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_TrapEffect(PlayerRef playerRef, NetworkId playerObject)
    {
        StartCoroutine(ProcessCoroutine(playerRef, playerObject));
    }

    IEnumerator ProcessCoroutine(PlayerRef playerRef, NetworkId playerObject)
    {
        //Animator animator = Runner.FindObject(playerObject).GetComponent<Animator>();
        if (nodeEffect != null) nodeEffect.Play();
        theDeath.Play("Attack");
        yield return new WaitForSecondsRealtime(1.5f);
        TurnManager.instance.RequestUpdateHealth(playerRef, -20);

        yield return new WaitForSecondsRealtime(1f);

        EndTurn(playerRef);
    }
}

