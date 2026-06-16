using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

[RequireComponent(typeof(CharacterController))]
public class NewBoardGameController : PlayerController
{
    #region === Core Components ===
    private Animator animator;
    private CharacterController _controller;
    private Rigidbody[] rigidbodies;
    #endregion


    #region === State Machine ===
    private BoardState currentState;

    public enum DebugState
    {
        Idle,
        Moving,
        ChooseDirection,
        Item,
        Node,
    }

    [Header("Debug")]
    public DebugState debugState;

    public IdleState idleState;
    public MovingState movingState;
    public ChooseDirectionState chooseDirectionState;
    public ItemState itemState;
    public NodeState nodeState;
    #endregion


    #region === Input ===
    [Header("Input")]
    public PlayerInput playerInput;
    public bool readyForInput = false;
    #endregion


    #region === Animation ===
    public string currentAnim { get; set; }
    #endregion


    #region === Movement ===
    [Header("Movement")]
    float verticalVelocity;
    public Vector3 moveDir;

    public Transform feet;
    #endregion

    #region === New Movement ==
    [Header("New Movement")]
    public SplineAnimate splineAnimate;
    #endregion

    #region === Board / Node ===
    [Header("Board")]
    public BoardNode currentNode;
    public int currentNodeId;             // Chỉ dùng local scene
    public string currentNodeName { get; set; }
    public BoardNode toMoveNode;
    #endregion


    #region === Item ===
    [Header("Item Controller")]
    public ItemController itemController;
    #endregion


    #region === Dice & Step UI ===
    [Header("Dice And Step")]
    public GameObject dice;                 // xúc xắc đang spawn trên scene
    public GameObject stepTextPrefab;       // text hiện số bước
    #endregion


    #region === Direction Arrow ===
    [Header("Arrow Direction")]
    public GameObject arrowDirectionPrefab;
    public int currentHoverArrowIndex;
    public ArrowPointer hoverArrow;
    public List<GameObject> spawnedArrows = new List<GameObject>();
    #endregion


    #region === Effect ===
    [Header("Effect")]
    public ParticleSystem rollDiceEffect;
    #endregion


    #region === Events / Runtime ===
    private System.Action onDirectionChose;
    #endregion

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


        //if (playerInput == null)
        //{
        //    playerInput = gameObject.AddComponent<PlayerInput>();
        //    // Load Input Action Asset
        //    var asset = Resources.Load<InputActionAsset>("InputAction/DefaultInputActions");
        //    playerInput.actions = asset;

        //    // Enable toàn bộ actions
        //    playerInput.actions.Enable();

        //    // Chọn map chính
        //    playerInput.defaultActionMap = "Player";
        //    playerInput.SwitchCurrentActionMap("Player");
        //    readyForInput = true;
        //    // Mock keyboard
        //    playerInput.neverAutoSwitchControlSchemes = true;
        //}

        //if (TurnManager.instance == null) DisableRagdoll();

    }

    private void OnEnable()
    {
        if (WizardPartyData.instance == null) return;

        splineAnimate.enabled = true;
        splineAnimate.Container = GameObject.Find("Spline Start").GetComponent<SplineContainer>();

       WizardPartyData.instance.playersNode.TryGetValue(gameObject, out var savedNode);
        if (savedNode != null)
        {
            BoardNode node = GameObject.Find(savedNode.name).GetComponent<BoardNode>();
            _controller.enabled = false;
            transform.position = node.transform.position;
            currentNode = node;
            splineAnimate.Container = node.splineContainer;
            splineAnimate.NormalizedTime = node.normalizeTime;
            _controller.enabled = true;
        }
        else
        {
            currentNode = PlayerSetupPosition.instance.spawnPosition[0].GetComponent<BoardNode>();
        }

        toMoveNode = currentNode.nextNodes[0];


        //enabled = TurnManager.instance.playerControllers[gameObject] == this;

        ChangeState(idleState);
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
                debugState = DebugState.Idle;
                break;
            case MovingState:
                currentState = movingState;
                debugState = DebugState.Moving;
                break;
            case ChooseDirectionState:
                currentState = chooseDirectionState;
                debugState = DebugState.ChooseDirection;
                break;
            case ItemState:
                currentState = itemState;
                debugState = DebugState.Item;
                break;
            case NodeState:
                currentState = nodeState;
                debugState = DebugState.Node;
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
        var step = Random.Range(1, 7);
        movingState.stepLeft = 1;
        //StepsLeft = 99;
        ChangeAnimation("RollDice");
        yield return new WaitForSecondsRealtime(1f);
        if (currentNode.nextNodes.Count > 1)
        {
            ChangeState(chooseDirectionState);
        }
        else
        {
            ChangeState(movingState);
        }
    }

    public void SetStepLeft(int step)
    {
        movingState.stepLeft = step;
    }

    public void SetCurrentNode(BoardNode node)
    {
        currentNode = node;
        toMoveNode = currentNode.nextNodes[0];
        WizardPartyData.instance.UpdatePlayerNode(gameObject, currentNode);
    }

    #endregion

    public void StartTurn()
    {
        StartCoroutine(ShowTurnAvatarUI());
        CameraFollow.instance.StartFollowTarget(transform);
    }

    IEnumerator ShowTurnAvatarUI()
    {
        AvatarTurnManager.instance.gameObject.SetActive(true);
        AvatarTurnManager.instance.Appear();
        //AvatarTurnManager.instance.HighlightTurn(PlayerManager.instance.players.IndexOf(playerInput));
        yield return new WaitForSeconds(3f);
        readyForInput = true;
        _controller.enabled = true;
        ShowDice();
        AvatarTurnManager.instance.Disappear();
        yield return new WaitForSeconds(1f);
        AvatarTurnManager.instance.gameObject.SetActive(false);
    }

    // --- Hàm kết thúc lượt ---
    public void EndTurn()
    {
        TurnManager.instance.NextTurn();
        readyForInput = false;
        this.enabled = false;
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
        step.Init(movingState.stepLeft.ToString());
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
        TurnManager.instance.UpdateController(gameObject, clone.GetComponent<NewBoardGameController>());
        yield return new WaitForSeconds(2.5f);
        clone.GetComponent<NewBoardGameController>().enabled = true;
        clone.GetComponent<CharacterController>().enabled = true;
        clone.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(1.5f);
        clone.GetComponent<NewBoardGameController>().enabled = false;
        Destroy(gameObject);
    }

}
