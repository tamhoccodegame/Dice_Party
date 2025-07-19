using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardCar : MonoBehaviour
{
    private CharacterController controller;

    public BoardNode currentNode;
    public BoardNode toMoveNode;

    public int currentPlayerInputIndex = 0;
    private PlayerInput currentPlayerInput;
    public List<PlayerInput> inputs;

    public int stepLeft = 0;

    public Transform[] playerSitPositions;

    [Header("Players Animator")]
    public List<Animator> animators;

    [Header("Car Animator")]
    public Animator carAnim;

    public GameObject arrowPrefab;
    public List<GameObject> spawnedArrows;

    public bool isWaitingForChoice = false;
    Coroutine moveCoroutine = null;

    public StepText stepTextPrefab;
    public GameObject dice;
    public ParticleSystem diceVFX;

    // Start is called before the first frame update
    void Start()
    {
        inputs = PlayerManager.instance.players;
        UpdatePlayerInput();
        currentNode = PlayerSpawner.instance.spawnPosition[0].GetComponent<BoardNode>();
        toMoveNode = currentNode.nextNodes[0];
        controller = GetComponent<CharacterController>();

        FindFirstObjectByType<CinemachineCamera>().Follow = transform;
        FindFirstObjectByType<CinemachineCamera>().LookAt = transform;

        WizardPartyData.instance.UpdateCarNode(currentNode);
    }

    void UpdatePlayerInput()
    {
        if(currentPlayerInput != null && currentPlayerInputIndex == 0)
        {
            Wizard.instance.SetCanMove(true);
        }

        if(currentPlayerInput != null) 
        currentPlayerInput.actions["Trigger"].started -= OnTrigger;

        currentPlayerInput = inputs[currentPlayerInputIndex];
        currentPlayerInput.actions["Trigger"].started += OnTrigger;
        dice.SetActive(true);
        moveCoroutine = null;
    }

    private void OnDisable()
    {
        foreach(var playerInput in inputs)
        {
            playerInput.actions["Trigger"].started -= OnTrigger;
            playerInput.actions["PrimaryButton"].started -= ChoosePrimaryDirection;
            playerInput.actions["SecondaryButton"].started -= ChooseSecondaryDirection;
        }
    }

    private void OnTrigger(InputAction.CallbackContext obj)
    {
        if (moveCoroutine != null) return;

        stepLeft = 5;
        if(currentNode.nextNodes.Count > 1)
        {
            ShowDirection();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(RollDice());
        }
    }

    private void Update()
    {
        if (toMoveNode != null)
        {
            Vector3 direction = toMoveNode.transform.position - transform.position;
            direction.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(direction);
            if(Quaternion.Angle(transform.rotation, newRotation) > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 5 *  Time.deltaTime);
        }
    }

    private void SetCurrentNode(BoardNode node)
    {
        currentNode = node;
        toMoveNode = currentNode.nextNodes[0];
    }

    IEnumerator RollDice()
    {
        carAnim.CrossFade("RollDice", 0.25f);
        yield return new WaitForSeconds(0.2f);
        dice.SetActive(false);
        diceVFX.Play();
        var step = Instantiate(stepTextPrefab, dice.transform.position - new Vector3(0, 1.5f, 0), Quaternion.identity)
                   .GetComponent<StepText>();
        step.Init(stepLeft.ToString());
        yield return new WaitForSeconds(1f);
        moveCoroutine = StartCoroutine(MoveToNextNode());
    }

    IEnumerator MoveToNextNode()
    {
        carAnim.CrossFade("StartMove", 0.25f);
        yield return new WaitForSeconds(0.5f);
        carAnim.CrossFade("Moving", 0.25f);

        while (stepLeft > 0)
        {
            while(isWaitingForChoice) yield return null;

            while(Vector3.Distance(transform.position, toMoveNode.transform.position) > 0.4f)
            {
                Vector3 moveDirection = (toMoveNode.transform.position - transform.position).normalized;
                controller.Move(moveDirection * 10f * Time.deltaTime);
                yield return null;
            }

            stepLeft--;
             
            currentNode = toMoveNode;

            if(currentNode.nextNodes.Count > 1)
            {
                carAnim.CrossFade("EndMove", 0.25f);
                yield return new WaitForSeconds(0.5f);
                carAnim.CrossFade("Idle", 0.25f);
                isWaitingForChoice = true;
                ShowDirection();
                yield break;
            }
            else
            {
                toMoveNode = currentNode.nextNodes[0];
            }

                yield return null;
        }
        yield return null;
        currentPlayerInputIndex = (currentPlayerInputIndex + 1) % inputs.Count;
        UpdatePlayerInput();
        dice.SetActive(false);
        WizardPartyData.instance.UpdateCarNode(currentNode);
    }

    void ClearArrow()
    {
        foreach(var arrow in spawnedArrows)
        {
            Destroy(arrow);
        }
        spawnedArrows.Clear();
    }

    void ShowDirection()
    {
        ClearArrow();
        for(int i = 0; i < currentNode.nextNodes.Count; i++)
        {
            Vector3 midPoint = (currentNode.nextNodes[i].transform.position + transform.position) / 2f;
            Vector3 spawnPosition = new Vector3(midPoint.x, arrowPrefab.transform.position.y, midPoint.z);

            ArrowPointer arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity).GetComponent<ArrowPointer>();
            arrow.transform.rotation = Quaternion.LookRotation((currentNode.nextNodes[i].transform.position - currentNode.transform.position), Vector3.up);
            arrow.Setup(this, i);

            spawnedArrows.Add(arrow.gameObject);
        }
        currentPlayerInput.actions["PrimaryButton"].started += ChoosePrimaryDirection;
        currentPlayerInput.actions["SecondaryButton"].started += ChooseSecondaryDirection;
    }

    private void ChooseSecondaryDirection(InputAction.CallbackContext obj)
    {
        currentPlayerInput.actions["SecondaryButton"].started -= ChooseSecondaryDirection;
        ChooseDirection(1);
    }

    private void ChoosePrimaryDirection(InputAction.CallbackContext obj)
    {
        currentPlayerInput.actions["PrimaryButton"].started -= ChoosePrimaryDirection;
        ChooseDirection(0);
    }

    public void ChooseDirection(int index)
    {
        ClearArrow();
        toMoveNode = currentNode.nextNodes[index];
        Wizard.instance.AddPlayerChoseNode(toMoveNode);
        isWaitingForChoice = false;
        StopAllCoroutines();
        moveCoroutine = StartCoroutine(MoveToNextNode());
    }
}
