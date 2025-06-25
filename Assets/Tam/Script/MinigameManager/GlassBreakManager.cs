using Fusion;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GlassCouple
{
    public BreakGlass glass1;
    public BreakGlass glass2;
}

public class GlassBreakManager : NetworkBehaviour
{
    public static GlassBreakManager instance;
    public GlassCouple[] glassCouples;

    public Image blackScreen;

    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkDictionary<PlayerRef, int> playerScore => default;

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkLinkedList<PlayerRef> playerRanks => default;

    [Header("Avatar Standing Position")]
    public Transform firstRankPosition;
    public Transform secondRankPosition;
    public Transform thirdRankPosition;

    public GameObject playerRewardPrefab;

    public TextMeshProUGUI[] playerTextUI;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI firstRankName;
    public TextMeshProUGUI secondRankName;

    public Transform spawnPosition;

    [Networked]
    public bool isGameOver { get; set; } = false;

    [Networked]
    public bool isGameStarted { get; set; } = false;

    public float fadeDuration = 1f;

    public void FadeIn() => StartCoroutine(FadeBlackScreen(0, 1));
    public void FadeOut() => StartCoroutine(FadeBlackScreen(1, 0));

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            foreach (var glassCouple in glassCouples)
            {
                if (Random.value < 0.5f)
                {
                    glassCouple.glass1.SetBreakable(true);
                    glassCouple.glass2.SetBreakable(false);
                }
                else
                {
                    glassCouple.glass1.SetBreakable(false);
                    glassCouple.glass2.SetBreakable(true);
                }
            }

        }

        MusicManager.instance.PlayMusic(MusicManager.MusicType.MNG);
        instance = this;
        tutorialPanel.SetActive(true);

        if (Object.HasStateAuthority)
        {
            RPC_HideTutorial();
        }
    }

    //Khi mà đến đích thì sẽ gọi hàm này
    public void RequestAddRank(PlayerRef player)
    {
        if (HasStateAuthority)
        {
            UpdateRank(player);
        }
        else
        {
            RPC_RequestUpdateRank(player);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]

    public void RPC_RequestUpdateRank(PlayerRef player)
    {
        UpdateRank(player);
    }

    public void UpdateRank(PlayerRef player)
    {
        if (isGameOver) return;

        if(!playerRanks.Contains(player))
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
                PlayerRef firstRankRef = playerRanks[^1]; 
                PlayerRef secondRankRef = playerRanks[^2]; 
                //PlayerRef thirdRankRef = playerRanks[^3];
                SpawnRewardAvatar(firstRankRef, secondRankRef/*, thirdRankRef*/);
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdateUILive()
    {
        int index = 0;
        foreach (var kvp in playerScore)
        {
            if (index < playerTextUI.Length)
            {
                playerTextUI[index].text = kvp.Value.ToString();
                index++;
            }
        }
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
        yield return new WaitForSecondsRealtime(10f);

        yield return StartCoroutine(FadeBlackScreen(0, 1));
        tutorialPanel.SetActive(false);

        yield return new WaitForSecondsRealtime(5f);
        yield return StartCoroutine(FadeBlackScreen(1, 0));

        GetComponent<PlayerSpawner>().SpawnPlayer();

        yield return new WaitForSecondsRealtime(4f);


        if (Object.HasStateAuthority)
        {
            isGameStarted = true;
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
        return playerRanks.Count >= Runner.ActivePlayers.Count();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void SpawnRewardAvatar(PlayerRef firstRank, PlayerRef secondRank/*, PlayerRef thirdRank*/)
    {
        MNGCauKinhController[] players = FindObjectsByType<MNGCauKinhController>(FindObjectsSortMode.None);

        foreach (var player in players)
        {
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            Runner.Despawn(networkObject);
        }

        // Spawn phần thưởng avatar cho người chơi ở vị trí xếp hạng
        var fGo = Runner.Spawn(playerRewardPrefab, firstRankPosition.position, playerRewardPrefab.transform.rotation, firstRank);
        firstRankName.text = firstRank.PlayerId.ToString();

        var sGo = Runner.Spawn(playerRewardPrefab, secondRankPosition.position, playerRewardPrefab.transform.rotation, secondRank);
        secondRankName.text = secondRank.PlayerId.ToString();

        //var tGo = Runner.Spawn(playerRewardPrefab, thirdRankPosition.position, playerRewardPrefab.transform.rotation, thirdRank);
        //secondRankName.text = thirdRank.PlayerId.ToString();


        RPC_ChangeAnimation(fGo, "Win");
        RPC_ChangeAnimation(sGo, "Lose");
        //RPC_ChangeAnimation(tGo, "Lose");

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ChangeAnimation(NetworkObject player, string animName)
    {
        player.GetComponent<Animator>().Play(animName);
    }

}
