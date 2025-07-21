using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VongXoayManager : MonoBehaviour
{
    public static VongXoayManager instance;

    public Dictionary<int, int> playerLives = new Dictionary<int, int>();

    public Dictionary<int, GameObject> playerRanks = new Dictionary<int, GameObject>();

    public bool isGameOver { get; set; } = false;

    public bool isGameStarted { get; set; } = false;

    public PlayableDirector introCutscene;

    [Header("Avatar Standing Position")]
    public Transform[] rankPositions;

    public TextMeshProUGUI[] playerLiveTextUI;

    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public GameOverSlotUI[] gameOverSlots;
    public TextMeshProUGUI whoWinsText;
    public GameObject gameOverVolume;

    public Image blackScreen;

    public float fadeDuration = 1f;

    public void FadeIn() => StartCoroutine(FadeBlackScreen(0, 1));
    public void FadeOut() => StartCoroutine(FadeBlackScreen(1, 0));

    public void Awake()
    {
        //MusicManager.instance.PlayMusic(MusicManager.MusicType.MNG);
        instance = this;
        tutorialPanel.SetActive(true);

        HideTutorial();
        InitPlayerLivesUI();
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

    void InitPlayerLivesUI()
    {

    }

    void HideTutorial()
    {
        StartCoroutine(HideTutorialCouroutine());
    }

    IEnumerator HideTutorialCouroutine()
    {
        yield return new WaitForSeconds(10f);

        yield return StartCoroutine(FadeBlackScreen(0, 1));
        tutorialPanel.SetActive(false);

        yield return new WaitForSeconds(5f);
        GetComponent<PlayerSpawner>().SpawnPlayer();
        introCutscene.Play();
        introCutscene.stopped += StartGame;
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeBlackScreen(1, 0));

    }

    private void StartGame(PlayableDirector obj)
    {
        Destroy(obj.gameObject);
        //FindFirstObjectByType<GlobalVolume>().StartFadeOut();

        isGameStarted = true;
    }
    private void UpdateLive()
    {
        if (isGameOver) return;

        //if (playerLives.TryGet(playerObject, out int value))
        //{
        //    playerLives.Set(playerObject, value - 1);
        //}
        //else
        //{
        //    playerLives.Add(playerObject, 3);
        //}

        //if (playerLives[playerObject] <= 0 && !playerRanks.ContainsKey(playerRef))
        //{
        //    playerRanks.Add(playerRef, playerObject);
        //}

        //if (Object.HasStateAuthority)
        //{
        //    RPC_UpdateUILive();
        //}

        //if (CheckGameOver())
        //{
        //    isGameOver = true;
        //    RPC_ShowGameOverPanel();

        //    //if (playerRanks.Count >= 2)
        //    playerRanks.Reverse();
        //}
    }

    IEnumerator ReturnToBoard()
    {
        //Volume active
        gameOverVolume.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SpawnRewardAvatar();
        yield return null;
        gameOverPanel.SetActive(true);
        gameOverVolume.SetActive(false);
        yield return new WaitForSeconds(6f);
        yield return StartCoroutine(FadeBlackScreen(0, 1));
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("TuanSceneMap");
    }

    bool CheckGameOver()
    {
        return playerLives.All(kvp => kvp.Value <= 0);
    }

    void ShowGameOverPanel()
    {
        StartCoroutine(ReturnToBoard());
    }

    public void SpawnRewardAvatar()
    {
        //whoWinsText.text = BoardGameData.instance.GetName(playerRanks.ElementAt(0).Key) + " Wins";
        for (int i = 0; i < playerRanks.Count; i++)
        {
            #region Player
            //NetworkObject iRankObject = Runner.FindObject(playerRanks.ElementAt(i).Value);
            //NetworkCharacterController iCc = iRankObject.GetComponent<NetworkCharacterController>();
            //iCc.gravity = 0;
            //iCc.jumpImpulse = 0;

            //if (HasStateAuthority)
            //{
            //    iCc.Teleport(rankPositions[i].position, Quaternion.Euler(0, -90, 0));
            //}

            //Animator iAnimator = iRankObject.GetComponent<Animator>();

            //if (i == 0) iAnimator.Play("Win");
            //else iAnimator.Play("Lose");
            #endregion

            #region UISlot
            //gameOverSlots[i].gameObject.SetActive(true);
            //gameOverSlots[i].keyQtyText.text = "10";
            //gameOverSlots[i].rankText.text = $"{i + 1}";

            //string playerName = BoardGameData.instance.GetName(playerRanks.ElementAt(i).Key);
            //gameOverSlots[i].nameText.text = playerName;
            //#endregion

            //#region Reward
            //BoardGameData data = BoardGameData.instance;
            //if(data != null)
            //{
            //    int rewardKeyQty = i == 0 ? 8 : 4;
            //    data.UpdateKey(iRankObject.InputAuthority, rewardKeyQty);

            //    //BoardItem boardItem = new ElectricGun();
            //    //data.UpdateItem(iRankObject.InputAuthority, boardItem);
            //}
            #endregion
        }
    }

    void UpdateUILive()
    {
        int index = 0;
        foreach (var kvp in playerLives)
        {
            if (index < playerLiveTextUI.Length)
            {
                playerLiveTextUI[index].text = kvp.Value.ToString();
                index++;
            }
        }
    }
}
