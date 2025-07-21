using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NewBoardGameController : MonoBehaviour
{
    private Animator animator;
    private CharacterController _controller;

    private BoardState currentState;
    public string currentAnim { get; set; }

    public IdleState idleState;
    public MovingState movingState;
    public ChooseDirectionState chooseDirectionState;
    public ItemState itemState;
    public NodeState nodeState;

    public enum NetworkState
    {
        Idle,
        Moving,
        ChooseDirection,
        Item,
        Node,
    }

    public int StepsLeft { get; set; }
    public bool isMyTurn => true;//asdsa;

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

    public void Awake()
    {
        animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        idleState = new IdleState(this);
        movingState = new MovingState(this);
        chooseDirectionState = new ChooseDirectionState(this);
        itemState = new ItemState(this);
        nodeState = new NodeState(this);
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
    }

    //public void Hurt(PlayerRef player, int ammount)
    //{
    //    StartCoroutine(HurtCoroutine(player, ammount));
    //}

    //public void Heal(PlayerRef player, int ammount)
    //{
    //    StartCoroutine(HealthCoroutine(player, ammount));
    //}

    //private IEnumerator HurtCoroutine(PlayerRef player, int ammount)
    //{
    //    string previousAnim = currentAnim;
    //    Debug.Log("💢 Bị đau!");
    //    RequestChangeAnimation("Hurt");
    //    TurnManager.instance.RequestUpdateHealth(player, -ammount);
    //    // hiệu ứng bị thương
    //    yield return new WaitForSecondsRealtime(0.5f);

    //    RequestChangeAnimation(previousAnim);
    //}

    //private IEnumerator HealthCoroutine(PlayerRef player, int ammount)
    //{
    //    string previousAnim = currentAnim;
    //    Debug.Log("❤️ Hồi máu!");
    //    RequestChangeAnimation("Heal");
    //    TurnManager.instance.RequestUpdateHealth(player, ammount);
    //    // hiệu ứng hồi máu
    //    yield return new WaitForSecondsRealtime(1f);

    //    RequestChangeAnimation(previousAnim);
    //}


    #region Move
    public void RollDice()
    {
        StartCoroutine(RollDiceCoroutine());
    }

    IEnumerator RollDiceCoroutine()
    {
            StepsLeft = Random.Range(1, 10);

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

        Vector3 dir = (toMoveNode.transform.position - feet.position).normalized;
        dir.y = 0;

        _controller.Move(dir); // tốc độ di chuyển

        if (Vector3.Distance(feet.position, toMoveNode.transform.position) < 0.3f)
        {
            currentNode = toMoveNode;
            //RPC_SetCurrentNode(currentNode.Object.Id);
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
        ShowDice();
    }

    // --- Hàm kết thúc lượt ---
    public void EndTurn()
    {
        
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
            //arrow.Setup(this, i);
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

    #region RPC
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


    public void SetCurrentNode()
    {
   
    }
    #endregion

}
