using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//Đang bị lỗi update currentNode do chỉ có host mới có quyền thay đổi cho nên là client sẽ vẫn thấy current node là node cũ
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

    [Header("ArrowDirection")]
    public GameObject arrowDirectionPrefab;
    public List<GameObject> spawnedArrows = new List<GameObject>();

    bool isMyTurn => TurnManager.instance.currentPlayerRef == Runner.LocalPlayer;

    [Header("Dice and Step")]
    public GameObject dicePrefab;
    private Dice activeDice;
    public TextMeshPro stepText;

    private enum MoveState { Idle, Rolling, WaitingForAnim, Moving }
    private MoveState moveState = MoveState.Idle;
    private MoveState currentState;

    private float animTimer = 0f;
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
        }

        if (currentNodeName != null)
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
                    predictedRoll = true;
                    int predictedSteps = Random.Range(1, 5);
                    currentStep = predictedSteps;
                    stepText.gameObject.SetActive(true);
                    stepText.text = predictedSteps.ToString();
                    RPC_RequestDiceRoll();
                }
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

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

            if (Vector3.Distance(transform.position, toMoveNode.transform.position) <= 0.5f)
            {
                currentNode = toMoveNode;
                currentStep--;

                if (currentStep > 0)
                {
                    if (currentNode.nextNodes.Count > 1)
                    {
                        waitingForChoice = true;
                        ShowDirectionChoices();
                        moveState = MoveState.Idle;
                        animator.CrossFade("Idle", 0.25f);
                        return;
                    }
                    else
                    {
                        toMoveNode = currentNode.nextNodes[0];
                    }
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

        if (currentState != moveState)
        {
            currentState = moveState;
            RPC_HandleAnim(moveState);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_HandleAnim(MoveState moveState)
    {
        switch (moveState)
        {
            case MoveState.Idle:
                animator.CrossFade("Idle", 0.25f);
                break;
            case MoveState.Moving:
                animator.CrossFade("Run", 0.1f);
                break;
            case MoveState.WaitingForAnim:
                break;
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

        foreach (var player in players)
        {
            PlayerRef playerRef = player.GetComponent<NetworkObject>().InputAuthority;
            string currentNodeName = player.currentNode.name;
            boardGameData.UpdateNode(playerRef, currentNodeName);
        }

        TurnManager.instance.RequestNextTurn();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestDiceRoll()
    {
        if (HasStateAuthority && moveState == MoveState.Idle)
        {
            StartMove();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowDice()
    {
        if (activeDice != null || !Object.HasStateAuthority) return;
        activeDice = Runner.Spawn(dicePrefab, transform.position + new Vector3(0, 5f, 0), Quaternion.identity).GetComponent<Dice>();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestShowDice()
    {
        RPC_ShowDice();
    }

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

        RPC_Move(currentStep);
    }

    void ShowDirectionChoices()
    {
        ClearArrow();
        animator.CrossFade("Idle", 0.25f);
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
        ClearArrow();
        animator.CrossFade("Run", 0.25f);
        toMoveNode = currentNode.nextNodes[index];
        waitingForChoice = false;
        moveState = MoveState.Moving;
    }

    void TriggerNodeEvent()
    {
        Debug.Log("Gọi sự kiện của node: " + currentNode.name);
        if (HasStateAuthority)
            currentNode.RPC_ProcessNode(Runner.LocalPlayer);
    }
}
