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

    [Header("ArrowDirection")]
    public GameObject arrowDirectionPrefab;
    public List<GameObject> spawnedArrows = new List<GameObject>();

    bool isMyTurn => TurnManager.instance.currentPlayerRef == Runner.LocalPlayer;

    [Header("Dice and Step")]
    public GameObject dicePrefab;
    private Dice activeDice;
    public TextMeshPro stepText;

    private enum MoveState { Idle, Rolling, WaitingForAnim, Moving }
    [Networked] private MoveState moveState { get; set; }

    private float animTimer = 0f;

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
        stepText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (HasInputAuthority && isMyTurn)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (moveState == MoveState.Idle)
                {
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

        if (moveState == MoveState.WaitingForAnim)
        {
            animTimer -= Runner.DeltaTime;
            if (animTimer <= 0f)
            {
                SetMoveState(MoveState.Moving);
            }
        }
    }

    private void SetMoveState(MoveState newState)
    {
        moveState = newState;
        UpdateAnimation();
    }

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

    public void StartTurn()
    {
        if (HasInputAuthority)
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
        if (moveState != MoveState.Idle) return;

        currentStep = Random.Range(1, 5);
        SetMoveState(MoveState.WaitingForAnim);

        RPC_UpdateStepText(currentStep);

        animTimer = 1f; // Thời gian animation roll
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateStepText(int steps)
    {
        currentStep = steps;
        stepText.text = steps.ToString();
        stepText.gameObject.SetActive(true);
        Runner.Despawn(activeDice.Object);
        StartCoroutine(HideStepText());
    }

    private IEnumerator HideStepText()
    {
        yield return new WaitForSeconds(1f);
        stepText.gameObject.SetActive(false);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestShowDice()
    {
        RPC_ShowDice();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowDice()
    {
        if (activeDice != null) return;

        Vector3 spawnPos = transform.position + new Vector3(0, 5f, 0);
        activeDice = Runner.Spawn(dicePrefab, spawnPos, Quaternion.identity).GetComponent<Dice>();
    }

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
        toMoveNode = currentNode.nextNodes[index];
        waitingForChoice = false;
        SetMoveState(MoveState.Moving);
    }

    void TriggerNodeEvent()
    {
        Debug.Log("Gọi sự kiện của node: " + currentNode.name);
        if (HasStateAuthority)
            currentNode.RPC_ProcessNode(Runner.LocalPlayer);
    }
}
