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

    public int time;
    public TextMeshProUGUI timeText;

    public PlayableDirector introCutscene;
    public AudioClip music;
    public AudioClip winMusic;

    [Header("Avatar Standing Position")]
    public Transform[] rankPositions;
    public Transform[] avatarHUDPositions;

    public PlayerSlotHUD[] playerHUDs;

    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public GameOverSlotUI[] gameOverSlots;
    public TextMeshProUGUI whoWinsText;
    public GameObject gameOverVolume;

    public Image blackScreen;

    public float fadeDuration = 1f;

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
        tutorialPanel.SetActive(true);
        HideTutorial();
        InitHUD();

        if (time != -1)
        {
            InvokeRepeating(nameof(CountDown), 0f, 1f);
        }
        else
        {
            timeText.transform.parent.gameObject.SetActive(false);
        }

            foreach (var player in playerObjects.Keys)
            {
                playerScores.Add(player, 1000); //Mỗi player khởi đầu 1k điểm
            }

        UpdateHUD();
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
        }
    }

    protected void HideTutorial()
    {
        StartCoroutine(HideTutorialCouroutine());
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

    protected IEnumerator HideTutorialCouroutine()
    {
        yield return new WaitForSeconds(10f);

        yield return StartCoroutine(FadeBlackScreen(0, 1));
        tutorialPanel.SetActive(false);

        yield return new WaitForSeconds(2.5f);
        yield return StartCoroutine(FadeBlackScreen(1, 0));

        if (introCutscene != null && introCutscene.gameObject.activeSelf)
        {
            introCutscene.Play();
            introCutscene.stopped += StartGame;
        }
        else
        {
            isGameStarted = true;
        }
    }

    protected virtual void TriggerAfterTutorial()
    {

    }

    private void StartGame(PlayableDirector obj)
    {
        TriggerAfterTutorial();
        Destroy(obj.gameObject);

        isGameStarted = true;
    }

    IEnumerator ReturnToBoard()
    {
        StopAllCoroutines();
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
            keyAdd -= 2;

            gameOverSlots[i].gameObject.SetActive(true);
            var inputGo = playerObjects[playerScores.ElementAt(i).Key];
            if (i > 1) inputGo.GetComponent<Animator>().Play($"Lose{Random.Range(1, 4)}");
            else       inputGo.GetComponent<Animator>().Play($"Win{Random.Range(1, 6)}");
            
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
