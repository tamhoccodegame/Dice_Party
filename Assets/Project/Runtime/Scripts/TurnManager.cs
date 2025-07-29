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
    public bool isFirstTry { get; set; } = false;

    [Header("BXH")]
    public Transform slotTemplate;
    public Transform slotContainer;

    public TextMeshProUGUI turnNotifyText;

    public PlayableDirector introCutscene;

    public Image blackScreen;
    public float fadeDuration = 1f;

    [Header("Demo")]
    public Transform chestGold;

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GetComponent<PlayerSpawner>().SpawnPlayer();

        MusicManager.instance.PlayMusic(music);
        StartCoroutine(FadeBlackScreen(1, 0));

        if (isFirstTry)
        {
            if (introCutscene.gameObject.activeSelf) StartCoroutine(DelayPlayIntroCutscene());
            else
            {

                //ShowChestGoldAndStartFirstTurn();

                StartCoroutine(DelayUpdatePlayerUI());



                UpdatePlayerDataUI();
            }
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
        FindFirstObjectByType<GlobalVolume>().StartFadeOut();

        ShowChestGoldAndStartFirstTurn();
        if (isFirstTry)
        {
            StartCoroutine(DelayUpdatePlayerUI());
        }

        UpdatePlayerDataUI();
    }

    void ShowChestGoldAndStartFirstTurn()
    {
        StartCoroutine(ShowChestGoldAndStartFirstTurnCoroutine());
    }

    IEnumerator ShowChestGoldAndStartFirstTurnCoroutine()
    {
        //CameraFollow.instance.RPC_StartFollowTarget(chestGold.GetComponent<NetworkObject>().Id);
        yield return new WaitForSecondsRealtime(3f);
        chestGold.GetComponent<ChestGoldNode>().chest.Play("FlyDown");
        yield return new WaitForSecondsRealtime(3f);
        StartFirstTurn();
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

        foreach (var kvp in dictCopy)
        {
            RectTransform slotRect = Instantiate(slotTemplate, slotContainer).GetComponent<RectTransform>();
            slotRect.gameObject.SetActive(true);

            BoardSlotRect boardSlotRect = slotRect.GetComponent<BoardSlotRect>();

            boardSlotRect.UpdateCup(kvp.Value.cupQty);
            boardSlotRect.UpdateKey(kvp.Value.keyQty);
            boardSlotRect.UpdateHealth(kvp.Value.health);
            
            int index = PlayerManager.instance.players.IndexOf(kvp.Key);
            boardSlotRect.UpdateName($"Player {index + 1}");
        }
        #endregion
    }

    #endregion


    #region Turn

    void StartFirstTurn()
    {
        currentPlayerIndex = 0;
        playerControllers.ElementAt(currentPlayerIndex).Value.enabled = true;
        playerControllers.ElementAt(currentPlayerIndex).Value.StartTurn();
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

    void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(2f);
        LevelLoader.instance.LoadScene(sceneName);
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

    #endregion

}