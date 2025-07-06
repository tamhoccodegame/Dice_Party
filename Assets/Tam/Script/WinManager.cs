using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class WinManager : NetworkBehaviour
{
    public static WinManager instance;

    public GameObject playerPrefab;
    public Transform[] spawnPositions;

    [Networked] public bool canMove { get; set; } = false;

    public PlayableDirector cutscene;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            int index = 1;
            foreach(var player in NetworkManager.instance.GetAllPlayers())
            {
                BoardGameData data = BoardGameData.instance;
                
                if (player == data.winner)
                {
                    Runner.Spawn(playerPrefab, spawnPositions[0].position, Quaternion.Euler(0, -180, 0), player);
                }
                else
                {
                    Runner.Spawn(playerPrefab, spawnPositions[index].position, Quaternion.Euler(0, -180, 0), player);
                    index++;
                }
            }

            RPC_PlayCutscene();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_PlayCutscene()
    {
        cutscene.Play();
        cutscene.stopped += Cutscene_stopped;
    }

    private void Cutscene_stopped(PlayableDirector obj)
    {
        if (HasStateAuthority)
        {
            canMove = true;
        }
    }
}
