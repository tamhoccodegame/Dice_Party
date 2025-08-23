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
    public int currentHoverArrowIndex;
    public ArrowPointer hoverArrow;
    public List<GameObject> spawnedArrows = new List<GameObject>(); // danh sách các mũi tên đã spawn ra

    [Header("Effect")]
    public ParticleSystem rollDiceEffect;

    [Header("ItemGun")]
    public BoardItem gun;
    public Transform gunSpawnPoint;

    [Header("ItemShit")]
    public BoardItem shitItem;
    public Transform shitSpawnPoint;

    [Header("ItemHorse")]
    public BoardItem horseItem;
    public Transform horseSpawnPoint;

    private Rigidbody[] rigidbodies;

    public bool readyForInput = false;

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
        
        
        enabled = TurnManager.instance.playerControllers[playerInput] == this;
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
        Debug.Log($"Change to {newState}");
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
        if (currentAnim == animName) return;
        currentAnim = animName;
        animator.CrossFade(animName, 0.25f);
    }

    private void Update()
    {
        currentState?.Update();

        if (Input.GetKeyDown(KeyCode.F))
        {
            UseSelectedItem();
        }

        verticalVelocity = -5f * Time.deltaTime;

        Vector3 move = Vector3.zero;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            move = moveDir * 6f * Time.deltaTime;
        }
        else moveDir = Vector3.zero;

        move.y += verticalVelocity;


        _controller.Move(move); // tốc độ di chuyển

        if (toMoveNode != null && currentState != itemState)
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
        StepsLeft = Random.Range(1, 6);
        //StepsLeft = 99;

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

    public void SetCurrentNode(BoardNode node)
    {
        currentNode = node;
        toMoveNode = currentNode.nextNodes[0];
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

            if(currentNode is ChestGoldNode chest)
            {
                TurnManager tm = TurnManager.instance;

                int index = 0;
                for(int i = 0; i < tm.chestGolds.Length; i++)
                {
                    if(tm.chestGolds[i] == chest.transform)
                    {
                        break;
                    }
                    index++;
                }

                if (index == WizardPartyData.instance.currentChestIndex)
                {
                    ChangeState(nodeState);
                    return true;
                }
            }

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
        if (gun == null)
        {
            Debug.LogWarning("No item to use.");
            return;
        }

        ChangeState(itemState);
    }

    public void StartTurn()
    {
        StartCoroutine(ShowTurnAvatarUI());
        CameraFollow.instance.StartFollowTarget(transform);
    }

    IEnumerator ShowTurnAvatarUI()
    {
        AvatarTurnManager.instance.gameObject.SetActive(true);
        AvatarTurnManager.instance.Appear();
        AvatarTurnManager.instance.HighlightTurn(PlayerManager.instance.players.IndexOf(playerInput));
        yield return new WaitForSeconds(3f);
        readyForInput = true;
        _controller.enabled = true;
        ShowDice();
        AvatarTurnManager.instance.Disappear();
        AvatarTurnManager.instance.gameObject.SetActive(false);
    }

    // --- Hàm kết thúc lượt ---
    public void EndTurn()
    {
        TurnManager.instance.NextTurn();
        readyForInput = false;
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
            if (i == 0)
            {
                currentHoverArrowIndex = 0;
                hoverArrow = arrow;
                arrow.Hover();
            }
            else
                arrow.UnHover();

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


    public void NextHoverArrow()
    {
        currentHoverArrowIndex = (currentHoverArrowIndex + 1) % spawnedArrows.Count;

        if (hoverArrow != null)
            hoverArrow.UnHover();

        hoverArrow = spawnedArrows[currentHoverArrowIndex].GetComponent<ArrowPointer>();
        hoverArrow.Hover();
    }

    public void PrevHoverArrow()
    {
        currentHoverArrowIndex -= 1;
        if(currentHoverArrowIndex < 0) currentHoverArrowIndex = spawnedArrows.Count - 1;

        if (hoverArrow != null)
            hoverArrow.UnHover();

        hoverArrow = spawnedArrows[currentHoverArrowIndex].GetComponent<ArrowPointer>();
        hoverArrow.Hover();
    }

    public void ChooseDirection()
    {
        ClearArrow();
        toMoveNode = currentNode.nextNodes[currentHoverArrowIndex];
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
        Vector3 spawnPosition = transform.position;
        ChangeState(idleState);
        var clone = Instantiate(gameObject, spawnPosition, Quaternion.identity);
        clone.GetComponent<CharacterController>().enabled = false;
        clone.GetComponent<NewBoardGameController>().enabled = false;
        clone.GetComponent<Animator>().enabled = false;

        clone.transform.position += new Vector3(0, 25, 0);


        _controller.enabled = false;
        animator.enabled = false;
        foreach (var rigid in rigidbodies)
        {
            rigid.isKinematic = false;
        }

        StartCoroutine(DelayDestroy(clone));
    }

    IEnumerator DelayDestroy(GameObject clone)
    {
        TurnManager.instance.UpdateController(playerInput, clone.GetComponent<NewBoardGameController>());
        yield return new WaitForSeconds(2.5f);
        clone.GetComponent<NewBoardGameController>().enabled = true;
        clone.GetComponent<CharacterController>().enabled = true;
        clone.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(1.5f);
        clone.GetComponent<NewBoardGameController>().enabled = false;
        Destroy(gameObject);
    }


}
