using Fusion;
using UnityEngine;

public class PlayerLobbySpawner : SimulationBehaviour, IPlayerJoined
{
    public void PlayerJoined(PlayerRef player)
    {
        string name = "Ala";
        Lobby.instance.Spawned();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
