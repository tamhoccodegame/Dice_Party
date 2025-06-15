using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BoardNode : NetworkBehaviour
{
    public bool isStartNode = false;
    public List<BoardNode> nextNodes;
    public enum EventType
    {
        None,
        Key,
        Blood,
    }

    public EventType eventType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ProcessNode(PlayerRef player)
    {
        switch (eventType)
        {
            case EventType.Key:
                Debug.Log("Add Key");
                break;
            case EventType.Blood:
                Debug.Log("Add Blood");
                break;
        }

        BoardGameController[] players = FindObjectsByType<BoardGameController>(FindObjectsSortMode.None);

        foreach(var p in players)
        {
            if (p.Object.InputAuthority == player)
            {
                p.EndTurn();
            }
            else continue;
        }

    }
}