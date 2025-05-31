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
        if (!Object.HasStateAuthority) return;

        BoardGameData boardGameData = BoardGameData.instance;
        bool isBoardScene = SceneManager.GetActiveScene().name == "TuanSceneMap";


        if (boardGameData != null && boardGameData.playerCurrentNode.Count > 0 && isBoardScene)
        {
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


    // Update is called once per frame
    void Update()
    {

    }
    private void OnDestroy()
    {
    }
}
