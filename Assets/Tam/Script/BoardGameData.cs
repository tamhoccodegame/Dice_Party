using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGameData : MonoBehaviour
{
    public static BoardGameData instance;

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public Dictionary<PlayerRef, string> playerCurrentNode => default;

    public void UpdateNode(PlayerRef player, string nodeName)
    {
        if (playerCurrentNode.ContainsKey(player))
        {
            playerCurrentNode[player] = nodeName;
        }
    }

    public string GetNode(PlayerRef player)
    {
        return playerCurrentNode[player];
    }
}
