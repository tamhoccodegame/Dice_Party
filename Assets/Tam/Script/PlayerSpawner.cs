using Fusion;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    private NetworkManager networkManager;
    public GameObject playerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();
    }

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;
        BoardNode spawn = FindFirstObjectByType<BoardNode>();

        Vector3 baseSpawnPosition = spawn.transform.position;

        foreach (var player in networkManager.GetAllPlayers())
        {
            Vector3 spawnPosition = baseSpawnPosition;

            Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player, (runner, obj) =>
            {
                obj.GetComponent<NetworkObject>().AssignInputAuthority(player);
            });
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
