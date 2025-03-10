using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    public BoardNode currentNode;
    public int currentStep;
    private NavMeshAgent agent;
    public bool isMoving = false;
    public bool waitingForChoice = false;
    public bool eventTriggered = false; // Để tránh gọi event nhiều lần

    public GameObject arrowDirectionPrefab;
    public List<GameObject> spawnedArrows = new List<GameObject>();

    private Coroutine moveCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && moveCoroutine == null)
        {
            currentStep = 3;
            moveCoroutine = StartCoroutine(MoveToNextNode());
        }

    }

    IEnumerator MoveToNextNode()
    {
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

            while (Vector3.Distance(transform.position, currentNode.transform.position) > 1.8f)
            {
                yield return null;
            }

            currentStep--;
        }
        yield return null;
        TriggerNodeEvent();
        moveCoroutine = null;
    }
    void ShowDirectionChoices()
    {
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
        ClearArrow();
        Debug.Log("Bạn chọn hướng: " + (index + 1));
        currentNode = currentNode.nextNodes[index];
        waitingForChoice = false; // Hủy trạng thái chờ chọn hướng
        isMoving = true;
        agent.SetDestination(currentNode.transform.position);
    }

    void TriggerNodeEvent()
    {
        Debug.Log("Gọi sự kiện của node: " + currentNode.name);
        currentNode.ProcessNode(); // Giả sử BoardNode có hàm này
    }

}
