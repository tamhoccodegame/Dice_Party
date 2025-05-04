using Fusion;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    private NetworkManager networkManager;

    public GameObject[] playerPrefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();
    }

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        Vector3 baseSpawnPosition = new Vector3(70, 0, 90);
        int index = 0;

        foreach (var player in networkManager.GetAllPlayers())
        {
            Vector3 spawnPosition = baseSpawnPosition + new Vector3(index * 3, 0, 0); // Tạo khoảng cách giữa players
            index++;

            Runner.Spawn(playerPrefabs[Random.Range(0, playerPrefabs.Length)], spawnPosition, Quaternion.identity, player, (runner, obj) => {
                obj.GetComponent<NetworkObject>().AssignInputAuthority(player);
            });

            Debug.Log($"Spawned player {player} với InputAuthority: {player}");
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
