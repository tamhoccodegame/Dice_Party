using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    private NetworkManager networkManager;

    public GameObject playerPrefab;
    public Transform[] spawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();
    }

    public override void Spawned()
    {
        //Test Scene Only
        //Runner.Spawn(playerPrefab, spawnPosition.position, Quaternion.identity, Runner.LocalPlayer);
    }

    public void SpawnPlayer()
    {
        if (!Object.HasStateAuthority) return;

        BoardGameData boardGameData = BoardGameData.instance;
        bool isBoardScene = SceneManager.GetActiveScene().name == "TuanSceneMap";


        if (boardGameData != null && boardGameData.playerCurrentNode.Count > 0 && isBoardScene)
        {
            TurnManager.instance.isFirstTry = false;
            foreach (var player in networkManager.GetAllPlayers())
            {
                Transform spawnPosition1 = GameObject.Find(boardGameData.GetNode(player)).transform;
                Runner.Spawn(playerPrefab, spawnPosition1.position, Quaternion.identity, player);
            }
        }
        else if (isBoardScene)
        {
            foreach (var player in networkManager.GetAllPlayers())
            {
                Runner.Spawn(playerPrefab, spawnPosition[0].position, Quaternion.identity, player);
            }
        }
        else
        {
            List<PlayerRef> playerList = networkManager.GetAllPlayers();
            for(int i = 0; i < playerList.Count; i++)
            {
                Runner.Spawn(playerPrefab, spawnPosition[i].position, Quaternion.identity, playerList[i]);
            }
        }
            
    }
}
