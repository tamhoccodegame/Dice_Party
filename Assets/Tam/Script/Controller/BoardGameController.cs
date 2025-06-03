using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BoardGameController : NetworkBehaviour
{
    [Header("Stat")]
    public float moveSpeed;
    public float rotationSpeed;

    [Header("Move")]
    public BoardNode currentNode;
    public BoardNode toMoveNode;

    [Networked] public int currentStep { get; set; }
    private CharacterController controller;
    private Animator animator;
    [Networked] public bool waitingForChoice { get; set; }

    [Space(20)]
    [Header("ArrowDirection")]
    public GameObject arrowDirectionPrefab;
    public List<GameObject> spawnedArrows = new List<GameObject>();

    // Lưu ý: host là state authority
    bool isMyTurn => TurnManager.instance.currentPlayerRef == Runner.LocalPlayer;

    [Space(20)]
    [Header("Dice and Step")]
    public GameObject dicePrefab;
    private Dice activeDice;
    public TextMeshPro stepText;

    // State Machine
    private enum MoveState { Idle, Rolling, WaitingForAnim, Moving }
    private MoveState moveState = MoveState.Idle;
    private MoveState currentState;

    private float animTimer = 0f;

    // Prediction flag
    private bool predictedRoll = false;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();

        string currentNodeName = null;

        BoardGameData gameData = BoardGameData.instance;
        if (gameData.playerCurrentNode.Count > 0)
        {
            currentNodeName = gameData.GetNode(Runner.LocalPlayer);
            Debug.Log(currentNodeName);

        }

        if(currentNodeName != null)
        {
            currentNode = GameObject.Find(currentNodeName).GetComponent<BoardNode>();
        }
        else
        {
            currentNode = GameObject.Find("Dice (7)").GetComponent<BoardNode>();
        }

        toMoveNode = currentNode.nextNodes[0];
    }

    void Update()
    {
        if (HasInputAuthority && isMyTurn)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!predictedRoll && moveState == MoveState.Idle)
                {
                    // Prediction: client đoán luôn bước đi (random)
                    predictedRoll = true;
                    int predictedSteps = Random.Range(1, 5);
                    currentStep = predictedSteps;
                    stepText.gameObject.SetActive(true);
                    stepText.text = predictedSteps.ToString();

                    // Gọi RPC request lên host thật sự
                    RPC_RequestDiceRoll();
                }
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return; // Chỉ host xử lý logic di chuyển

        if (moveState == MoveState.Moving)
        {
            Vector3 direction = (toMoveNode.transform.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
                transform.rotation = newRotation;
            }

            Vector3 moveDir = direction * moveSpeed * Runner.DeltaTime;
            controller.Move(moveDir);

            if (Vector3.Distance(transform.position, toMoveNode.transform.position) <= 0.5f)
            {
                currentNode = toMoveNode;
                currentStep--;

                if (currentStep > 0)
                {
                    toMoveNode = currentNode.nextNodes[0];
                }
                else
                {
                    moveState = MoveState.Idle;
                    animator.CrossFade("Idle", 0.25f);
                    TriggerNodeEvent();
                    EndTurn();
                }
            }
        }

        if (moveState == MoveState.WaitingForAnim)
        {
            animTimer -= Runner.DeltaTime;
            if (animTimer <= 0f)
            {
                animator.CrossFade("Run", 0.25f);
                moveState = MoveState.Moving;
            }
        }
        
        if(currentState != moveState)
        {
            currentState = moveState;
            RPC_HandleAnim(moveState);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_HandleAnim(MoveState moveState)
    {
        switch(moveState)
        {
            case MoveState.Idle:
                animator.CrossFade("Idle", 0.25f);
                break;
            case MoveState.Moving:
                animator.CrossFade("Run", 0.1f);
                break;
            case MoveState.WaitingForAnim:
                break;
            default: break;
        }
    }

    public void StartTurn()
    {
        if (Object.HasInputAuthority)
        {
            RPC_RequestShowDice();
        }
    }

    public void EndTurn()
    {
        BoardGameData boardGameData = BoardGameData.instance;

        BoardGameController[] players = FindObjectsByType<BoardGameController>(FindObjectsSortMode.None);

        foreach(var player in players)
        {
            PlayerRef playerRef = player.GetComponent<NetworkObject>().InputAuthority;
            string currentNodeName = player.currentNode.name;

            boardGameData.UpdateNode(playerRef, currentNodeName);
            Debug.Log("Updated Board Game Data");
        }

        TurnManager.instance.RequestNextTurn();
    }

    // Client gọi RPC request lên host (state authority)
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestDiceRoll()
    {
        if (HasStateAuthority && moveState == MoveState.Idle)
        {
            StartMove();
        }
    }

    // Host gọi RPC multicast cho tất cả client để show dice
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowDice()
    {
        if (activeDice != null || !Object.HasStateAuthority) return;

        activeDice = Runner.Spawn(dicePrefab, transform.position + new Vector3(0, 5f, 0), Quaternion.identity).GetComponent<Dice>();
    }

    // Client gọi RPC request show dice
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestShowDice()
    {
        RPC_ShowDice();
    }

    // Host gọi RPC multicast cho tất cả client bắt đầu di chuyển với steps
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_Move(int steps)
    {
        currentStep = steps;
        stepText.gameObject.SetActive(true);
        stepText.text = steps.ToString();

        animator.CrossFade("RollDice", 0.25f);
        animTimer = 0.25f + animator.GetCurrentAnimatorStateInfo(0).length;
        stepText.gameObject.SetActive(false);
        moveState = MoveState.WaitingForAnim;

        // Reset prediction khi nhận dữ liệu từ host
        predictedRoll = false;

        if (!HasStateAuthority && moveState == MoveState.Idle)
        {
            animator.CrossFade("Run", 0.25f);
            moveState = MoveState.Moving;
        }
    }

    private void StartMove()
    {
        currentStep = Random.Range(1, 5);
        stepText.gameObject.SetActive(true);
        stepText.text = currentStep.ToString();

        if (activeDice != null)
        {
            activeDice.RequestDestroyDice();
            activeDice = null;
        }

        animator.CrossFade("RollDice", 0.25f);
        animTimer = 0.25f + animator.GetCurrentAnimatorStateInfo(0).length;
        stepText.gameObject.SetActive(false);
        moveState = MoveState.WaitingForAnim;

        // Đồng bộ bước đi với client
        RPC_Move(currentStep);
    }

    void ShowDirectionChoices()
    {
        animator.CrossFade("Idle", 0.25f);
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

        Debug.Log("Nhấn 1, 2, 3 để chọn hướng!");
    }

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
        animator.CrossFade("Run", 0.25f);
        ClearArrow();
        Debug.Log("Bạn chọn hướng: " + (index + 1));
        toMoveNode = currentNode.nextNodes[index];
        waitingForChoice = false;
    }

    void TriggerNodeEvent()
    {
        Debug.Log("Gọi sự kiện của node: " + currentNode.name);
        if (HasStateAuthority)
            currentNode.RPC_ProcessNode(Runner.LocalPlayer);
    }
}
