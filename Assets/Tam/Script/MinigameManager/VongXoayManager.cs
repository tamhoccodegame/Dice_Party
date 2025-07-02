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
    public NetworkDictionary<PlayerRef, NetworkId> playerRanks => default;

    [Networked]
    public bool isGameOver { get; set; } = false;

    public GameObject gameOverVolume;

    [Networked]
    public bool isGameStarted { get; set; } = false;

    public PlayableDirector introCutscene;

    [Header("Avatar Standing Position")]
    public Transform[] rankPositions;

    public TextMeshProUGUI[] playerTextUI;

    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public GameOverSlotUI[] gameOverSlots;

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

    public void RequestUpdateLive(PlayerRef playerRef, NetworkId playerObject)
    {
        if (isGameOver) return;

        if (Object.HasStateAuthority)
        {
            UpdateLive(playerRef, playerObject);
        }
        else
        {
            RPC_RequestUpdateLive(playerRef, playerObject);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestUpdateLive(PlayerRef playerRef, NetworkId playerObject)
    {
        UpdateLive(playerRef, playerObject);
    }

    private void UpdateLive(PlayerRef playerRef, NetworkId playerObject)
    {
        if (isGameOver) return;

        if (playerLives.TryGet(playerObject, out int value))
        {
            playerLives.Set(playerObject, value - 1);
        }
        else
        {
            playerLives.Add(playerObject, 3);
            RPC_UpdateUILive();
            return;
        }

        if (playerLives[playerObject] <= 0 && !playerRanks.ContainsKey(playerRef))
        {
            playerRanks.Add(playerRef, playerObject);
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

        }
    }

    IEnumerator ReturnToBoard()
    {
        //Volume active
        gameOverVolume.SetActive(true); 
        yield return new WaitForSecondsRealtime(1.5f);
        RPC_SpawnRewardAvatar();
        yield return null;
        gameOverPanel.SetActive(true);
        gameOverVolume.SetActive(false);
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
            #region Player
            NetworkObject iRankObject = Runner.FindObject(playerRanks.ElementAt(i).Value);
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
            #endregion

            #region UISlot
            gameOverSlots[i].gameObject.SetActive(true);
            gameOverSlots[i].keyQtyText.text = "10";
            gameOverSlots[i].rankText.text = $"{i + 1}";

            string playerName = BoardGameData.instance.GetName(playerRanks.ElementAt(i).Key);
            gameOverSlots[i].nameText.text = playerName;
            #endregion
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
