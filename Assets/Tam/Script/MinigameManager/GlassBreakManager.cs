using Fusion;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
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

    public TextMeshProUGUI countDownText;

    [Networked] public int time { get; set; }

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkDictionary<NetworkId, int> playerScore => default;

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkLinkedList<NetworkId> playerRanks => default;

    [Header("Avatar Standing Position")]
    public Transform[] rankPositions;

    public GameObject playerRewardPrefab;

    public TextMeshProUGUI[] playerTextUI;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI firstRankName;
    public TextMeshProUGUI secondRankName;
    public GameObject globalVolume;

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

        countDownText.text = time.ToString();
        InvokeRepeating(nameof(CountDown), 0f, 1f);

        MusicManager.instance.PlayMusic(MusicManager.MusicType.MNG);
        instance = this;
        tutorialPanel.SetActive(true);

        if (Object.HasStateAuthority)
        {
            RPC_HideTutorial();
        }
    }

    void CountDown()
    {
        if (!isGameStarted || isGameOver) return;

        if (HasStateAuthority)
        {
            time -= 1;
        }

        countDownText.text = time.ToString();
    }

    void CheckTimeOut()
    {

    }

    //Khi mà đến đích thì sẽ gọi hàm này
    public void RequestAddRank(NetworkId player)
    {
        if (Object.HasStateAuthority)
        {
            UpdateRank(player);
        }
        else
        {
            RPC_RequestUpdateRank(player);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]

    public void RPC_RequestUpdateRank(NetworkId player)
    {
        UpdateRank(player);
    }

    public void UpdateRank(NetworkId player)
    {
        if (isGameOver) return;

        if (!playerRanks.Contains(player))
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
            RPC_ShowGameOverPanel();

            //if (playerRanks.Count >= 2)
            {
                RPC_SpawnRewardAvatar();
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
        globalVolume.SetActive(true);
        //Play SFX
        yield return new WaitForSecondsRealtime(2f);
        globalVolume.SetActive(false);
        gameOverPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(6f);
        yield return StartCoroutine(FadeBlackScreen(0, 1));
        yield return new WaitForSecondsRealtime(3f);

        if (HasStateAuthority)
            Runner.LoadScene("TuanSceneMap");
    }

    bool CheckGameOver()
    {
        return playerRanks.Count >= Runner.ActivePlayers.Count();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ShowGameOverPanel()
    {
        StartCoroutine(ReturnToBoard());
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void RPC_SpawnRewardAvatar()
    {
        FindFirstObjectByType<CinemachineCamera>().enabled = false;

        for (int i = 0; i < playerRanks.Count; i++)
        {
            NetworkObject iRankObject = Runner.FindObject(playerRanks[i]);
            NetworkCharacterController iCc = iRankObject.GetComponent<NetworkCharacterController>();
            iCc.gravity = 0;
            iCc.jumpImpulse = 0;

            if (HasStateAuthority)
            {
                iCc.Teleport(rankPositions[i].position, Quaternion.Euler(0, -90, 0));
            }

            MNGCauKinhController iCk = iRankObject.GetComponent<MNGCauKinhController>();

            Animator iAnimator = iRankObject.GetComponent<Animator>();

            if (iCk.isGoal)
            {
                if (i == 0) iAnimator.Play("Win");
                else iAnimator.Play("Lose");
            }
            else
            {
                iAnimator.Play("Lose");
            }
        }
    }
}
