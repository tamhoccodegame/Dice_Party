using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class VongXoayManager : NetworkBehaviour
{
    public NetworkDictionary<PlayerRef, int> playerLives = new NetworkDictionary<PlayerRef, int>();

    public TextMeshProUGUI[] playerTextUI;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            foreach(var player in NetworkManager.instance.GetAllPlayers())
            {
                playerLives[player] = 3; 
            }
        }
    }

    public void UpdateLive(PlayerRef player)
    {
        playerLives[player] -= 1;
        RPC_UpdateUILive();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdateUILive()
    {
        int index = 0;
        foreach (var kvp in playerLives)
        {
            PlayerRef player = kvp.Key;
            int lives = kvp.Value;

            // Hiển thị lên UI
            playerTextUI[index].text = lives.ToString();
            index++;
        }
    }

    public void GameOver()
    {

    }

    public void UpgradeDifficult()
    {

    }

}
