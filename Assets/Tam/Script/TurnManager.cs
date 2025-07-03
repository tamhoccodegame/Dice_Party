using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

    [Header("Camera")]
    public Camera cam;
    public Vector3 camOffset;
    private Transform targetCam; // Vị trí camera cần đến
    private float cameraLerpSpeed = 4f; // Tốc độ Lerp (tùy chỉnh)

    private bool isCameraMoving; // Không dùng Networked nữa

    public TextMeshProUGUI turnNotifyText;

    public enum GameState
    {
        BoardGame,
        MiniGame
    }

    public GameState currentState;

    public Image blackScreen;

    public float fadeDuration = 1f;

    public void FadeIn() => StartCoroutine(FadeBlackScreen(0, 1));
    public void FadeOut() => StartCoroutine(FadeBlackScreen(1, 0));

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
        }

        if (Object.HasStateAuthority)
        {
            RPC_StartFirstTurn();

            if (isFirstTry)
            {
                StartCoroutine(DelayUpdatePlayerUI());
            }

            RPC_UpdatePlayerDataUI();
        }
    }

    IEnumerator DelayUpdatePlayerUI()
    {
        foreach (var player in NetworkManager.instance.GetAllPlayers())
        {
            RPC_UpdateKey(player, 0);
            RPC_UpdateCup(player, 0);
            RPC_UpdateHealth(player, 50);
        }
        yield return new WaitForSecondsRealtime(0.5f);

        RPC_UpdatePlayerDataUI();
    }

    #region Camera

    private void Update()
    {
        if (targetCam == null) return;
        if (!isCameraMoving)
        {
            Vector3 desiredPosition = targetCam.position + camOffset;
            if (Vector3.Distance(cam.transform.position, desiredPosition) > 0.3f)
                cam.transform.position = Vector3.Lerp(cam.transform.position, desiredPosition, Time.deltaTime * cameraLerpSpeed);
        }
    }

    public override void FixedUpdateNetwork()
    {

    }

    void StartFollowTarget()
    {
        StartCoroutine(ChangeFollowTarget());
    }

    IEnumerator ChangeFollowTarget()
    {
        RPC_SetIsCamMoving(true);
        Vector3 oldTarget = cam.transform.position;
        Vector3 newTarget = playerController[currentPlayerIndex].transform.position + camOffset;

        float elapsedTime = 0f;
        float duration = 1.5f;

        while (elapsedTime < duration)
        {
            cam.transform.position = Vector3.Lerp(oldTarget, newTarget, elapsedTime / duration);
            elapsedTime += Runner.DeltaTime;
            yield return null;
        }
        NetworkId newTargetId = playerController[currentPlayerIndex].Object.Id;
        cam.transform.position = newTarget;
        RPC_ChangeCameraPosition(newTargetId);
        RPC_SetIsCamMoving(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SetIsCamMoving(bool enabled)
    {
        isCameraMoving = enabled;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ChangeCameraPosition(NetworkId newTargetId)
    {
        targetCam = Runner.FindObject(newTargetId).transform;
    }

    #endregion


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
        Debug.Log("playersBoardStat count: " + BoardGameData.instance.playersBoardStat.Count);

        foreach (var kvp in BoardGameData.instance.playersBoardStat)
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
            StartFollowTarget();
        }

        playerController[currentPlayerIndex].StartTurn();
        UpdateTurnUI();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestNextTurn()
    {
        RPC_NextTurn();
    }

    public void RequestNextTurn()
    {
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
                RPC_LoadScene();
            }
            StartFollowTarget();
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
    void RPC_LoadScene()
    {
        StartCoroutine(LoadMNG());
    }

    IEnumerator LoadMNG()
    {
        yield return null;
        LevelLoader.instance.LoadScene("MNG3");
        //yield return StartCoroutine(FadeBlackScreen(0, 1));
        //if (isFirstTry)
        //{
        //    LevelLoader.instance.LoadScene("MNG3");

        //}
        //else
        //{
        //    LevelLoader.instance.LoadScene("MNG1");
        //}
    }
    #endregion

}