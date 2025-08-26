using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerSlotHUD
{
    public TextMeshProUGUI textUI;
    public Image avatar;
}

public class WizardMiniGameManager : MonoBehaviour
{
    public static WizardMiniGameManager instance;

    public bool isGameOver { get; set; } = false;
    public bool isGameStarted { get; set; } = false;

    [Space(20)]
    public bool isAllReady = false;

    [Space(20)]
    [Header("UIs & Sounds")]
    public int time;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI startText;
    public AudioSource startSound;
    public PlayableDirector introCutscene;
    public AudioClip music;
    public AudioClip winMusic;

    [Space(20)]
    [Header("Avatar Standing Position")]
    public Transform[] rankPositions;

    public PlayerSlotHUD[] playerHUDs;

    [Space(20)]
    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;
    public Image[] playerAvatarReady;
    public TextMeshProUGUI[] playerReadyText;
    public Dictionary<PlayerInput, bool> playersReadyStatus = new Dictionary<PlayerInput, bool>();

    [Space(20)]
    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public GameOverSlotUI[] gameOverSlots;
    public TextMeshProUGUI whoWinsText;
    public GameObject gameOverVolume;
    public Image blackScreen;
    public float fadeDuration = 1f;

    [Space(20)]
    [Header("Players Condition")]
    public Dictionary<PlayerInput, int> playerScores = new Dictionary<PlayerInput, int>();

    //Từng player tự đăng ký vô
    public Dictionary<PlayerInput, GameObject> playerObjects = new Dictionary<PlayerInput, GameObject>();

    public List<PlayerInput> playersCompleteGame = new List<PlayerInput>();

    protected virtual void Awake()
    {
        instance = this;
    }

    protected virtual void Start()
    {
        GetComponent<PlayerSpawner>().SpawnPlayer();
        MusicManager.instance.PlayMusic(music);
        ShowTutorial();
        InitHUD();
        InitReadyStatus();

        if (time != -1)
        {
            InvokeRepeating(nameof(CountDown), 0f, 1f);
        }
        else if (timeText != null)
        {
            timeText.transform.parent.gameObject.SetActive(false);
        }

        foreach (var player in playerObjects.Keys)
        {
            playerScores.Add(player, 1000); //Mỗi player khởi đầu 1k điểm
        }

        UpdateHUD();
    }

    protected void InitReadyStatus()
    {
        var players = PlayerManager.instance.players;
        for(int i = 0; i < players.Count; i++)
        {
            playerReadyText[i].gameObject.SetActive(false);
            playersReadyStatus.Add(players[i], false);
        }
    }

    public void UpdatePlayerCompletedGame(PlayerInput input)
    {
        playersCompleteGame.Add(input);
        if (CheckGameOver())
        {
            ShowGameOverPanel();
        }
    }

    public void UpdatePlayerScore(PlayerInput input, int ammount)
    {
        playerScores[input] += ammount;

        playerScores[input] = Mathf.Max(0, playerScores[input]);
        UpdateHUD();

        if (CheckGameOver())
        {
            ShowGameOverPanel();
        }
    }

    protected void CountDown()
    {
        if (time <= 0 || !isGameStarted || isGameOver) return;
        time -= 1;
        time = Mathf.Max(time, 0);
        timeText.text = time.ToString();

        if (CheckGameOver())
        {
            ShowGameOverPanel();
        }
    }

    void InitHUD()
    {
        for (int i = 0; i < PlayerManager.instance.players.Count; i++)
        {
            Sprite playerAvatar = AvatarLoader.instance.GetAvatarSprite(i);

            if (playerAvatar == null)
            {
                Debug.LogError("Cannot find Player Avatar");
                return;
            }

            playerHUDs[i].avatar.sprite = playerAvatar;
            playerHUDs[i].textUI.transform.parent.gameObject.SetActive(true);

            playerAvatarReady[i].sprite = playerAvatar;
            playerAvatarReady[i].transform.parent.gameObject.SetActive(true);
        }
    }

    protected void ShowTutorial()
    {
        StartCoroutine(DelayShowTutorial());
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

    protected IEnumerator DelayShowTutorial()
    {
        tutorialPanel.SetActive(false);

        yield return StartCoroutine(FadeBlackScreen(1, 0));

        if (introCutscene != null && introCutscene.gameObject.activeSelf)
        {
            introCutscene.Play();
            introCutscene.stopped += StartGame;
        }
        else
        {
            TriggerAfterCutscene();
        }
    }

    protected virtual void TriggerAfterCutscene()
    {
        tutorialPanel.SetActive(true);
        StartCoroutine(WaitForAllReady());
    }

    IEnumerator WaitForAllReady()
    {
        while (!isAllReady)
        {
            for(int i = 0; i < playersReadyStatus.Count; i++)
            {
                PlayerInput playerInput = playersReadyStatus.ElementAt(i).Key;
                if (playerInput.actions["Confirm"].triggered && !playersReadyStatus[playerInput])
                {
                    playersReadyStatus[playerInput] = true;
                    playerReadyText[i].gameObject.SetActive(true);
                }
            }

            isAllReady = playersReadyStatus.Values.All(r => r);

            yield return null;
        }

        yield return new WaitForSeconds(2.5f);
        tutorialPanel.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        startText.gameObject.SetActive(true);
        startSound.Play();

        yield return new WaitForSeconds(2f);

        startText.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        isGameStarted = true;
    }

    private void StartGame(PlayableDirector obj)
    {
        TriggerAfterCutscene();
        Destroy(obj.gameObject);
    }

    IEnumerator ReturnToBoard()
    {
        //Volume active
        //gameOverVolume.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SpawnRewardAvatar();
        yield return null;
        gameOverPanel.SetActive(true);
        //gameOverVolume.SetActive(false);
        yield return new WaitForSeconds(6f);
        yield return StartCoroutine(FadeBlackScreen(0, 1));
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("TuanSceneMap");
    }

    public virtual bool CheckGameOver()
    {
        return false;
    }

    public virtual void ShowGameOverPanel()
    {
        StartCoroutine(ReturnToBoard());
    }

    public virtual void SpawnRewardAvatar()
    {
        FindFirstObjectByType<Light>().shadows = LightShadows.None;

        playerScores = playerScores
                       .OrderByDescending(c => c.Value)
                       .ToDictionary(c => c.Key, c => c.Value);


        int keyAdd = 8;

        for (int i = 0; i < playerScores.Count; i++)
        {
            WizardPartyData.instance.UpdatePlayerKey(playerScores.ElementAt(i).Key, keyAdd);
            gameOverSlots[i].keyQtyText.text = keyAdd.ToString();
            keyAdd -= 2;
            gameOverSlots[i].gameObject.SetActive(true);
            var inputGo = playerObjects[playerScores.ElementAt(i).Key];
            if (i > 1) inputGo.GetComponent<Animator>().Play($"Lose{Random.Range(1, 4)}");
            else inputGo.GetComponent<Animator>().Play($"Win{Random.Range(1, 6)}");

            inputGo.GetComponent<PlayerController>().enabled = false;
            inputGo.GetComponent<CharacterController>().enabled = false;
            inputGo.transform.position = rankPositions[i].position;
            inputGo.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }

    public virtual void UpdateHUD()
    {
        List<PlayerInput> inputs = PlayerManager.instance.players;

        for (int i = 0; i < inputs.Count; i++)
        {
            int score = playerScores[inputs[i]];
            playerHUDs[i].textUI.text = score.ToString();
        }

        if (CheckGameOver())
        {
            ShowGameOverPanel();
        }

    }
}
