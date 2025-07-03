using Fusion;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
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

    public PlayableDirector introCutscene;

    public Image blackScreen;

    public CinemachineCamera cam;

    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;

    public TextMeshProUGUI countDownText;

    [Networked] public int time { get; set; }

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkDictionary<NetworkId, int> playerScores => default;

    [Networked]
    [Capacity(4)]
    [UnitySerializeField]
    public NetworkDictionary<PlayerRef, NetworkId> playerRanks => default;

    [Header("Avatar Standing Position")]
    public Transform[] rankPositions;

    public TextMeshProUGUI[] playerTextUI;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public GameOverSlotUI[] gameOverSlots;
    public TextMeshProUGUI whoWinsText;
    public GameObject gameOverVolume;

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
        cam.enabled = false; 
        if (Object.HasStateAuthority)
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
            RPC_InitPlayerScore();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_InitPlayerScore()
    {
        for(int i = 0; i < NetworkManager.instance.GetAllPlayers().Count; i++)
        {
            playerTextUI[i].transform.parent.gameObject.SetActive(true);
        }
    }

    void CountDown()
    {
        if (!isGameStarted || isGameOver) return;

        if (HasStateAuthority)
        {
            time -= 1;

            CheckTimeOut();
        }
        countDownText.text = time.ToString();
    }

    void CheckTimeOut()
    {
        if(time <= 0)
        {
            foreach (var p in PlayerSpawner.instance.GetSpawnedCharacters())
            {
                UpdateRank(p.Key, p.Value);
            }
        }

    }

    //Khi mà đến đích thì sẽ gọi hàm này
    public void RequestAddRank(PlayerRef playerRef, NetworkId playerObject)
    {
        if (Object.HasStateAuthority)
        {
            UpdateRank(playerRef, playerObject);
        }
        else
        {
            RPC_RequestUpdateRank(playerRef, playerObject);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]

    public void RPC_RequestUpdateRank(PlayerRef playerRef, NetworkId playerObject)
    {
        UpdateRank(playerRef, playerObject);
    }

    public void UpdateRank(PlayerRef playerRef, NetworkId playerObject)
    {
        if (isGameOver) return;

        if (!playerRanks.ContainsKey(playerRef))
        {
            playerRanks.Add(playerRef, playerObject);
        }

        if (Object.HasStateAuthority)
        {
            RPC_UpdateUILive(playerObject);
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
    void RPC_UpdateUILive(NetworkId playerId)
    {
        for(int i = 0; i < playerScores.Count; i++)
        {
            if (playerScores.ElementAt(i).Key == playerId)
                playerTextUI[i].text = playerScores.ElementAt(i).Value.ToString();
            else continue;
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
        introCutscene.Play();
        introCutscene.stopped += StartGame;
        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(FadeBlackScreen(1, 0));

        GetComponent<PlayerSpawner>().SpawnPlayer();
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

    IEnumerator ReturnToBoard()
    {
        gameOverVolume.SetActive(true);
        //Play SFX
        yield return new WaitForSecondsRealtime(2f);
        gameOverVolume.SetActive(false);
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SpawnRewardAvatar()
    {
        StartCoroutine(SpawnRewardAvatarDelayed());
    }

    IEnumerator SpawnRewardAvatarDelayed()
    {
        yield return new WaitForSeconds(0.2f); // Hoặc vài frame nhỏ

        #region Player
        GameObject.Find("FreeLook Camera").SetActive(false);
        for (int i = 0; i < playerRanks.Count; i++)
        {
            NetworkObject iRankObject = Runner.FindObject(playerRanks.ElementAt(i).Value);
            NetworkCharacterController iCc = iRankObject.GetComponent<NetworkCharacterController>();
            iCc.gravity = 0;
            iCc.jumpImpulse = 0;
            iCc.acceleration = 0;
            iCc.maxSpeed = 0;

            if (HasStateAuthority)
            {
                iCc.Teleport(rankPositions[i].position, Quaternion.Euler(0, -90, 0));
            }

            MNGCauKinhController iCk = iRankObject.GetComponent<MNGCauKinhController>();
            iCk.enabled = false;
            Animator iAnimator = iRankObject.GetComponent<Animator>();

            if (iCk.isGoal)
            {
                Debug.Log(i);
                if (i == 0) iAnimator.Play("Win");
                else iAnimator.Play("Lose");
            }
            else
            {
                iAnimator.Play("Lose");
            }
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
}
