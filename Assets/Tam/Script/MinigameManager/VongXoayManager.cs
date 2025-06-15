using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkDictionary<PlayerRef, NetworkBool> readyStatus => default;

    [Networked]
    public bool isGameOver { get; set; } = false;

    [Networked]
    public bool isGameStarted { get; set; } = false;

    [Header("Avatar Standing Position")]
    public Transform firstRankPosition;
    public Transform secondRankPosition;

    public GameObject playerRewardPrefab;

    public TextMeshProUGUI[] playerTextUI;

    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI firstRankName;
    public TextMeshProUGUI secondRankName;

    public Transform spawnPosition;


    public Image blackScreen;

    public float fadeDuration = 1f;

    public void FadeIn() => StartCoroutine(FadeBlackScreen(0, 1));
    public void FadeOut() => StartCoroutine(FadeBlackScreen(1, 0));

    public override void Spawned()
    {
        instance = this;
        tutorialPanel.SetActive(true);

        if (Object.HasStateAuthority)
        {
            RPC_HideTutorial();
            foreach (var player in NetworkManager.instance.GetAllPlayers())
            {
                playerLives.Set(player, 3);
            }
        }

        RequestUpdateLive(Runner.LocalPlayer);
    }

    private IEnumerator FadeBlackScreen(float from, float to)
    {
        float elapsed = 0f;
        Color color = blackScreen.color;
        color.a = from;

        blackScreen.color = color;

        Color newColor = blackScreen.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Dùng unscaled để không bị ảnh hưởng bởi Time.timeScale
            newColor.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            blackScreen.color = newColor;
            yield return null;
        }

        blackScreen.color = newColor;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_HideTutorial()
    {
        StartCoroutine(HideTutorialCouroutine());
    }

    IEnumerator HideTutorialCouroutine()
    {
        //Ẩn dần black screen rồi đợi 10s
        yield return StartCoroutine(FadeBlackScreen(1, 0));
        yield return new WaitForSecondsRealtime(10f);

        yield return StartCoroutine(FadeBlackScreen(0, 1));
        tutorialPanel.SetActive(false);

        yield return new WaitForSecondsRealtime(5f);
        yield return StartCoroutine(FadeBlackScreen(1, 0));

        yield return new WaitForSecondsRealtime(4f);


        if (Object.HasStateAuthority)
        {
            isGameStarted = true;
        }
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
            StartCoroutine(ReturnToBoard());

            isGameOver = true;
            RPC_ShowGameOverPanel();

            if (Object.HasStateAuthority && playerRanks.Count >= 2)
            {
                PlayerRef firstRankRef = playerRanks[^1]; // Người cuối cùng chết (Top 1)
                PlayerRef secondRankRef = playerRanks[^2]; // Người chết trước nó (Top 2)
                SpawnRewardAvatar(firstRankRef, secondRankRef);
            }
        }
    }

    IEnumerator ReturnToBoard()
    {
        yield return new WaitForSecondsRealtime(6f);
        yield return StartCoroutine(FadeBlackScreen(0, 1));
        yield return new WaitForSecondsRealtime(3f);

        Runner.LoadScene("TuanSceneMap");
    }

    bool CheckGameOver()
    {
        return playerLives.All(kvp => kvp.Value <= 0);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void SpawnRewardAvatar(PlayerRef firstRank, PlayerRef secondRank)
    {
        MNGVongXoayController[] players = FindObjectsByType<MNGVongXoayController>(FindObjectsSortMode.None);
        
        foreach(var player in players)
        {
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            Runner.Despawn(networkObject);
        }

        // Spawn phần thưởng avatar cho người chơi ở vị trí xếp hạng
        var fGo = Runner.Spawn(playerRewardPrefab, firstRankPosition.position, playerRewardPrefab.transform.rotation, firstRank);
        firstRankName.text = firstRank.PlayerId.ToString();

        var sGo = Runner.Spawn(playerRewardPrefab, secondRankPosition.position, playerRewardPrefab.transform.rotation, secondRank);
        secondRankName.text = secondRank.PlayerId.ToString();

        RPC_ChangeAnimation(fGo, "Win");
        RPC_ChangeAnimation(sGo, "Lose");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ChangeAnimation(NetworkObject player, string animName)
    {
        player.GetComponent<Animator>().Play(animName);
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
