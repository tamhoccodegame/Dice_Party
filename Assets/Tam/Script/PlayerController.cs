using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Stat")]
    public float moveSpeed;
    public float rotationSpeed;

    [Header("Move")]
    public BoardNode currentNode;
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

    private Coroutine moveCoroutine;
    private bool isPressed;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentNode = FindFirstObjectByType<BoardNode>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isMyTurn && moveCoroutine == null)
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
                activeDice.DestroySelf();
                activeDice = null;
            }

            RPC_RequestMove(currentStep); // ⬅️ Gửi yêu cầu đến host
        }

        Vector3 direction = (currentNode.transform.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero) // Tránh lỗi khi direction là zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
            transform.rotation = newRotation;
        }
    }

    public void StartTurn()
    {
        if (HasStateAuthority)
        {
            RPC_ShowDice();
        }
    }

    public void EndTurn()
    {
        if (HasStateAuthority)
        {
            TurnManager.instance.NextTurn();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowDice()
    {
        if (activeDice != null || !Object.HasStateAuthority) return;

        activeDice = Runner.Spawn(dicePrefab, transform.position + new Vector3(0, 3.5f, 0), Quaternion.identity).GetComponent<Dice>();
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
        moveCoroutine = StartCoroutine(MoveToNextNode());
    }

    IEnumerator MoveToNextNode()
    {
        animator.CrossFade("RollDice", 0.25f);
        yield return new WaitForSeconds(0.25f);
        stepText.gameObject.SetActive(false);
        float animTime = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animTime);

        animator.CrossFade("Run", 0.25f);
        while (currentStep > 0)
        {
            if (currentNode.nextNodes.Count > 1)
            {
                waitingForChoice = true;
                ShowDirectionChoices();

                while (waitingForChoice) yield return null;
            }
            else
            {
                currentNode = currentNode.nextNodes[0];
            }

            while (Vector3.Distance(transform.position, currentNode.transform.position) > 1.3f)
            {
                Vector3 direction = (currentNode.transform.position - transform.position).normalized;
                Vector3 movement = direction * moveSpeed * Runner.DeltaTime;

                controller.Move(movement);
                yield return null;
            }

            currentStep--;
        }

        animator.CrossFade("Idle", 0.25f);
        TriggerNodeEvent();
        moveCoroutine = null;
        EndTurn();
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
        currentNode = currentNode.nextNodes[index];
        waitingForChoice = false;
        //Move
    }

    void TriggerNodeEvent()
    {
        Debug.Log("Gọi sự kiện của node: " + currentNode.name);
        currentNode.ProcessNode();
    }
}
