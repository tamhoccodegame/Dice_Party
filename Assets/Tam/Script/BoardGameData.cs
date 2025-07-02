using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGameData : MonoBehaviour
{
    public static BoardGameData instance;

    public Dictionary<PlayerRef, string> playersCurrentNode = new Dictionary<PlayerRef, string>();
    public Dictionary<PlayerRef, string> playersName = new Dictionary<PlayerRef, string>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateNode(PlayerRef player, string nodeName)
    {
        if (!playersCurrentNode.ContainsKey(player))
        {
            playersCurrentNode.Add(player, nodeName);
        }
        else
        {
            playersCurrentNode[player] = nodeName;
        }

        foreach (var kvp in playersCurrentNode)
        {
            Debug.Log($"{kvp.Key} {kvp.Value}");
        }
    }

    public string GetNode(PlayerRef player)
    {
        if(playersCurrentNode.ContainsKey(player))
        return playersCurrentNode[player];

        return null;
    }

    public void SetName(PlayerRef player, string name)
    {
        if (!playersName.ContainsKey(player))
        {
            playersName.Add(player, name);
        }
    }

    public string GetName(PlayerRef player)
    {
        if(playersName.ContainsKey(player))
            return playersName[player];
        return null;
    }
}
