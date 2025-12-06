using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class GlassCouple
{
    public BreakGlass glass1;
    public BreakGlass glass2;
}

public class GlassBreakManager : MonoBehaviour
{
    public AudioClip music;

    public static GlassBreakManager instance;
    public GlassCouple[] glassCouples;

    public PlayableDirector introCutscene;

    public Image blackScreen;

    public CinemachineCamera cam;

    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;

    public TextMeshProUGUI countDownText;

    public int time { get; set; }

    public Dictionary<int, int> playerScores = new();
    public Dictionary<int, GameObject> playerRanks => default;

    [Header("Avatar Standing Position")]
    public Transform[] rankPositions;

    public TextMeshProUGUI[] playerTextUI;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public GameOverSlotUI[] gameOverSlots;
    public TextMeshProUGUI whoWinsText;
    public GameObject gameOverVolume;

    public Transform spawnPosition;

    public bool isGameOver { get; set; } = false;

    public bool isGameStarted { get; set; } = false;

    public float fadeDuration = 1f;

    public void FadeIn() => StartCoroutine(FadeBlackScreen(0, 1));
    public void FadeOut() => StartCoroutine(FadeBlackScreen(1, 0));

    public void Awake()
    {
        cam.enabled = false; 
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

        countDownText.text = time.ToString();
        InvokeRepeating(nameof(CountDown), 0f, 1f);

        MusicManager.instance.PlayMusic(music);
        instance = this;
        tutorialPanel.SetActive(true);

      
            HideTutorial();
            InitPlayersScore();
    }

    void InitPlayersScore()
    {
       
    }

    void CountDown()
    {
        if (!isGameStarted || isGameOver) return;

            time -= 1;

            CheckTimeOut();
        countDownText.text = time.ToString();
    }

    void CheckTimeOut()
    {
        if(time <= 0)
        {
            foreach (var p in PlayerSpawner.instance.GetSpawnedCharacters())
            {
            }
        }

    }
    public void UpdateRank()
    {
        if (isGameOver) return;

        //if (!playerRanks.ContainsKey(playerRef))
        //{
        //    playerRanks.Add(playerRef, playerObject);
        //}

        //if (Object.HasStateAuthority)
        //{
        //    RPC_UpdateUILive(playerObject);
        //}

        //if (CheckGameOver())
        //{
        //    isGameOver = true;
        //    RPC_ShowGameOverPanel();

        //    //if (playerRanks.Count >= 2)
        //    {
        //        RPC_SpawnRewardAvatar();
        //    }
        //}
    }

    void UpdateUILive()
    {
        for(int i = 0; i < playerScores.Count; i++)
        {
            //if (playerScores.ElementAt(i).Key == playerId)
            //    playerTextUI[i].text = playerScores.ElementAt(i).Value.ToString();
            //else continue;
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

    void HideTutorial()
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

        GetComponent<PlayerSpawner>().TrySpawnPlayer();
    }

    private void StartGame(PlayableDirector obj)
    {
        Destroy(obj.gameObject);
        FindFirstObjectByType<GlobalVolume>().StartFadeOut();
            isGameStarted = true;
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

            SceneManager.LoadScene("TuanSceneMap");
    }

    bool CheckGameOver()
    {
        return false;
        //return playerRanks.Count >= Runner.ActivePlayers.Count();
    }

    void RPC_ShowGameOverPanel()
    {
        StartCoroutine(ReturnToBoard());
    }

    public void SpawnRewardAvatar()
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
            //NetworkObject iRankObject = Runner.FindObject(playerRanks.ElementAt(i).Value);
            //NetworkCharacterController iCc = iRankObject.GetComponent<NetworkCharacterController>();
            //iCc.gravity = 0;
            //iCc.jumpImpulse = 0;
            //iCc.acceleration = 0;
            //iCc.maxSpeed = 0;

            //if (HasStateAuthority)
            //{
            //    iCc.Teleport(rankPositions[i].position, Quaternion.Euler(0, -90, 0));
            //}

            //MNGCauKinhController iCk = iRankObject.GetComponent<MNGCauKinhController>();
            //iCk.enabled = false;
            //Animator iAnimator = iRankObject.GetComponent<Animator>();

            //if (iCk.isGoal)
            //{
            //    Debug.Log(i);
            //    if (i == 0) iAnimator.Play("Win");
            //    else iAnimator.Play("Lose");
            //}
            //else
            //{
            //    iAnimator.Play("Lose");
            //}
            #endregion

            #region UISlot
            //gameOverSlots[i].gameObject.SetActive(true);
            //gameOverSlots[i].keyQtyText.text = "10";
            //gameOverSlots[i].rankText.text = $"{i + 1}";

            //string playerName = BoardGameData.instance.GetName(playerRanks.ElementAt(i).Key);
            //gameOverSlots[i].nameText.text = playerName;
            #endregion
        }


    }
}
