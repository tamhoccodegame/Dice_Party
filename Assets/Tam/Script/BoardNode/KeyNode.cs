using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyNode : BoardNode
{
    public override void RPC_ProcessNode(PlayerRef player)
    {
        if(nodeEffect != null) 
        nodeEffect.Play();
        TurnManager.instance.RequestUpdateKey(player, 2);
        TurnManager.instance.RequestUpdateCup(player, 1);
        EndTurn(player);
    }
}
