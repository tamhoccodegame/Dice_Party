using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    public bool isGameOver { get; set; } = false;

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
    public TextMeshProUGUI whoWinsText;
    public GameObject gameOverVolume;

    public Image blackScreen;

    public float fadeDuration = 1f;

    protected virtual void Awake()
    {
        GetComponent<PlayerSpawner>().SpawnPlayer();
    }

    protected virtual void Start()
    {
        tutorialPanel.SetActive(true);
        HideTutorial();
        InitHUD();
    }

    void InitHUD()
    {
        for(int i = 0; i < PlayerManager.instance.players.Count; i++)
        {
            playerTextUI[i].transform.parent.gameObject.SetActive(true);
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

        yield return new WaitForSeconds(5f);

        if(introCutscene != null && introCutscene.gameObject.activeSelf)
        {
            introCutscene.Play();
            introCutscene.stopped += StartGame;
        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeBlackScreen(1, 0));
        TriggerAfterTutorial();
    }

    protected virtual void TriggerAfterTutorial()
    {

    }

    private void StartGame(PlayableDirector obj)
    {
        Destroy(obj.gameObject);
        //FindFirstObjectByType<GlobalVolume>().StartFadeOut();

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
        
    }

    public virtual void UpdateHUD()
    {
        
    }
}
