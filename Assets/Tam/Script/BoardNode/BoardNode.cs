using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BoardNode : NetworkBehaviour
{
    public bool isStartNode = false;
    public List<BoardNode> nextNodes;

    public ParticleSystem nodeEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void ProcessNode(PlayerRef playerRef, NetworkId playerObject)
    {

    }

    protected void EndTurn(PlayerRef player)
    {
        BoardGameController[] players = FindObjectsByType<BoardGameController>(FindObjectsSortMode.None);

        foreach (var p in players)
        {
            if (p.Object.InputAuthority == player)
            {
                p.EndTurn();
            }
            else continue;
        }
    }
}