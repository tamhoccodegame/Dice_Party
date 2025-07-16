using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusNode : BoardNode
{
    public override void ProcessNode(PlayerRef playerRef, NetworkId playerObject)
    {
        if (HasStateAuthority)
        {
            RPC_PlusEffect(playerRef, playerObject);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_PlusEffect(PlayerRef playerRef, NetworkId playerObject)
    {
        StartCoroutine(ProcessCoroutine(playerRef, playerObject));
    }

    IEnumerator ProcessCoroutine(PlayerRef playerRef, NetworkId playerObject)
    {
        if (nodeEffect != null) nodeEffect.Play();

        NewBoardGameController player = Runner.FindObject(playerObject).GetComponent<NewBoardGameController>();

        player.RequestSetStepLeft(3);

        player.RequestSetCurrentNode(Object.Id);

        yield return new WaitForSecondsRealtime(0.5f);

        player.RequestChangeState(NewBoardGameController.NetworkState.Moving);
    }
}
