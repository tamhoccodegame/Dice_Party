using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Bắt buộc object này phải có CharacterController
[RequireComponent(typeof(CharacterController))]
public class BoardGameController : NetworkBehaviour
{
    // --- Các thông số cấu hình di chuyển ---
    [Header("Stat")]
    public float moveSpeed;               // tốc độ di chuyển
    public float rotationSpeed;           // tốc độ quay mặt nhân vật

    // --- Các node hiện tại và node sắp di chuyển ---
    [Header("Move")]
    public BoardNode currentNode;         // node hiện tại đang đứng
    public BoardNode toMoveNode;          // node sẽ di chuyển tới tiếp

    [Networked] public int currentStep { get; set; }      // số bước xúc xắc random, sync qua network
    private CharacterController controller;               // component điều khiển di chuyển vật lý
    private Animator animator;                            // component điều khiển animation
    [Networked] public bool waitingForChoice { get; set; } // đang chờ người chơi chọn hướng đi (sync)

    // --- Quản lý các mũi tên chọn hướng ---
    [Header("ArrowDirection")]
    public GameObject arrowDirectionPrefab;   // prefab của mũi tên chỉ hướng
    public List<GameObject> spawnedArrows = new List<GameObject>(); // danh sách các mũi tên đã spawn ra

    // --- Kiểm tra có phải lượt của mình hay không ---
    bool isMyTurn => TurnManager.instance.currentPlayerRef == Runner.LocalPlayer;

    // --- Quản lý xúc xắc và UI hiển thị bước ---
    [Header("Dice and Step")]
    public GameObject dicePrefab;    // prefab xúc xắc
    private Dice activeDice;         // xúc xắc đang spawn trên scene
    public TextMeshPro stepText;     // text hiện số bước trên UI

    // --- State Machine cho việc di chuyển ---
    private enum MoveState { Idle, Rolling, WaitingForAnim, Moving }
    [Networked] private MoveState moveState { get; set; }  // state hiện tại, sync toàn bộ clients

    private MoveState cachedMoveState;   // lưu state cũ để kiểm tra thay đổi (chỉ dùng cho animation)
    private float animTimer = 0f;        // thời gian chờ khi chơi animation roll dice

    // --- Hàm Spawned() chạy khi object này spawn ---
    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();

        // Lấy dữ liệu node từ BoardGameData (nếu có)
        string currentNodeName = null;
        BoardGameData gameData = BoardGameData.instance;

        if (gameData != null && gameData.playerCurrentNode.Count > 0)
        {
            currentNodeName = gameData.GetNode(Runner.LocalPlayer);
        }

        // Nếu không có dữ liệu từ BoardGameData thì lấy node mặc định
        if (currentNodeName != null)
        {
            currentNode = GameObject.Find(currentNodeName).GetComponent<BoardNode>();
        }
        else
        {
            currentNode = GameObject.Find("Dice (7)").GetComponent<BoardNode>();
        }

        // Set node tiếp theo mặc định là node đầu tiên
        toMoveNode = currentNode.nextNodes[0];
        stepText.gameObject.SetActive(false);

        // Khởi tạo cached state để sync animation
        cachedMoveState = moveState;
        UpdateAnimation();  // cập nhật animation đúng state
    }

    // --- Hàm Update() chỉ chạy trên client local ---
    void Update()
    {
        if (HasInputAuthority && isMyTurn)
        {
            // Nếu là lượt mình và đang idle thì bấm space để roll dice
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (moveState == MoveState.Idle)
                {
                    RPC_RequestDiceRoll();
                }
            }
        }

        // Cập nhật số bước hiển thị trên UI
        stepText.text = currentStep.ToString();
        stepText.gameObject.SetActive(currentStep > 0);
    }

    // --- FixedUpdateNetwork() chạy trên State Authority (host) ---
    public override void FixedUpdateNetwork()
    {
        // Kiểm tra nếu moveState thay đổi thì update animation
        if (cachedMoveState != moveState)
        {
            cachedMoveState = moveState;
            UpdateAnimation();
        }

        if (!HasStateAuthority)
            return; // chỉ host xử lý logic game

        // --- Logic di chuyển nhân vật ---
        if (moveState == MoveState.Moving && !waitingForChoice)
        {
            Vector3 direction = (toMoveNode.transform.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
            }

            Vector3 moveDir = direction * moveSpeed * Runner.DeltaTime;
            controller.Move(moveDir);

            // Đã tới node kế tiếp
            if (Vector3.Distance(transform.position, toMoveNode.transform.position) <= 0.5f)
            {
                currentNode = toMoveNode;
                currentStep--;

                if (currentStep > 0)
                {
                    // Nếu có ngã ba thì chờ người chơi chọn hướng
                    if (currentNode.nextNodes.Count > 1)
                    {
                        waitingForChoice = true;
                        ShowDirectionChoices();
                        SetMoveState(MoveState.Idle);
                        return;
                    }
                    else
                    {
                        toMoveNode = currentNode.nextNodes[0];
                    }
                }
                else
                {
                    SetMoveState(MoveState.Idle);
                    TriggerNodeEvent();
                }
            }
        }

        // --- Logic chờ anim roll dice kết thúc ---
        if (moveState == MoveState.WaitingForAnim)
        {
            animTimer -= Runner.DeltaTime;
            if (animTimer <= 0f)
            {
                SetMoveState(MoveState.Moving);
            }
        }
    }

    // --- Đổi state ---
    private void SetMoveState(MoveState newState)
    {
        moveState = newState;
    }

    // --- Cập nhật animation theo state ---
    private void UpdateAnimation()
    {
        switch (moveState)
        {
            case MoveState.Idle:
                animator.CrossFade("Idle", 0.25f);
                break;
            case MoveState.Moving:
                animator.CrossFade("Run", 0.25f);
                break;
            case MoveState.WaitingForAnim:
                animator.CrossFade("RollDice", 0.25f);
                break;
        }
    }

    // --- Hàm gọi khi bắt đầu lượt ---
    public void StartTurn()
    {
        if (HasInputAuthority)
        {
            RPC_RequestShowDice();
        }
    }

    // --- Hàm kết thúc lượt ---
    public void EndTurn()
    {
        BoardGameData boardGameData = BoardGameData.instance;
        BoardGameController[] players = FindObjectsByType<BoardGameController>(FindObjectsSortMode.None);

        // Lưu trạng thái node hiện tại vào data để đồng bộ map
        foreach (var player in players)
        {
            PlayerRef playerRef = player.GetComponent<NetworkObject>().InputAuthority;
            string currentNodeName = player.currentNode.name;
            boardGameData.UpdateNode(playerRef, currentNodeName);
        }

        TurnManager.instance.RequestNextTurn();
    }

    // --- RPC: Client gửi yêu cầu lắc xúc xắc ---
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestDiceRoll()
    {
        if (moveState != MoveState.Idle) return;

        currentStep = Random.Range(1, 5);   // random số bước
        SetMoveState(MoveState.WaitingForAnim);
        animTimer = 1f;                     // đợi 1 giây chơi animation roll
    }

    // --- RPC: Client yêu cầu host spawn dice ---
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestShowDice()
    {
        if (activeDice != null)
            return;

        Vector3 spawnPos = transform.position + new Vector3(0, 5f, 0);
        activeDice = Runner.Spawn(dicePrefab, spawnPos, Quaternion.identity).GetComponent<Dice>();
    }

    // --- Spawn các mũi tên chọn hướng khi tới ngã ba ---
    void ShowDirectionChoices()
    {
        ClearArrow();
        for (int i = 0; i < currentNode.nextNodes.Count; i++)
        {
            BoardNode next = currentNode.nextNodes[i];
            Vector3 midPoint = (currentNode.transform.position + next.transform.position) / 2;
            midPoint.y = arrowDirectionPrefab.transform.position.y;

            ArrowPointer arrow = Instantiate(arrowDirectionPrefab, midPoint, Quaternion.identity).GetComponent<ArrowPointer>();
            arrow.transform.rotation = Quaternion.LookRotation((next.transform.position - currentNode.transform.position), Vector3.up);
            arrow.Setup(this, i);
            spawnedArrows.Add(arrow.gameObject);
        }
    }

    // --- Clear các mũi tên chọn hướng cũ ---
    void ClearArrow()
    {
        foreach (var go in spawnedArrows)
        {
            Destroy(go);
        }
        spawnedArrows.Clear();
    }

    // --- Hàm khi client chọn hướng ---
    public void ChooseDirection(int index)
    {
        ClearArrow();
        toMoveNode = currentNode.nextNodes[index];
        waitingForChoice = false;
        SetMoveState(MoveState.Moving);
    }

    // --- Gọi sự kiện khi đứng trên node ---
    void TriggerNodeEvent()
    {
        Debug.Log("Gọi sự kiện của node: " + currentNode.name);
        if (HasStateAuthority)
            currentNode.RPC_ProcessNode(Runner.LocalPlayer);
    }
}
