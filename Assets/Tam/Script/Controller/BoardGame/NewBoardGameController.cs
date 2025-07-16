using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(NetworkCharacterController))]
public class NewBoardGameController : NetworkBehaviour
{
    private Animator animator;
    private NetworkCharacterController _controller;

    private BoardState currentState;
    [Networked] public string currentAnim { get; set; }

    [Networked] public Vector3 NetworkPosition { get; set; }
    private Vector3 _smoothPos;

    public IdleState idleState;
    public MovingState movingState;
    public ChooseDirectionState chooseDirectionState;
    public ItemState itemState;
    public NodeState nodeState;

    public enum NetworkState
    {
        Idle,
        Moving,
        ChooseDirection,
        Item,
        Node,
    }

    [Networked, UnitySerializeField] public NetworkState networkState { get; set; }

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

        _smoothPos = transform.position;

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

        if(HasStateAuthority)
        RPC_ChangeNetworkState(NetworkState.Idle);
    }

    public void RequestChangeState(NetworkState newState)
    {
        if (newState == networkState) return;

        if (HasStateAuthority)
        {
            networkState = newState;
            RPC_ChangeNetworkState(newState);
        }
        else
        {
            RPC_RequestChangeNetworkState(newState);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestChangeNetworkState(NetworkState newState)
    {
        networkState = newState;
        RPC_ChangeNetworkState(networkState);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ChangeNetworkState(NetworkState newState)
    {
        currentState?.Exit();
        switch (newState)
        {
            case NetworkState.Idle:
                currentState = idleState;
                break;
            case NetworkState.Moving:
                currentState = movingState;
                break;
            case NetworkState.ChooseDirection:
                currentState = chooseDirectionState;
                break;
            case NetworkState.Item:
                currentState = itemState;
                break;
            case NetworkState.Node:
                currentState = nodeState;
                break;
        }

        currentState.Enter();
    }

    public void RequestChangeAnimation(string animName)
    {
        if (animName == currentAnim) return;

        if (HasStateAuthority)
        {
            RPC_ChangeAnimation(animName);
        }
        else if(HasInputAuthority)
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
        currentAnim = animName;
        animator.Play(animName);
    }

    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            currentState?.Update();
        }
    }

    public void RequestHurt(PlayerRef player, int ammount)
    {
        if (HasStateAuthority)
            RPC_Hurt(player, ammount);
        else
            RPC_RequestHurt(player, ammount);
    }

    public void RequestHealth(PlayerRef player, int ammount)
    {
        if (HasStateAuthority)
            RPC_Health(player, ammount);
        else
            RPC_RequestHealth(player, ammount);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestHurt(PlayerRef player, int ammount)
    {
        RPC_Hurt(player, ammount);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestHealth(PlayerRef player, int ammount)
    {
        RPC_Health(player, ammount);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Hurt(PlayerRef player, int ammount)
    {
        StartCoroutine(HurtCoroutine(player, ammount));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Health(PlayerRef player, int ammount)
    {
        StartCoroutine(HealthCoroutine(player, ammount));
    }

    private IEnumerator HurtCoroutine(PlayerRef player, int ammount)
    {
        string previousAnim = currentAnim;
        Debug.Log("💢 Bị đau!");
        RequestChangeAnimation("Hurt");
        TurnManager.instance.RequestUpdateHealth(player, -ammount);
        // hiệu ứng bị thương
        yield return new WaitForSecondsRealtime(0.5f);

        RequestChangeAnimation(previousAnim);
    }

    private IEnumerator HealthCoroutine(PlayerRef player, int ammount)
    {
        string previousAnim = currentAnim;
        Debug.Log("❤️ Hồi máu!");
        RequestChangeAnimation("Heal");
        TurnManager.instance.RequestUpdateHealth(player, ammount);
        // hiệu ứng hồi máu
        yield return new WaitForSecondsRealtime(1f);

        RequestChangeAnimation(previousAnim);
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            NetworkPosition = transform.position;
        }
        else
        {
            _smoothPos = Vector3.Lerp(_smoothPos, NetworkPosition, 0.15f);
            transform.position = _smoothPos;
        }

        if (!HasStateAuthority) return;
        currentState?.FixedUpdateNetwork();
    }

    #region Move
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
                RPC_ChangeNetworkState(NetworkState.ChooseDirection);
                yield break;
            }
            StepsLeft = 5;
        }

        RPC_ChangeAnimation("RollDice");
        yield return new WaitForSecondsRealtime(1f);

        RequestChangeState(NetworkState.Moving);
    }

    public void RequestSetStepLeft(int step)
    {
        if (HasStateAuthority)
        {
            RPC_SetStepLeft(step);
        }
        else
        {
            RPC_RequestSetStepLeft(step);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestSetStepLeft(int step)
    {
        RPC_SetStepLeft(step);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetStepLeft(int step)
    {
        StepsLeft = step;
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
                    RequestChangeState(NetworkState.ChooseDirection);
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

    #endregion

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
        RequestChangeState(NetworkState.Moving);
    }

    private void ShowDice()
    {
        dice.SetActive(true);
    }

    #region RPC
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
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

    public void RequestSetCurrentNode(NetworkId nodeId)
    {
        if (HasStateAuthority)
        {
            RPC_SetCurrentNode(nodeId);
        }
        else
        {
            RPC_RequestSetCurrentNode(nodeId);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestSetCurrentNode(NetworkId nodeId)
    {
        RPC_SetCurrentNode(nodeId);
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
        if (HasStateAuthority)
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

    public void RequestSetItemPosition(int itemId)
    {
        if (HasStateAuthority)
        {
            RPC_SetItemPosition(itemId);
        }
        else
        {
            RPC_RequestSetItemPosition(itemId);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestSetItemPosition(int itemId)
    {
        RPC_SetItemPosition(itemId);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetItemPosition(int itemId)
    {
        Debug.Log("Alo em");
        var itemTransform = ItemDatabase.instance.GetItemByItemId(itemId).transform;

        itemTransform.SetParent(gunSpawnPoint);
        itemTransform.transform.localPosition = Vector3.zero;
        itemTransform.localRotation = Quaternion.identity;
    }

    public void RequestTriggerItem()
    {
        if(HasStateAuthority)
        {
            RPC_TriggerItem();
        }
        else
        {
            RPC_RequestTriggerItem();
        }
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
