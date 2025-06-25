using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    private NetworkManager networkManager;

    public GameObject playerPrefab;
    public Transform spawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();
    }

    public override void Spawned()
    {
        
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
        else
            foreach (var player in networkManager.GetAllPlayers())
            {
                Runner.Spawn(playerPrefab, spawnPosition.position, Quaternion.identity, player);
            }
    }
}
