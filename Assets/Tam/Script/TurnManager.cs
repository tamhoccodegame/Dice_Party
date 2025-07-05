using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager instance;
    public List<BoardGameController> playerController;
    [Networked] public int currentPlayerIndex { get; set; }
    [Networked] public PlayerRef currentPlayerRef { get; set; }

    [Networked] public bool isFirstTry { get; set; } = true;

    [Header("BXH")]
    public Transform slotTemplate;
    public Transform slotContainer;

    public TextMeshProUGUI turnNotifyText;

    public PlayableDirector introCutscene;

    public enum GameState
    {
        BoardGame,
        MiniGame
    }

    public GameState currentState;

    public Image blackScreen;
    public float fadeDuration = 1f;

    [Header("Demo")]
    public Transform chestGold;

    private void Awake()
    {
        instance = this;
    }

    public override void Spawned()
    {
        GetComponent<PlayerSpawner>().SpawnPlayer();
        MusicManager.instance.PlayMusic(MusicManager.MusicType.Board);
        StartCoroutine(FadeBlackScreen(1, 0));

        playerController = FindObjectsByType<BoardGameController>(FindObjectsSortMode.InstanceID).ToList();

        if (isFirstTry)
        {
            BoardGameData.instance.EnsurePlayerStat(NetworkManager.instance.GetAllPlayers());
            if (HasStateAuthority) StartCoroutine(DelayPlayIntroCutscene());
        }
        else
        {
            if (Object.HasStateAuthority)
            {
                RPC_StartFirstTurn();

                if (isFirstTry)
                {
                    StartCoroutine(DelayUpdatePlayerUI());

                    foreach (var player in NetworkManager.instance.GetAllPlayers())
                    {
                        BoardGameData.instance.UpdateItem(player, new ElectricGun());
                    }
                }

                RPC_UpdatePlayerDataUI();
            }
        }

    }

    IEnumerator DelayPlayIntroCutscene()
    {
        yield return new WaitForSecondsRealtime(1f);
        RPC_PlayIntroCutscene();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_PlayIntroCutscene()
    {
        introCutscene.Play();
        introCutscene.stopped += IntroCutscene_stopped;
    }

    private void IntroCutscene_stopped(PlayableDirector obj)
    {
        Destroy(obj.gameObject);
        FindFirstObjectByType<GlobalVolume>().StartFadeOut();
        if (Object.HasStateAuthority)
        {
            RPC_ShowChestGoldAndStartFirstTurn();
            if (isFirstTry)
            {
                StartCoroutine(DelayUpdatePlayerUI());

                foreach (var player in NetworkManager.instance.GetAllPlayers())
                {
                    BoardGameData.instance.UpdateItem(player, new ElectricGun());
                }
            }

            RPC_UpdatePlayerDataUI();
        }

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ShowChestGoldAndStartFirstTurn()
    {
        StartCoroutine(ShowChestGoldAndStartFirstTurnCoroutine());
    }

    IEnumerator ShowChestGoldAndStartFirstTurnCoroutine()
    {
        if(HasStateAuthority)
        CameraFollow.instance.RPC_StartFollowTarget(chestGold.GetComponent<NetworkObject>().Id);
        yield return new WaitForSecondsRealtime(3f);
        chestGold.GetComponent<ChestGoldNode>().chest.Play("FlyDown");
        yield return new WaitForSecondsRealtime(3f);
        RPC_StartFirstTurn();
    }

    IEnumerator DelayUpdatePlayerUI()
    {
        foreach (var player in NetworkManager.instance.GetAllPlayers())
        {
            RPC_UpdateKey(player, 0);
            RPC_UpdateCup(player, 0);
            RPC_UpdateHealth(player, 30);
        }
        yield return new WaitForSecondsRealtime(0.5f);

        RPC_UpdatePlayerDataUI();
    }

    #region PlayerBoardData
    public void RequestUpdateKey(PlayerRef player, int ammount)
    {
        if (HasStateAuthority)
        {
            RPC_UpdateKey(player, ammount);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdateKey(PlayerRef player, int ammount)
    {
        BoardGameData.instance.UpdateKey(player, ammount);
        if (HasStateAuthority)
        {
            RPC_UpdatePlayerDataUI();
        }
    }

    public void RequestUpdateCup(PlayerRef player, int ammount)
    {
        if (HasStateAuthority)
        {
            RPC_UpdateCup(player, ammount);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdateCup(PlayerRef player, int ammount)
    {
        BoardGameData.instance.UpdateCup(player, ammount);
        if (HasStateAuthority)
        {
            RPC_UpdatePlayerDataUI();
        }
    }

    public void RequestUpdateHealth(PlayerRef player, int ammount)
    {
        if (HasStateAuthority)
        {
            RPC_UpdateHealth(player, ammount);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdateHealth(PlayerRef player, int ammount)
    {
        BoardGameData.instance.UpdateHealth(player, ammount);
        if (HasStateAuthority)
        {
            RPC_UpdatePlayerDataUI();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdatePlayerDataUI()
    {
        UpdatePlayerDataUI();
    }

    public void UpdatePlayerDataUI()
    {
        foreach (Transform child in slotContainer)
        {
            if (child == slotTemplate) continue;
            Destroy(child.gameObject);
        }

        #region UpdatePlayerBoardStatUI
        Debug.Log("playersBoardStat count: " + BoardGameData.instance.playersBoardStat.Count);

        Dictionary<PlayerRef, BoardGameStat> dictCopy = BoardGameData.instance.playersBoardStat;
        dictCopy.OrderByDescending(d => d.Value.cupQty);

        foreach (var kvp in dictCopy)
        {
            RectTransform slotRect = Instantiate(slotTemplate, slotContainer).GetComponent<RectTransform>();
            slotRect.gameObject.SetActive(true);

            BoardSlotRect boardSlotRect = slotRect.GetComponent<BoardSlotRect>();

            boardSlotRect.UpdateCup(kvp.Value.cupQty);
            boardSlotRect.UpdateKey(kvp.Value.keyQty);
            boardSlotRect.UpdateHealth(kvp.Value.health);

            string playerName = BoardGameData.instance.GetName(kvp.Key);

            if (string.IsNullOrEmpty(playerName))
                boardSlotRect.UpdateName(kvp.Key.PlayerId.ToString());
            else
                boardSlotRect.UpdateName(playerName);
        }
        #endregion
    }

    #endregion


    #region Turn

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StartFirstTurn()
    {
        if (Object.HasStateAuthority)
        {
            currentPlayerIndex = 0;
            currentPlayerRef = playerController[currentPlayerIndex].Object.InputAuthority;
            CameraFollow.instance.RPC_StartFollowTarget(playerController[currentPlayerIndex].Object.Id);
        }

        playerController[currentPlayerIndex].StartTurn();
        UpdateTurnUI();
    }

    public bool CheckWin()
    {
        BoardGameData data = BoardGameData.instance;
        foreach (var player in NetworkManager.instance.GetAllPlayers())
        {
            if (data.playersBoardStat[player].cupQty >= 1)
            {
                data.winner = player;
                return true;
            }
        }
        return false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestNextTurn()
    {
        RPC_NextTurn();
    }

    public void RequestNextTurn()
    {
        if (Object.HasStateAuthority)
            if (CheckWin())
            {
                RPC_LoadScene("Win");
                return;
            }

        if (Object.HasStateAuthority)
        {
            RPC_NextTurn();
        }
        else
        {
            RPC_RequestNextTurn();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NextTurn()
    {
        if (Object.HasStateAuthority)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % playerController.Count;
            currentPlayerRef = playerController[currentPlayerIndex].Object.InputAuthority;
            if (currentPlayerIndex == 0)
            {
                RPC_LoadScene(isFirstTry ? "MNG3" : "MNG1");
            }
            CameraFollow.instance.RPC_StartFollowTarget(playerController[currentPlayerIndex].Object.Id);
        }

        if (currentPlayerIndex != 0)
        {
            playerController[currentPlayerIndex].StartTurn();
            UpdateTurnUI();
        }
    }

    void UpdateTurnUI()
    {
        if (currentPlayerRef == Runner.LocalPlayer)
        {
            turnNotifyText.text = "Your Turn";
        }
        else
        {
            turnNotifyText.text = $"{playerController[currentPlayerIndex].name}'s Turn";
        }

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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_LoadScene(string sceneName)
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