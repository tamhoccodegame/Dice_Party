using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Bắt buộc object này phải có CharacterController
[RequireComponent(typeof(CharacterController))]
public class BoardGameController : NetworkBehaviour
{
    // --- Các node hiện tại và node sắp di chuyển ---
    [Header("Move")]
    public BoardNode currentNode;         // node hiện tại đang đứng
    public int currentNodeId;             //Chỉ dùng được local scene
    [Networked, UnitySerializeField] public string currentNodeName { get; set; }
    public BoardNode toMoveNode;          // node sẽ di chuyển tới tiếp

    [Networked] public int currentStep { get; set; }      // số bước xúc xắc random, sync qua network
    private NetworkCharacterController controller;               // component điều khiển di chuyển vật lý
    private Animator animator;                            // component điều khiển animation
    [Networked] public bool waitingForChoice { get; set; } // đang chờ người chơi chọn hướng đi (sync)

    // --- Quản lý các mũi tên chọn hướng ---
    [Header("ArrowDirection")]
    public GameObject arrowDirectionPrefab;   // prefab của mũi tên chỉ hướng
    public List<GameObject> spawnedArrows = new List<GameObject>(); // danh sách các mũi tên đã spawn ra

    [Header("Effect")]
    public ParticleSystem rollDiceEffect;

    // --- Kiểm tra có phải lượt của mình hay không ---
    bool isMyTurn => TurnManager.instance.currentPlayerRef == Runner.LocalPlayer;

    // --- Quản lý xúc xắc và UI hiển thị bước ---
    [Header("Dice and Step")]
    public GameObject dice;         // xúc xắc đang spawn trên scene
    public TextMeshPro stepText;     // text hiện số bước trên UI

    // --- State Machine cho việc di chuyển ---
    private enum State { Idle, Rolling, WaitingForAnim, Moving, UsingItem }
    [Networked] private State currentState { get; set; }  // state hiện tại, sync toàn bộ clients

    private State cachedMoveState;   // lưu state cũ để kiểm tra thay đổi (chỉ dùng cho animation)
    private float animTimer = 0f;        // thời gian chờ khi chơi animation roll dice

    public Transform feet;

    // --- Hàm Spawned() chạy khi object này spawn ---
    public override void Spawned()
    {
        controller = GetComponent<NetworkCharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();

        //inventory = UIInventory.inventory;
        //inventory.onItemUsed += OnItemUsed;

        // Lấy dữ liệu node từ BoardGameData (nếu có)
        string currentNodeName1 = null;
        BoardGameData gameData = BoardGameData.instance;

        if (gameData != null && gameData.playerCurrentNode.Count > 0)
        {
            currentNodeName1 = gameData.GetNode(Object.InputAuthority);
        }

        // Nếu không có dữ liệu từ BoardGameData thì lấy node mặc định
        if (currentNodeName1 != null)
        {
            currentNode = GameObject.Find(currentNodeName1).GetComponent<BoardNode>();
        }
        else
        {
            currentNode = GameObject.Find("AddDice").GetComponent<BoardNode>();
        }

        Debug.Log(currentNode.name);

        // Set node tiếp theo mặc định là node đầu tiên
        if (HasStateAuthority)
            RPC_SetCurrentNode(currentNode.Object.Id);

        toMoveNode = currentNode.nextNodes[0];
        stepText.gameObject.SetActive(false);

        // Khởi tạo cached state để sync animation
        cachedMoveState = currentState;
        UpdateAnimation();  // cập nhật animation đúng state
    }

    // --- Hàm Update() chỉ chạy trên client local ---
    void Update()
    {
        // Đảm bảo cả host và client đều update animation nếu state thay đổi
        if (cachedMoveState != currentState)
        {
            cachedMoveState = currentState;
            UpdateAnimation();
        }

        if (HasInputAuthority && isMyTurn)
        {
            if (currentState != State.Idle) return;

            // Nếu là lượt mình và đang idle thì bấm space để roll dice
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RPC_RequestDiceRoll();
                RPC_HideDice();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                RPC_HideDice();
            }
        }
    }

    // --- FixedUpdateNetwork() chạy trên State Authority (host) ---
    public override void FixedUpdateNetwork()
    {
        // Kiểm tra nếu moveState thay đổi thì update animation
        if (cachedMoveState != currentState)
        {
            cachedMoveState = currentState;
            UpdateAnimation();
        }

        if (!HasStateAuthority)
            return; // chỉ host xử lý logic game

        // --- Logic di chuyển nhân vật ---
        if (currentState == State.Moving && !waitingForChoice)
        {
            Vector3 direction = (toMoveNode.transform.position - feet.position).normalized;
            direction.y = 0;

            controller.Move(direction);

            // Đã tới node kế tiếp
            if (Vector3.Distance(feet.position, toMoveNode.transform.position) <= 0.5f)
            {
                currentNode = toMoveNode;
                if (HasStateAuthority)
                    RPC_SetCurrentNode(currentNode.Object.Id);
                currentStep--;

                if (currentStep > 0)
                {
                    // Nếu có ngã ba thì chờ người chơi chọn hướng
                    if (currentNode.nextNodes.Count > 1)
                    {
                        waitingForChoice = true;
                        RPC_ShowDirectionChoices();
                        SetMoveState(State.Idle);
                        return;
                    }
                    else
                    {
                        toMoveNode = currentNode.nextNodes[0];
                    }
                }
                else
                {
                    SetMoveState(State.Idle);
                    TriggerNodeEvent();
                }
            }
        }

        // --- Logic chờ anim roll dice kết thúc ---
        if (currentState == State.WaitingForAnim)
        {
            animTimer -= Runner.DeltaTime;
            if (animTimer <= 0f)
            {
                if (currentNode.nextNodes.Count > 1)
                {
                    waitingForChoice = true;
                    RPC_ShowDirectionChoices();
                    SetMoveState(State.Idle);
                    return;
                }
                else
                {
                    SetMoveState(State.Moving);
                }
            }

        }
    }


    // --- Đổi state ---
    private void SetMoveState(State newState)
    {
        currentState = newState;
    }

    // --- Cập nhật animation theo state ---
    private void UpdateAnimation()
    {
        switch (currentState)
        {
            case State.Idle:
                animator.CrossFade("Idle", 0.25f);
                break;
            case State.Moving:
                animator.CrossFade("Run", 0.25f);
                break;
            case State.WaitingForAnim:
                animator.CrossFade("RollDice", 0.25f);
                break;
        }
    }

    // --- Hàm gọi khi bắt đầu lượt ---
    public void StartTurn()
    {
        ShowDice();
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
            string currentNodeName = player.currentNodeName;
            boardGameData.UpdateNode(playerRef, currentNodeName);
        }

        TurnManager.instance.RequestNextTurn();
    }


    // --- Spawn các mũi tên chọn hướng khi tới ngã ba ---
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ShowDirectionChoices()
    {
        if (!isMyTurn) return;

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
        if (!isMyTurn) return;
        ClearArrow();
        RPC_ChooseDirection(index);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_ChooseDirection(int index)
    {
        toMoveNode = currentNode.nextNodes[index];
        waitingForChoice = false;
        SetMoveState(State.Moving);
    }

    // --- Gọi sự kiện khi đứng trên node ---
    void TriggerNodeEvent()
    {
        Debug.Log("Gọi sự kiện của node: " + currentNode.name);
        if (HasStateAuthority)
            currentNode.RPC_ProcessNode(Runner.LocalPlayer);
    }

    void OnItemUsed(NetworkId itemPrefab)
    {

    }

    private void ShowDice()
    {
        dice.SetActive(true);
    }

    #region RPC
    // --- RPC: Client gửi yêu cầu lắc xúc xắc ---
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestDiceRoll()
    {
        if (currentState != State.Idle) return;

        currentStep = Random.Range(1, 5);   // random số bước
        SetMoveState(State.WaitingForAnim);
        animTimer = 1f;                     // đợi 1 giây chơi animation roll
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_HideDice()
    {
        StartCoroutine(HideDiceCoroutine());
    }

    IEnumerator HideDiceCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        stepText.gameObject.SetActive(true);
        rollDiceEffect.Play();
        dice.SetActive(false);
        stepText.text = currentStep.ToString();
        yield return new WaitForSeconds(0.5f);
        stepText.gameObject.SetActive(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SetCurrentNode(NetworkId nodeId)
    {
        currentNode = Runner.FindObject(nodeId).GetComponent<BoardNode>();
        currentNodeName = currentNode.name;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestUseItem()
    {
        if (currentState != State.Idle) return;

        currentStep = Random.Range(1, 5);   // random số bước
        SetMoveState(State.WaitingForAnim);
        animTimer = 1f;                     // đợi 1 giây chơi animation roll
        rollDiceEffect.Play();
    }

    #endregion

}