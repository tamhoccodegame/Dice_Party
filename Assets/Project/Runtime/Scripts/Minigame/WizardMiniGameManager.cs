using System.Collections;
using System.Collections.Generic;
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

    public PlayableDirector introCutscene;
    public AudioClip music;

    [Header("Avatar Standing Position")]
    public Transform[] rankPositions;
    public Transform[] avatarHUDPositions;

    public GameObject emptyComponentAvatarPrefab;

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

    public Dictionary<PlayerInput, int> playerInitLives = new Dictionary<PlayerInput, int>();

    public Dictionary<PlayerInput, GameObject> playerObjects = new Dictionary<PlayerInput, GameObject>();

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        GetComponent<PlayerSpawner>().SpawnPlayer();
        MusicManager.instance.PlayMusic(music);
        tutorialPanel.SetActive(true);
        HideTutorial();
        InitHUD();
    }

    void InitHUD()
    {
        for(int i = 0; i < PlayerManager.instance.players.Count; i++)
        {
            Sprite playerAvatar = AvatarLoader.instance.GetAvatarSprite(i);

            if(playerAvatar == null)
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

        if(introCutscene != null && introCutscene.gameObject.activeSelf)
        {
            introCutscene.Play();
            introCutscene.stopped += StartGame;
        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeBlackScreen(1, 0));
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

        SceneManager.LoadScene("Map1");
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
        
    }

    public virtual void UpdateHUD()
    {
        
    }
}
