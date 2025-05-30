using Fusion;
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

    bool isMyTurn => TurnManager.instance.currentPlayerRef == Runner.LocalPlayer;

    [Space(20)]
    [Header("Dice and Step")]
    public GameObject dicePrefab;
    private Dice activeDice;
    public TextMeshPro stepText;

    private bool isPressed;

    // State Machine
    private enum MoveState { Idle, Rolling, WaitingForAnim, Moving }
    private MoveState moveState = MoveState.Idle;

    private float animTimer = 0f;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
        currentNode = GameObject.Find("Dice (7)").GetComponent<BoardNode>();
        toMoveNode = currentNode.nextNodes[0];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isMyTurn && moveState == MoveState.Idle)
        {
            isPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (isPressed)
        {
            isPressed = false;
            currentStep = Random.Range(1, 5);
            stepText.gameObject.SetActive(true);
            stepText.text = currentStep.ToString();

            if (activeDice != null)
            {
                activeDice.RequestDestroyDice();
                activeDice = null;
            }

            if (Object.HasInputAuthority)
                RPC_RequestMove(currentStep);
        }

        // Handle rotation
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

        // Handle animation wait
        if (moveState == MoveState.WaitingForAnim)
        {
            animTimer -= Runner.DeltaTime;
            if (animTimer <= 0f)
            {
                animator.CrossFade("Run", 0.25f);
                moveState = MoveState.Moving;
            }
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
        TurnManager.instance.RequestNextTurn();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestShowDice()
    {
        RPC_ShowDice();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowDice()
    {
        if (activeDice != null || !Object.HasStateAuthority) return;

        activeDice = Runner.Spawn(dicePrefab, transform.position + new Vector3(0, 5f, 0), Quaternion.identity).GetComponent<Dice>();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestMove(int steps)
    {
        RPC_Move(steps);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_Move(int steps)
    {
        currentStep = steps;
        stepText.gameObject.SetActive(true);
        stepText.text = steps.ToString();

        if (HasStateAuthority && moveState == MoveState.Idle)
        {
            animator.CrossFade("RollDice", 0.25f);
            animTimer = 0.25f + animator.GetCurrentAnimatorStateInfo(0).length;
            stepText.gameObject.SetActive(false);
            moveState = MoveState.WaitingForAnim;
        }
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
