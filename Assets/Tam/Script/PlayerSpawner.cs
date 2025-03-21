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
        Debug.Log(networkManager.GetAllPlayers().Count);
        Vector3 spawnPosition = new Vector3(70, 0, 90);
        foreach (var player in networkManager.GetAllPlayers())
        {
            Runner.Spawn(playerPrefabs[Random.Range(0, playerPrefabs.Length)], spawnPosition, Quaternion.identity);
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
