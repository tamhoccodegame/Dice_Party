using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public AudioClip music;
    public static TurnManager instance;
    public Dictionary<PlayerInput, NewBoardGameController> playerControllers = new Dictionary<PlayerInput, NewBoardGameController>();
    public int currentPlayerIndex { get; set; }
    public bool isFirstTry { get; set; } = true;

    [Header("BXH")]
    public Transform slotTemplate;
    public Transform slotContainer;

    public TextMeshProUGUI turnNotifyText;

    public PlayableDirector introCutscene;

    public Image blackScreen;
    public float fadeDuration = 1f;

    public Transform[] chestGolds;

    bool isGoldChestOpened = true;


    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GetComponent<PlayerSpawner>().TrySpawnPlayer();
        AvatarTurnManager.instance.gameObject.SetActive(false);
        MusicManager.instance?.PlayMusic(music);
        StartCoroutine(FadeBlackScreen(1, 0));

        isGoldChestOpened = WizardPartyData.instance.isGoldChestOpened;

        isFirstTry = WizardPartyData.instance.isFirstTry;

        if (isFirstTry)
        {
            isFirstTry = false;
            WizardPartyData.instance.isFirstTry = false;
            if (introCutscene.gameObject.activeSelf) StartCoroutine(DelayPlayIntroCutscene());
        }
        else
        {
            StartFirstTurn();
            StartCoroutine(DelayUpdatePlayerUI());
            UpdatePlayerDataUI();
        }
    }

    public void UpdateController(PlayerInput playerInput, NewBoardGameController controller)
    {
        playerControllers[playerInput] = controller;
    }

    IEnumerator DelayPlayIntroCutscene()
    {
        yield return new WaitForSecondsRealtime(1f);
        PlayIntroCutscene();
    }

    void PlayIntroCutscene()
    {
        introCutscene.Play();
        introCutscene.stopped += IntroCutscene_stopped;
    }

    private void IntroCutscene_stopped(PlayableDirector obj)
    {
        Destroy(obj.gameObject);

        StartFirstTurn();
        StartCoroutine(DelayUpdatePlayerUI());
        UpdatePlayerDataUI();
    }

    void ShowChestGoldAndStartFirstTurn()
    {
        StartCoroutine(ShowChestGoldAndStartFirstTurnCoroutine());
    }

    IEnumerator ShowChestGoldAndStartFirstTurnCoroutine()
    {
        int chestIndex = Random.Range(0, chestGolds.Length);
        WizardPartyData.instance.currentChestIndex = chestIndex; // GHI NHỚ rương chính xác

        // Các rương còn lại bay lên
        for (int i = 0; i < chestGolds.Length; i++)
        {
            if (i == chestIndex) continue;
            chestGolds[i].GetComponent<ChestGoldNode>().chest.Play("FlyUp");
        }

        // Camera chỉ follow đúng rương cần bay xuống
        CameraFollow.instance.StartFollowTarget(chestGolds[chestIndex]);
        Debug.Log("🎥 Camera follow ô chứa rương bay xuống: " + chestIndex);

        yield return new WaitForSecondsRealtime(5f);

        // Rương chính bay xuống
        chestGolds[chestIndex].GetComponent<ChestGoldNode>().chest.Play("FlyDown");

        yield return new WaitForSecondsRealtime(3f);
        StartFirstTurn(); // ✅ Giờ đã an toàn gọi lại
    }


    IEnumerator DelayUpdatePlayerUI()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        UpdatePlayerDataUI();
    }

    #region PlayerBoardData

    public void UpdatePlayerDataUI()
    {
        foreach (Transform child in slotContainer)
        {
            if (child == slotTemplate) continue;
            Destroy(child.gameObject);
        }

        #region UpdatePlayerBoardStatUI
        Dictionary<PlayerInput, PlayerBoardStat> dictCopy = WizardPartyData.instance.playersStat;
        dictCopy.OrderByDescending(d => d.Value.cupQty);

        int index = 1;

        foreach (var kvp in dictCopy)
        {
            RectTransform slotRect = Instantiate(slotTemplate, slotContainer).GetComponent<RectTransform>();
            slotRect.gameObject.SetActive(true);

            BoardSlotRect boardSlotRect = slotRect.GetComponent<BoardSlotRect>();

            boardSlotRect.UpdateName($"Player {index}");
            boardSlotRect.UpdateCup(kvp.Value.cupQty);
            boardSlotRect.UpdateKey(kvp.Value.keyQty);
            boardSlotRect.UpdateHealth(kvp.Value.health);

            index++;
        }
        #endregion
    }

    #endregion


    #region Turn

    void StartFirstTurn()
    {
        //if (isGoldChestOpened)
        //{
        //    isGoldChestOpened = false;
        //    WizardPartyData.instance.isGoldChestOpened = false;

        //    ShowChestGoldAndStartFirstTurn(); // Gọi 1 lần duy nhất
        //    return;
        //}
        //else
        //{
        //    int currentChestIndex = WizardPartyData.instance.currentChestIndex;
        //    if (currentChestIndex != -1)
        //    {
        //        chestGolds[currentChestIndex].GetComponent<ChestGoldNode>().chest.Play("FlyDown");
        //    }
        //}

        foreach(var goldChest in FindObjectsByType<ChestGoldNode>(FindObjectsSortMode.None))
        {
            goldChest.chest.Play("FlyDown");
        }

        currentPlayerIndex = 0;
        var player = playerControllers.ElementAt(currentPlayerIndex).Value;
        player.enabled = true;
        player.StartTurn();
        UpdateTurnUI();
    }


    public bool CheckWin()
    {
        BoardGameData data = BoardGameData.instance;

        return false;
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % playerControllers.Count;
        if (currentPlayerIndex == 0)
        {
            LoadScene(WizardPartyData.instance.GetMinigame());
        }
        //CameraFollow.instance.RPC_StartFollowTarget(playerController[currentPlayerIndex].Object.Id);
        if (currentPlayerIndex != 0)
        {
            playerControllers.ElementAt(currentPlayerIndex).Value.enabled = true;
            playerControllers.ElementAt(currentPlayerIndex).Value.StartTurn();
            UpdateTurnUI();
        }
    }

    void UpdateTurnUI()
    {
        //if (currentPlayerRef == Runner.LocalPlayer)
        //{
        //    turnNotifyText.text = "Your Turn";
        //}
        //else
        //{
        //    turnNotifyText.text = $"{playerController[currentPlayerIndex].name}'s Turn";
        //}

        turnNotifyText.gameObject.SetActive(true);
    }

    #endregion


    #region SceneProcess
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

    void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(5f);
        LevelLoader.instance.LoadScene(sceneName);
    }
    #endregion

}