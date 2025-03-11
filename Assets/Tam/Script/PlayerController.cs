using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public BoardNode currentNode;
    public int currentStep;
    private NavMeshAgent agent;
    private Animator animator;
    public bool waitingForChoice = false;

    [Space(20)]
    [Header("ArrowDirection")]
    public GameObject arrowDirectionPrefab;
    public List<GameObject> spawnedArrows = new List<GameObject>();

    [Space(20)]
    [Header("CanMove")]
    public bool isMyTurn = false;

    [Space(20)]
    [Header("Dice and Step")]
    public GameObject dicePrefab;
    private Dice activeDice;
    public TextMeshPro stepText;

    private Coroutine moveCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.autoBraking = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && moveCoroutine == null && isMyTurn)
        {
            currentStep = Random.Range(1, 5);
            stepText.gameObject.SetActive(true);
            stepText.text = currentStep.ToString();
            activeDice.DestroySelf();
            moveCoroutine = StartCoroutine(MoveToNextNode());
        }

    }

    public void StartTurn()
    {
        isMyTurn = true;
        activeDice = Instantiate(dicePrefab, transform.position + new Vector3(0, 3.5f, 0), Quaternion.identity).GetComponent<Dice>();
    }

    public void EndTurn()
    {
        isMyTurn = false;
        GameManager.instance.NextTurn();
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
                // Đợi người chơi chọn hướng nếu có nhiều hơn một lựa chọn
                waitingForChoice = true;
                ShowDirectionChoices();
                while (waitingForChoice) // Chờ người chơi chọn hướng
                {
                    yield return null;
                }
            }
            else
            {
                currentNode = currentNode.nextNodes[0];
            }

            agent.SetDestination(currentNode.transform.position);

            while (Vector3.Distance(transform.position, currentNode.transform.position) > 1.7f)
            {
                yield return null;
            }

            currentStep--;
        }
        yield return null;
        animator.CrossFade("Idle", 0.25f);
        isMyTurn = false;
        TriggerNodeEvent();
        moveCoroutine = null;
        EndTurn();
    }
    void ShowDirectionChoices()
    {
        animator.CrossFade("Idle", 0.25f);
        ClearArrow();
        foreach (var next in currentNode.nextNodes)
        {
            Vector3 midPoint = (currentNode.transform.position + next.transform.position) / 2;
            midPoint.y = arrowDirectionPrefab.transform.position.y;
            ArrowPointer arrow = Instantiate(arrowDirectionPrefab, midPoint, Quaternion.identity).GetComponent<ArrowPointer>();
            arrow.transform.rotation = Quaternion.LookRotation((next.transform.position - currentNode.transform.position), Vector3.up);
            arrow.Setup(this, currentNode.nextNodes.IndexOf(next));
            spawnedArrows.Add(arrow.gameObject);
        }
        // Giả lập hiển thị UI chọn hướng
        Debug.Log("Nhấn 1, 2, 3 để chọn hướng!");
    }

    void ClearArrow()
    {
        if (spawnedArrows.Count <= 0) return;
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
        waitingForChoice = false; // Hủy trạng thái chờ chọn hướng
        agent.SetDestination(currentNode.transform.position);
    }

    void TriggerNodeEvent()
    {
        Debug.Log("Gọi sự kiện của node: " + currentNode.name);
        currentNode.ProcessNode(); // Giả sử BoardNode có hàm này
    }

}
