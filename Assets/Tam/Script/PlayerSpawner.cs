using Fusion;
using UnityEngine;

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
