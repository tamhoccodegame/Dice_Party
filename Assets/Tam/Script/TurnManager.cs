using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public List<NewBoardGameController> playerController;
    public int currentPlayerIndex { get; set; }
     public bool isFirstTry { get; set; } = true;

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

        GetComponent<PlayerSpawner>().SpawnPlayer();
        MusicManager.instance.PlayMusic(MusicManager.MusicType.Board);
        StartCoroutine(FadeBlackScreen(1, 0));

        playerController = FindObjectsByType<NewBoardGameController>(FindObjectsSortMode.InstanceID).ToList();

        if (isFirstTry)
        {
            if (introCutscene.gameObject.activeSelf) StartCoroutine(DelayPlayIntroCutscene());
            else
            {
                
                    //ShowChestGoldAndStartFirstTurn();
                    if (isFirstTry)
                    {
                        StartCoroutine(DelayUpdatePlayerUI());

                        
                    }

                    UpdatePlayerDataUI();
            }
        }
        else
        {
           
                StartFirstTurn();

                if (isFirstTry)
                {
                    StartCoroutine(DelayUpdatePlayerUI());

                    //foreach (var player in NetworkManager.instance.GetAllPlayers())
                    //{
                    //    BoardGameData.instance.UpdateItem(player, new ElectricGun());
                    //}
                }

                UpdatePlayerDataUI();
        }

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
        //Debug.Log("playersBoardStat count: " + BoardGameData.instance.playersBoardStat.Count);

        //Dictionary<PlayerRef, BoardGameStat> dictCopy = BoardGameData.instance.playersBoardStat;
        //dictCopy.OrderByDescending(d => d.Value.cupQty);

        //foreach (var kvp in dictCopy)
        //{
        //    RectTransform slotRect = Instantiate(slotTemplate, slotContainer).GetComponent<RectTransform>();
        //    slotRect.gameObject.SetActive(true);

        //    BoardSlotRect boardSlotRect = slotRect.GetComponent<BoardSlotRect>();

        //    boardSlotRect.UpdateCup(kvp.Value.cupQty);
        //    boardSlotRect.UpdateKey(kvp.Value.keyQty);
        //    boardSlotRect.UpdateHealth(kvp.Value.health);

        //    string playerName = BoardGameData.instance.GetName(kvp.Key);

        //    if (string.IsNullOrEmpty(playerName))
        //        boardSlotRect.UpdateName(kvp.Key.PlayerId.ToString());
        //    else
        //        boardSlotRect.UpdateName(playerName);
        //}
        #endregion
    }

    #endregion


    #region Turn

    void StartFirstTurn()
    {
            currentPlayerIndex = 0;

        playerController[currentPlayerIndex].StartTurn();
        UpdateTurnUI();
    }

    public bool CheckWin()
    {
        BoardGameData data = BoardGameData.instance;
        
        return false;
    }
  
    public void NextTurn()
    {
            currentPlayerIndex = (currentPlayerIndex + 1) % playerController.Count;
            if (currentPlayerIndex == 0)
            {
                LoadScene(isFirstTry ? "MNG3" : "MNG3");
            }
            //CameraFollow.instance.RPC_StartFollowTarget(playerController[currentPlayerIndex].Object.Id);
        
        if (currentPlayerIndex != 0)
        {
            playerController[currentPlayerIndex].StartTurn();
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
        yield return null;
        LevelLoader.instance.LoadScene(sceneName);
    }
    #endregion

}