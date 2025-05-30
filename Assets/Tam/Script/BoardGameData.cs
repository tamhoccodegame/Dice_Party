using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGameData : MonoBehaviour
{
    public static BoardGameData instance;

    public Dictionary<PlayerRef, string> playerCurrentNode = new Dictionary<PlayerRef, string>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateNode(PlayerRef player, string nodeName)
    {
        if (!playerCurrentNode.ContainsKey(player))
        {
            playerCurrentNode[player] = nodeName;
        }
        else
        {
            playerCurrentNode.Add(player, nodeName);
        }

        foreach (var kvp in playerCurrentNode)
        {
            Debug.Log($"{kvp.Key} {kvp.Value}");
        }
    }

    public string GetNode(PlayerRef player)
    {
        return playerCurrentNode[player];
    }
}
