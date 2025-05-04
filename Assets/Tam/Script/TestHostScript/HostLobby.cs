using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostLobby : NetworkBehaviour
{
    public TextMeshProUGUI playerListText;  // UI để hiển thị danh sách người chơi
    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();
    }

    public override void Spawned()
    {
        Invoke(nameof(UpdatePlayerList),0.5f);
        networkManager.onPlayerListChange += NetworkManager_onPlayerListChange;
    }

    private void NetworkManager_onPlayerListChange()
    {
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        List<PlayerRef> players = networkManager.GetAllPlayers();
        Debug.Log(players.Count);
        players.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));

        playerListText.text = "Player: \n";
        foreach (PlayerRef player in players)
        {
            playerListText.text += $"Player {player.PlayerId} \n";
        }
    }

    public void StartGame()
    {
        Runner.LoadScene("BoardScene");
    }
   
}

