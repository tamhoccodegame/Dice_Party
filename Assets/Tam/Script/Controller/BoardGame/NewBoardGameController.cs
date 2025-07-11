using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(NetworkCharacterController))]
public class NewBoardGameController : NetworkBehaviour
{
    private Animator animator;
    private NetworkCharacterController _controller;

    private BoardState currentState;

    public IdleState idleState;
    public MovingState movingState;
    public ChooseDirectionState chooseDirectionState;
    public ItemState itemState;
    public NodeState nodeState;

    [Networked] public int StepsLeft { get; set; }
    public bool isMyTurn => Runner.LocalPlayer == TurnManager.instance.currentPlayerRef;

    public Transform feet;
    public BoardNode currentNode;
    public int currentNodeId;             //Chỉ dùng được local scene
    [Networked, UnitySerializeField] public string currentNodeName { get; set; }
    public BoardNode toMoveNode;

    // --- Quản lý xúc xắc và UI hiển thị bước ---
    [Header("Dice and Step")]
    public GameObject dice;         // xúc xắc đang spawn trên scene
    public GameObject stepTextPrefab;     // text hiện số bước trên UI

    // --- Quản lý các mũi tên chọn hướng ---
    [Header("ArrowDirection")]
    public GameObject arrowDirectionPrefab;   // prefab của mũi tên chỉ hướng
    public List<GameObject> spawnedArrows = new List<GameObject>(); // danh sách các mũi tên đã spawn ra

    [Header("Effect")]
    public ParticleSystem rollDiceEffect;

    public BoardItem currentItem;
    public Transform gunSpawnPoint;

    public override void Spawned()
    {
        animator = GetComponent<Animator>();
        _controller = GetComponent<NetworkCharacterController>();

        idleState = new IdleState(this);
        movingState = new MovingState(this);
        chooseDirectionState = new ChooseDirectionState(this);
        itemState = new ItemState(this);
        nodeState = new NodeState(this);

        string currentNodeName1 = null;
        BoardGameData gameData = BoardGameData.instance;

        if (gameData != null && gameData.playersCurrentNode.Count > 0)
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
            currentNode = FindFirstObjectByType<PlayerSpawner>().spawnPosition[0].GetComponent<BoardNode>();
        }

        toMoveNode = currentNode.nextNodes[0];

        ChangeState(idleState);
    }

    public void ChangeState(BoardState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();

        if (Object.HasStateAuthority)
        RPC_ChangeAnimation(newState.ToString());
        else
        RPC_RequestChangeAnimation(currentState.ToString());
    }

    public void RequestChangeAnimation(string animName)
    {
        if (Object.HasStateAuthority)
        {
            RPC_ChangeAnimation(animName);
        }
        else
        {
            RPC_RequestChangeAnimation(animName);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestChangeAnimation(string animName)
    {
        RPC_ChangeAnimation(animName);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ChangeAnimation(string animName)
    {
        animator.Play(animName);
    }

    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            Debug.Log(currentState);
            currentState?.Update();
        }
    }

    public override void FixedUpdateNetwork()
    {
        currentState?.FixedUpdateNetwork();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestRollDice()
    {
        RPC_RollDice();
        RPC_HideDice();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RollDice()
    {
        StartCoroutine(RollDiceCoroutine());
    }

    IEnumerator RollDiceCoroutine()
    {
        if(HasStateAuthority)
        {
            if(currentNode.nextNodes.Count > 1)
            {
                ChangeState(chooseDirectionState);
                yield break;
            }
            StepsLeft = 3;
        }

        RPC_ChangeAnimation("RollDice");
        yield return new WaitForSecondsRealtime(1f);
        ChangeState(movingState);
    }

    // Di chuyển từng bước
    public bool MoveStep()
    {
        if (StepsLeft <= 0 || toMoveNode == null) return false;

        Vector3 dir = (toMoveNode.transform.position - feet.position).normalized;
        dir.y = 0;

        _controller.Move(dir); // tốc độ di chuyển

        if (Vector3.Distance(feet.position, toMoveNode.transform.position) < 0.3f)
        {
            currentNode = toMoveNode;
            RPC_SetCurrentNode(currentNode.Object.Id);
            StepsLeft--;

            if (StepsLeft > 0)
            {
                if(currentNode.nextNodes.Count > 1)
                {
                    ChangeState(chooseDirectionState);
                    return true;
                }
                else
                {
                    toMoveNode = currentNode.nextNodes[0]; 
                }
            }
        }
        return true;
    }

    // Item (tạm bỏ qua inventory)
    public void UseSelectedItem()
    {
        Debug.Log("💥 Sử dụng item!");
        // Sau này xử lý skill / bắn súng / trap
    }

    public void StartTurn()
    {
        ShowDice();
    }

    // --- Hàm kết thúc lượt ---
    public void EndTurn()
    {
        BoardGameData boardGameData = BoardGameData.instance;
        NewBoardGameController[] players = FindObjectsByType<NewBoardGameController>(FindObjectsSortMode.None);

        // Lưu trạng thái node hiện tại vào data để đồng bộ map
        foreach (var player in players)
        {
            PlayerRef playerRef = player.GetComponent<NetworkObject>().InputAuthority;
            string currentNodeName = player.currentNodeName;
            boardGameData.UpdateNode(playerRef, currentNodeName);
        }

        if (HasInputAuthority)
            TurnManager.instance.RequestNextTurn();
    }


    // --- Spawn các mũi tên chọn hướng khi tới ngã ba ---
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ShowDirectionChoices()
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
        ChangeState(movingState);
    }

    private void ShowDice()
    {
        dice.SetActive(true);
    }

    #region RPC
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_HideDice()
    {
        StartCoroutine(HideDiceCoroutine());
    }

    IEnumerator HideDiceCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        var step = Instantiate(stepTextPrefab, dice.transform.position - new Vector3(0, 1.5f, 0), Quaternion.identity)
                   .GetComponent<StepText>();
        step.Init(StepsLeft.ToString());
        rollDiceEffect.Play();
        dice.SetActive(false);
        yield return new WaitForSeconds(0.5f);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetCurrentNode(NetworkId nodeId)
    {
        currentNode = Runner.FindObject(nodeId).GetComponent<BoardNode>();
        currentNodeName = currentNode.name;
    }
    #endregion

    #region ItemRef

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestUsingItem(int itemId)
    {
        RPC_SetUsingItem(itemId);
    }

    public void RequestSetUsingItem(int itemId)
    {
        if (Object.HasStateAuthority)
        {
            RPC_SetUsingItem(itemId);
        }
        else
        {
            RPC_RequestUsingItem(itemId);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetUsingItem(int itemId)
    {
        BoardItem item = ItemDatabase.instance.GetItemByItemId(itemId);

        //if (item != null)
        {
            currentItem = item;
            currentItem.Use(this);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetItemPosition(int itemId)
    {
        var itemTransform = ItemDatabase.instance.GetItemByItemId(itemId).transform;

        itemTransform.SetParent(gunSpawnPoint);
        itemTransform.transform.localPosition = Vector3.zero;
        itemTransform.localRotation = Quaternion.identity;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestTriggerItem()
    {
        RPC_TriggerItem();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]

    public void RPC_TriggerItem()
    {
        StartCoroutine(currentItem.ProcessCoroutine(this));
    }

    #endregion
}
