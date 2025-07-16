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
        EndTurn(playerRef);
    }

    protected void EndTurn(PlayerRef player)
    {
        NewBoardGameController[] players = FindObjectsByType<NewBoardGameController>(FindObjectsSortMode.None);

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