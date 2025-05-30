using System.Collections;
using System.Collections.Generic;
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
    public NetworkLinkedList<int> playerRanks => default;

    public Dictionary<int, Transform> avatarStandingPosition = new Dictionary<int, Transform>();

    public GameObject firstRankCamRT;
    public GameObject secondRankCamRT;

    public Vector3 cameraOffset;

    public TextMeshProUGUI[] playerTextUI;

    [Header("GameOverPanel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI firstRankText;
    public TextMeshProUGUI firstRankName;
    public TextMeshProUGUI secondRankText;
    public TextMeshProUGUI secondRankName;

    [Header("AvatarPosition")]
    public Transform[] avatarPositions;

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

        RegisterPlayer();

        RPC_UpdateUILive();
    }


    public void RegisterPlayer()
    {
        List<PlayerRef> players = NetworkManager.instance.GetAllPlayers();

        for (int i = 0; i < players.Count; i++)
        {
            int playerId = players[i].PlayerId;
            avatarStandingPosition[playerId] = avatarPositions[i];
        }
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

        if(playerLives[player] <= 0 && !playerRanks.Contains(player.PlayerId))
            playerRanks.Add(player.PlayerId);

        RPC_UpdateUILive();
        bool isOver = CheckGameOver();
        if (isOver)
        {
            ShowGameOverPanel();
            int firstRankRef = playerRanks[playerRanks.Count - 1];
            //PlayerRef secondRankRef = playerRanks[playerRanks.Count - 2];

            Transform firstRankPos = avatarStandingPosition[firstRankRef];
            //Vector3 secondRankPos = playerObjects[secondRankRef].transform.position;

            var firstCam = Instantiate(firstRankCamRT, firstRankPos.position, Quaternion.identity, firstRankPos);
            //var secondCam = Instantiate(secondRankCamRT, secondRankPos, Quaternion.identity);

            firstCam.transform.localPosition = cameraOffset;
            firstCam.transform.rotation = Quaternion.Euler(0, 180, 0);

            //secondCam.transform.localPosition = cameraOffset;
            //secondCam.transform.rotation = Quaternion.Euler(0, 180, 0);

        }
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
