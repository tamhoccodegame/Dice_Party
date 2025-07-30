using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class NewBoardGameController : PlayerController
{
    private Animator animator;
    private CharacterController _controller;

    private BoardState currentState;

    public PlayerInput playerInput;
    public string currentAnim { get; set; }

    public IdleState idleState;
    public MovingState movingState;
    public ChooseDirectionState chooseDirectionState;
    public ItemState itemState;
    public NodeState nodeState;

    float verticalVelocity;
    Vector3 moveDir;

    public enum NetworkState
    {
        Idle,
        Moving,
        ChooseDirection,
        Item,
        Node,
    }

    public int StepsLeft { get; set; }

    public Transform feet;
    public BoardNode currentNode;
    public int currentNodeId;             //Chỉ dùng được local scene
    public string currentNodeName { get; set; }
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

    private Rigidbody[] rigidbodies;

    public void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();  
        animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        idleState = new IdleState(this);
        movingState = new MovingState(this);
        chooseDirectionState = new ChooseDirectionState(this);
        itemState = new ItemState(this);
        nodeState = new NodeState(this);

        ChangeState(idleState);
    }

    private void Start()
    {
        string savedNode = WizardPartyData.instance.playersNode[playerInput];
        if (savedNode != null)
        {
            BoardNode node = GameObject.Find(savedNode).GetComponent<BoardNode>();
            _controller.enabled = false;
            transform.position = node.transform.position;
            currentNode = node;
            _controller.enabled = true;
        }
        else
        {
            currentNode = PlayerSpawner.instance.spawnPosition[0].GetComponent<BoardNode>();
        }

        toMoveNode = currentNode.nextNodes[0];
    }

    public override PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    public override void SetInput(PlayerInput input)
    {
        playerInput = input;
    }

    public void ChangeState(BoardState newState)
    {
        currentState?.Exit();
        switch (newState)
        {
            case IdleState:
                currentState = idleState;
                break;
            case MovingState:
                currentState = movingState;
                break;
            case ChooseDirectionState:
                currentState = chooseDirectionState;
                break;
            case ItemState:
                currentState = itemState;
                break;
            case NodeState:
                currentState = nodeState;
                break;
        }

        currentState.Enter();
    }

    public void ChangeAnimation(string animName)
    {
        currentAnim = animName;
        animator.Play(animName);
    }

    private void Update()
    {
        currentState?.Update();

        verticalVelocity = -50f * Time.deltaTime;

        Vector3 move = Vector3.zero;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            move = moveDir * 6f * Time.deltaTime;
        }
        else moveDir = Vector3.zero;

            move.y += verticalVelocity;


        _controller.Move(move); // tốc độ di chuyển

        if (toMoveNode != null)
        {
            Vector3 direction = toMoveNode.transform.position - transform.position;
            direction.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 20 * Time.deltaTime);
        }
    }

    #region Move
    public void RollDice()
    {
        StartCoroutine(RollDiceCoroutine());
        HideDice();
    }

    IEnumerator RollDiceCoroutine()
    {
        StepsLeft = 2;

        ChangeAnimation("RollDice");
        yield return new WaitForSecondsRealtime(1f);
        if (currentNode.nextNodes.Count > 1)
        {
            ChangeState(chooseDirectionState);
        }
        else
            ChangeState(movingState);
    }

    public void SetStepLeft(int step)
    {
        StepsLeft = step;
    }

    // Di chuyển từng bước
    public bool MoveStep()
    {
        if (StepsLeft <= 0 || toMoveNode == null) return false;

        moveDir = (toMoveNode.transform.position - feet.position).normalized;
        moveDir.y = 0;

        if (Vector3.Distance(feet.position, toMoveNode.transform.position) < 0.3f)
        {
            moveDir = Vector3.zero;
            currentNode = toMoveNode;
            WizardPartyData.instance.UpdatePlayerNode(playerInput, currentNode);
            StepsLeft--;

            if (StepsLeft > 0)
            {
                if (currentNode.nextNodes.Count > 1)
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

    #endregion

    // Item (tạm bỏ qua inventory)
    public void UseSelectedItem()
    {
        Debug.Log("💥 Sử dụng item!");
        // Sau này xử lý skill / bắn súng / trap
    }

    public void StartTurn()
    {
        StartCoroutine(ShowTurnAvatarUI());
        CameraFollow.instance.StartFollowTarget(transform);
    }

    IEnumerator ShowTurnAvatarUI()
    {
        AvatarTurnManager.instance.gameObject.SetActive(true);
        AvatarTurnManager.instance.HighlightTurn(PlayerManager.instance.players.IndexOf(playerInput));
        yield return new WaitForSeconds(3f);
        _controller.enabled = true;
        ShowDice();
        AvatarTurnManager.instance.gameObject.SetActive(false);
    }

    // --- Hàm kết thúc lượt ---
    public void EndTurn()
    {
        TurnManager.instance.NextTurn();
        this.enabled = false;
    }


    // --- Spawn các mũi tên chọn hướng khi tới ngã ba ---
    public void ShowDirectionChoices()
    {
        ClearArrow();
        for (int i = 0; i < currentNode.nextNodes.Count; i++)
        {
            BoardNode next = currentNode.nextNodes[i];
            Vector3 midPoint = (currentNode.transform.position + next.transform.position) / 2;
            midPoint.y = arrowDirectionPrefab.transform.position.y;

            ArrowPointer arrow = Instantiate(arrowDirectionPrefab, midPoint, Quaternion.identity).GetComponent<ArrowPointer>();
            arrow.transform.rotation = Quaternion.LookRotation((next.transform.position - currentNode.transform.position), Vector3.up);
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


    public void ChooseDirection(int index)
    {
        toMoveNode = currentNode.nextNodes[index];
        ChangeState(movingState);
    }

    private void ShowDice()
    {
        dice.SetActive(true);
    }

    private void HideDice()
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

    public void DisableRagdoll()
    {
        foreach (var rigid in rigidbodies)
        {
            rigid.isKinematic = true;
        }
    }

    public void EnableRagdoll()
    {
        Vector3 spawnPosition = transform.position + new Vector3(0, 15, 0);
        ChangeState(idleState);
        var clone = Instantiate(gameObject, spawnPosition, Quaternion.identity);
        clone.GetComponent<PlayerController>().enabled = false;
        TurnManager.instance.UpdateController(playerInput, clone.GetComponent<NewBoardGameController>());

        animator.enabled = false;
        _controller.enabled = false;
        foreach(var rigid in rigidbodies)
        {
            rigid.isKinematic = false;
        }

        StartCoroutine(DelayDestroy(clone));
    }

    IEnumerator DelayDestroy(GameObject clone)
    {
        yield return new WaitForSeconds(2.5f);
        clone.GetComponent<PlayerController>().enabled = true;
        Destroy(gameObject);
    }


}
