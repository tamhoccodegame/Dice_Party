using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class VongXoayManager : NetworkBehaviour
{
    public static VongXoayManager instance;

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkDictionary<NetworkId, int> playerLives => default;

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkLinkedList<NetworkId> playerRanks => default;

    [Networked]
    public bool isGameOver { get; set; } = false;

    [Networked]
    public bool isGameStarted { get; set; } = false;

    public PlayableDirector introCutscene;

    [Header("Avatar Standing Position")]
    public Transform[] rankPositions;

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
        MusicManager.instance.PlayMusic(MusicManager.MusicType.MNG);
        instance = this;
        tutorialPanel.SetActive(true);

        if (Object.HasStateAuthority)
        {
            RPC_HideTutorial();
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
        GetComponent<PlayerSpawner>().SpawnPlayer();
        introCutscene.Play();
        introCutscene.stopped += StartGame;
        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(FadeBlackScreen(1, 0));

    }

    private void StartGame(PlayableDirector obj)
    {
        Destroy(obj.gameObject);
        FindFirstObjectByType<GlobalVolume>().StartFadeOut();
        if (Object.HasStateAuthority)
        {
            isGameStarted = true;
        }
    }

    public void RequestUpdateLive(NetworkId player)
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
    public void RPC_RequestUpdateLive(NetworkId player)
    {
        UpdateLive(player);
    }

    private void UpdateLive(NetworkId player)
    {
        if (isGameOver) return;

        if (playerLives.TryGet(player, out int value))
        {
            playerLives.Set(player, value - 1);
        }
        else
        {
            playerLives.Add(player, 3);
            RPC_UpdateUILive();
            return;
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
            RPC_ShowGameOverPanel();

            //if (playerRanks.Count >= 2)
            playerRanks.Reverse();

            RPC_SpawnRewardAvatar();
        }
    }

    IEnumerator ReturnToBoard()
    {
        //Volume active
        yield return new WaitForSecondsRealtime(1.5f);
        gameOverPanel.SetActive(true);
        //Volume deactive
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
        StartCoroutine(ReturnToBoard());
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void RPC_SpawnRewardAvatar()
    {
        for (int i = 0; i < playerRanks.Count; i++)
        {
            NetworkObject iRankObject = Runner.FindObject(playerRanks[i]);
            MNGVongXoayController iCtrl = iRankObject.GetComponent<MNGVongXoayController>();
            iCtrl.enabled = false;
            NetworkCharacterController iCc = iRankObject.GetComponent<NetworkCharacterController>();
            iCc.gravity = 0;
            iCc.jumpImpulse = 0;

            if (HasStateAuthority)
            {
                iCc.Teleport(rankPositions[i].position, Quaternion.Euler(0, -90, 0));
            }

            Animator iAnimator = iRankObject.GetComponent<Animator>();

            if (i == 0) iAnimator.Play("Win");
            else iAnimator.Play("Lose");
        }
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
