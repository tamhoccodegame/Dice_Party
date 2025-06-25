using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct PlayerBoardData : INetworkStruct
{
    public int health;
    public int key;
    public int cup;
}

public class TurnManager : NetworkBehaviour
{
    public static TurnManager instance;
    public List<BoardGameController> playerController;
    [Networked] public int currentPlayerIndex { get; set; }
    [Networked] public PlayerRef currentPlayerRef { get; set; }

    [Header("BXH")]
    public Transform slotTemplate;
    public Transform slotContainer;
    [Networked, Capacity(4)] public NetworkDictionary<PlayerRef, PlayerBoardData> playersData => default;
    

    [Header("Camera")]
    public Camera cam;
    public Vector3 camOffset;
    private Vector3 targetCamPosition; // Vị trí camera cần đến
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
        cam = Camera.main;

        playerController = FindObjectsByType<BoardGameController>(FindObjectsSortMode.InstanceID).ToList();

        if (Object.HasStateAuthority)
        {
            RPC_StartFirstTurn();

            foreach(PlayerRef player in NetworkManager.instance.GetAllPlayers())
            {
                playersData.Add(player, new PlayerBoardData { key = 0, cup = 0, health = 50 });
            }
        }

        UpdatePlayerDataUI();
    }

    // Update camera bằng nội suy để di chuyển mượt
    private void Update()
    {
        if (!isCameraMoving)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetCamPosition, Time.deltaTime * cameraLerpSpeed);
        }
    }

    // Chỉ Host mới cập nhật vị trí mới và gửi cho Client
    public override void FixedUpdateNetwork()
    {
        if (!isCameraMoving && playerController.Count > 0 && playerController[currentPlayerIndex] != null)
        {
            Vector3 newCamPosition = playerController[currentPlayerIndex].transform.position + camOffset;

            // Chỉ gửi RPC nếu khoảng cách giữa vị trí cũ và mới lớn hơn 0.2
            if (Vector3.Distance(newCamPosition, targetCamPosition) > 0.2f)
            {
                RPC_UpdateCameraPosition(newCamPosition);
            }
        }
    }

    public void RequestUpdateKey(PlayerRef player, int ammount)
    {
        if (HasStateAuthority)
        {
            var data = playersData[player];
            data.key += ammount;
            playersData.Set(player, data);
            RPC_UpdatePlayerDataUI();
        }
    }

    public void RequestUpdateCup(PlayerRef player, int ammount)
    {
        if (HasStateAuthority)
        {
            var data = playersData[player];
            data.cup += ammount;
            playersData.Set(player, data);
            RPC_UpdatePlayerDataUI();
        }
    }

    public void RequestUpdateHealth(PlayerRef player, int ammount)
    {
        if (HasStateAuthority)
        {
            var data = playersData[player];
            data.health += ammount;
            playersData.Set(player, data);
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
        foreach(Transform child in slotContainer)
        {
            if (child == slotTemplate) continue;
            Destroy(child.gameObject);
        }

        var playerList = playersData.OrderByDescending(p => p.Value.cup).ThenByDescending(p => p.Value.key).ToList();

        foreach(var player in playerList)
        {
            RectTransform slotRect = Instantiate(slotTemplate, slotContainer).GetComponent<RectTransform>();
            slotRect.gameObject.SetActive(true);    

            BoardSlotRect boardSlotRect = slotRect.GetComponent<BoardSlotRect>();   

            boardSlotRect.UpdateCup(player.Value.cup);
            boardSlotRect.UpdateKey(player.Value.key);
            boardSlotRect.UpdateHealth(player.Value.health);
            boardSlotRect.UpdateName(player.Key.PlayerId.ToString());
        }
    }


    public PlayerBoardData GetPlayerData(PlayerRef player)
    {
        return playersData[player];
    }

    #region Turn

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StartFirstTurn()
    {
        if (Object.HasStateAuthority)
        {
            currentPlayerIndex = 0;
            currentPlayerRef = playerController[currentPlayerIndex].Object.InputAuthority;
        }

        playerController[currentPlayerIndex].StartTurn();
        UpdateTurnUI();
        StartFollowTarget();
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
        }

        if (currentPlayerIndex != 0)
        {
            playerController[currentPlayerIndex].StartTurn();
            UpdateTurnUI();
            StartFollowTarget();
        }
    }

    void UpdateTurnUI()
    {
        if(currentPlayerRef == Runner.LocalPlayer)
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
        yield return StartCoroutine(FadeBlackScreen(0, 1));
        LevelLoader.instance.LoadScene("MNG1");
    }

    #region Camera
    void StartFollowTarget()
    {
        RPC_RequestFollowTarget();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdateCameraPosition(Vector3 newPosition)
    {
        targetCamPosition = newPosition;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)] // Chạy trên tất cả client
    void RPC_RequestFollowTarget()
    {
        if(!Object.HasStateAuthority) RPC_FollowTarget();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_FollowTarget()
    {
        Debug.Log("RPC_StartFollowTarget gọi trên client: " + Runner.LocalPlayer);
        if (!isCameraMoving) StartCoroutine(ChangeFollowTarget());
    }

    IEnumerator ChangeFollowTarget()
    {
        isCameraMoving = true;
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

        cam.transform.position = newTarget;
        isCameraMoving = false;
    }
    #endregion
}
