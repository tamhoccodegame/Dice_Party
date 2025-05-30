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

    [Networked]
    public bool isGameOver { get; set; } = false;

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
                playerLives.Set(player, 3);
            }
        }

        RequestUpdateLive(Runner.LocalPlayer);
    }

    public void RequestUpdateLive(PlayerRef player)
    {
        if (isGameOver) return;

        if (Object.HasStateAuthority)
        {
            UpdateLive(player);
        }
        else
        {
            RPC_RequestUpdateLive(player);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestUpdateLive(PlayerRef player)
    {
        UpdateLive(player);
    }

    private void UpdateLive(PlayerRef player)
    {
        if (isGameOver) return;

        if (playerLives.TryGet(player, out int value))
        {
            playerLives.Set(player, value - 1);
        }

        if (playerLives[player] <= 0 && !playerRanks.Contains(player))
        {
            playerRanks.Add(player);
        }

        if (Object.HasStateAuthority)
        {
            RPC_UpdateUILive();
        }

        if (CheckGameOver())
        {
            isGameOver = true;
            ShowGameOverPanel();

            if (Object.HasStateAuthority && playerRanks.Count >= 2)
            {
                PlayerRef firstRankRef = playerRanks[^1]; // Người cuối cùng chết (Top 1)
                PlayerRef secondRankRef = playerRanks[^2]; // Người chết trước nó (Top 2)
                SpawnRewardAvatar(firstRankRef, secondRankRef);
            }
        }
    }

    bool CheckGameOver()
    {
        return playerLives.All(kvp => kvp.Value <= 0);
    }

    void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void SpawnRewardAvatar(PlayerRef firstRank, PlayerRef secondRank)
    {
        // Spawn phần thưởng avatar cho người chơi ở vị trí xếp hạng
        Runner.Spawn(playerRewardPrefab, firstRankPosition.position, playerRewardPrefab.transform.rotation, firstRank);
        firstRankName.text = firstRank.PlayerId.ToString();

        Runner.Spawn(playerRewardPrefab, secondRankPosition.position, playerRewardPrefab.transform.rotation, secondRank);
        secondRankName.text = secondRank.PlayerId.ToString();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdateUILive()
    {
        int index = 0;
        foreach (var kvp in playerLives)
        {
            if (index < playerTextUI.Length)
            {
                playerTextUI[index].text = kvp.Value.ToString();
                index++;
            }
        }
    }
}
