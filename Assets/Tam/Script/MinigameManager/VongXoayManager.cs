using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

public class VongXoayManager : NetworkBehaviour
{
    public static VongXoayManager instance;

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkDictionary<PlayerRef, int> playerLives => default;

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkLinkedList<PlayerRef> playerRanks => default;

    [Header("Avatar Standing Position")]
    public Transform firstRankPosition;
    public Transform secondRankPosition;

    public GameObject playerRewardPrefab;

    public TextMeshProUGUI[] playerTextUI;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI firstRankName;
    public TextMeshProUGUI secondRankName;

    public override void Spawned()
    {
        instance = this;
        if (Object.HasStateAuthority)
        {
            foreach (var player in NetworkManager.instance.GetAllPlayers())
            {
                playerLives.Set(player, 3); // Sử dụng Set an toàn hơn
            }
        }

        RPC_UpdateUILive();
    }

    public void RequestUpdateLive(PlayerRef player)
    {
        if (Object.HasStateAuthority) 
        UpdateLive(player);
    }

    private void UpdateLive(PlayerRef player)
    {
        if(playerLives.TryGet(player, out int value))
        playerLives.Set(player, value - 1);

        if(playerLives[player] <= 0 && !playerRanks.Contains(player))
            playerRanks.Add(player);

        RPC_UpdateUILive();
        bool isOver = CheckGameOver();
        if (isOver)
        {
            ShowGameOverPanel();

            if (Object.HasStateAuthority)
            {
                PlayerRef firstRankRef = playerRanks[playerRanks.Count - 1];
                //PlayerRef secondRankRef = playerRanks[playerRanks.Count - 2];
                
                RPC_SpawnRewardAvatar(firstRankRef);
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SpawnRewardAvatar(PlayerRef firstRank)
    {
        Runner.Spawn(playerRewardPrefab, firstRankPosition.position, playerRewardPrefab.transform.rotation, firstRank);
        firstRankName.text = firstRank.PlayerId.ToString();
    }

    void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    bool CheckGameOver()
    {
        return playerLives.Count(kvp => kvp.Value == 0) >= playerLives.Count();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdateUILive()
    {
        int index = 0;
        foreach (var kvp in playerLives)
        {
            int lives = kvp.Value;

            // Hiển thị lên UI
            playerTextUI[index].text = lives.ToString();
            index++;
        }
    }
}
