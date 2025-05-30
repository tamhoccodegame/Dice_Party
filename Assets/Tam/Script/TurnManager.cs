using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager instance;
    public List<BoardGameController> playerController;
    [Networked] public int currentPlayerIndex { get; set; }
    [Networked] public PlayerRef currentPlayerRef { get; set; }

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

    private void Awake()
    {
        instance = this;
    }

    public override void Spawned()
    {
        cam = Camera.main;

        playerController = FindObjectsByType<BoardGameController>(FindObjectsSortMode.InstanceID).ToList();

        if (Object.HasStateAuthority)
        {
            RPC_StartFirstTurn();
        }
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
        if (!isCameraMoving && playerController.Count > 0)
        {
            Vector3 newCamPosition = playerController[currentPlayerIndex].transform.position + camOffset;

            // Chỉ gửi RPC nếu khoảng cách giữa vị trí cũ và mới lớn hơn 0.2
            if (Vector3.Distance(newCamPosition, targetCamPosition) > 0.2f)
            {
                RPC_UpdateCameraPosition(newCamPosition);
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdateCameraPosition(Vector3 newPosition)
    {
        targetCamPosition = newPosition;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StartFirstTurn()
    {
        if (Object.HasStateAuthority)
        {
            currentPlayerIndex = 0;
            currentPlayerRef = playerController[currentPlayerIndex].Object.InputAuthority;
            playerController[currentPlayerIndex].StartTurn();
        }
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
                Runner.LoadScene("MNG3");
        }

        playerController[currentPlayerIndex].StartTurn();
        UpdateTurnUI();
        StartFollowTarget();
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

    void StartFollowTarget()
    {
        RPC_RequestFollowTarget();
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
}
