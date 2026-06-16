//using System.Collections;
//using System.Collections.Generic;
//using Unity.Cinemachine;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class BoardCar : PlayerController    
//{
//    public AudioClip music;
//    public AudioSource startEngineSFX;

//    private CharacterController controller;

//    public BoardNode currentNode;
//    public BoardNode toMoveNode;

//    public int currentPlayerInputIndex = 0;
//    private PlayerInput currentPlayerInput;
//    public List<PlayerInput> inputs;

//    public int stepLeft = 0;

//    public Transform[] playerSitPositions;
//    public Transform drivePosition;

//    [Header("Players Animator")]
//    public Animator[] animators;

//    [Header("Car Animator")]
//    public Animator carAnim;

//    public GameObject arrowPrefab;
//    public List<GameObject> spawnedArrows;

//    public bool isWaitingForChoice = false;
//    Coroutine moveCoroutine = null;

//    public StepText stepTextPrefab;
//    public GameObject dice;
//    public ParticleSystem diceVFX;

//    public CinemachineCamera rollCam;
//    public CinemachineCamera closeCam;

//    public bool canMove = true;
//    private bool isTurnDone = false;

//    // Start is called before the first frame update
//    void Start()
//    {
//        inputs = PlayerManager.instance.players;
//        UpdatePlayerInput();

//        string savedNode = WizardPartyData.instance.carNode;
//        if (!string.IsNullOrEmpty(savedNode))
//        {
//            currentNode = GameObject.Find(savedNode).GetComponent<BoardNode>();
//        }
//        else
//        {
//            currentNode = PlayerSpawner.instance.spawnPosition[0].GetComponent<BoardNode>();
//        }

//        transform.position = currentNode.transform.position;
//        toMoveNode = currentNode.nextNodes[0];

//        drivePosition = playerSitPositions[0];


//        controller = GetComponent<CharacterController>();
//        Wizard.instance.player = this;

//        FindFirstObjectByType<CinemachineCamera>().Follow = transform;
//        FindFirstObjectByType<CinemachineCamera>().LookAt = transform;

//        animators = GetComponentsInChildren<Animator>();

//        WizardPartyData.instance.UpdateCarNode(currentNode);
//        canMove = true;
//    }

//    void UpdatePlayerInput()
//    {
//        if(isTurnDone)
//        {
//            StartCoroutine(DelaySetWizardCanMove());
//            return;
//        }

//        MusicManager.instance.PlayMusic(music);
//        StartCoroutine(ShowTurnAvatarUI());
//        CinecameraManager.instance.TriggerCamera(rollCam);

//        if (currentPlayerInput != null) 
//        currentPlayerInput.actions["Trigger"].started -= OnTrigger;

//        currentPlayerInput = inputs[currentPlayerInputIndex];
//        currentPlayerInput.actions["Trigger"].started += OnTrigger;
//        dice.SetActive(true);
//        moveCoroutine = null;
//    }

//    IEnumerator ShowTurnAvatarUI()
//    {
//        AvatarTurnManager.instance.gameObject.SetActive(true);
//        AvatarTurnManager.instance.HighlightTurn(currentPlayerInputIndex);
//        //Vector3 currentPlayerPosition = animators[currentPlayerInputIndex].transform.position;
//        //animators[currentPlayerInputIndex].transform.position = drivePosition.position;

//        //int prevDriverIndex = currentPlayerInputIndex - 1;
//        //if (prevDriverIndex < 0) prevDriverIndex = inputs.Count - 1;
//        //Transform prevDriver = animators[prevDriverIndex].transform;
//        //prevDriver.position = currentPlayerPosition;
//        yield return new WaitForSeconds(3f);
//        AvatarTurnManager.instance.gameObject.SetActive(false);
//    }

//    IEnumerator DelaySetWizardCanMove()
//    {
//        yield return new WaitForSeconds(2.5f);
//        canMove = false;
//        isTurnDone = false;
//        Wizard.instance.SetCanMove(true);
//    }

//    private void OnDisable()
//    {
//        foreach(var playerInput in inputs)
//        {
//            playerInput.actions["Trigger"].started -= OnTrigger;
//            playerInput.actions["PrimaryButton"].started -= ChoosePrimaryDirection;
//            playerInput.actions["SecondaryButton"].started -= ChooseSecondaryDirection;
//        }
//    }

//    private void OnTrigger(InputAction.CallbackContext obj)
//    {
//        if (moveCoroutine != null || !canMove) return;

//        stepLeft = 2;
//        if(currentNode.nextNodes.Count > 1)
//        {
//            ShowDirection();
//        }
//        else
//        {
//            StopAllCoroutines();
//            StartCoroutine(RollDice());
//        }
//    }

//    public void SetCanMove(bool canMove)
//    {
//        this.canMove = canMove;
//        if (canMove)
//        {
//            UpdatePlayerInput();
//        }
//    }

//    private void Update()
//    {
//        if (toMoveNode != null)
//        {
//            Vector3 direction = toMoveNode.transform.position - transform.position;
//            if (Mathf.Abs(transform.position.y - toMoveNode.transform.position.y) >= 3f)
//            {
//                direction.y = 0;
//            }
//            Quaternion newRotation = Quaternion.LookRotation(direction);
//            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 5 *  Time.deltaTime);
//        }
//    }

//    public void SetCurrentNode(BoardNode node)
//    {
//        currentNode = node;
//        if(currentNode != null)
//        toMoveNode = currentNode.nextNodes[0];
//        else 
//            toMoveNode = null;
//    }

//    public void UpdateCurrentNodeData(BoardNode node)
//    {
//        WizardPartyData.instance.UpdateCarNode(node);
//    }

//    public PlayerInput GetInput()
//    {
//        return inputs[currentPlayerInputIndex];
//    }
//    IEnumerator RollDice()
//    {
//        carAnim.CrossFade("RollDice", 0.25f);
//        yield return new WaitForSeconds(0.2f);
//        dice.SetActive(false);
//        diceVFX.Play();
//        var step = Instantiate(stepTextPrefab, dice.transform.position - new Vector3(0, 1.5f, 0), Quaternion.identity)
//                   .GetComponent<StepText>();
//        step.Init(stepLeft.ToString());
//        yield return new WaitForSeconds(1f);
//        moveCoroutine = StartCoroutine(MoveToNextNode());
//        yield return new WaitForSeconds(0.5f);
//    }

//    public void TryMove()
//    {
//        MusicManager.instance.PlayMusic(music);
//        if(stepLeft > 0)
//        {
//            if(currentNode.nextNodes.Count > 1)
//            {
//                ShowDirection();
//            }
//            else
//            {
//                StartCoroutine(MoveToNextNode());
//            }
//        }
//        else
//        {
//            carAnim.CrossFade("Idle", 0.25f);
//        }
//    }

//    IEnumerator MoveToNextNode()
//    {
//        CinecameraManager.instance.TriggerCamera(closeCam);
//        startEngineSFX.Play();
//        carAnim.CrossFade("StartMove", 0.25f);
//        yield return new WaitForSeconds(1f);
//        carAnim.CrossFade("Moving", 0.25f);

//        while (stepLeft > 0)
//        {
//            while(isWaitingForChoice) yield return null;

//            while(Vector3.Distance(transform.position, toMoveNode.transform.position) > 0.4f)
//            {
//                Vector3 moveDirection = (toMoveNode.transform.position - transform.position).normalized;
//                controller.Move(moveDirection * 10f * Time.deltaTime);
//                yield return null;
//            }

//            stepLeft--;
             
//            currentNode = toMoveNode;
//            UpdateCurrentNodeData(currentNode);

//            if (currentNode.nextNodes.Count > 1 && stepLeft > 0)
//            {
//                carAnim.CrossFade("EndMove", 0.25f);
//                yield return new WaitForSeconds(0.5f);
//                carAnim.CrossFade("Idle", 0.25f);
//                isWaitingForChoice = true;
//                ShowDirection();
//                yield break;
//            }
//            else
//            {
//                toMoveNode = currentNode.nextNodes[0];
//            }

//                yield return null;
//        }

//        yield return null;
//        currentPlayerInputIndex = (currentPlayerInputIndex + 1) % inputs.Count;
//        if(currentPlayerInputIndex == 0) isTurnDone = true;

//        yield return new WaitForSeconds(1f);
//        dice.SetActive(false);
//        //currentNode.ProcessNode(this);
//        WizardPartyData.instance.UpdateCarNode(currentNode);
//    }

//    void ClearArrow()
//    {
//        foreach(var arrow in spawnedArrows)
//        {
//            Destroy(arrow);
//        }
//        spawnedArrows.Clear();
//    }

//    void ShowDirection()
//    {
//        //CinecameraManager.instance.ResetCamera();
//        ClearArrow();
//        for(int i = 0; i < currentNode.nextNodes.Count; i++)
//        {
//            Vector3 midPoint = (currentNode.nextNodes[i].transform.position + transform.position) / 2f;
//            Vector3 spawnPosition = new Vector3(midPoint.x, arrowPrefab.transform.position.y, midPoint.z);

//            ArrowPointer arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity).GetComponent<ArrowPointer>();
//            arrow.transform.rotation = Quaternion.LookRotation((currentNode.nextNodes[i].transform.position - currentNode.transform.position), Vector3.up);
//            arrow.Setup(this, i);

//            spawnedArrows.Add(arrow.gameObject);
//        }
//        currentPlayerInput.actions["PrimaryButton"].started += ChoosePrimaryDirection;
//        currentPlayerInput.actions["SecondaryButton"].started += ChooseSecondaryDirection;
//    }

//    private void ChooseSecondaryDirection(InputAction.CallbackContext obj)
//    {
//        currentPlayerInput.actions["SecondaryButton"].started -= ChooseSecondaryDirection;
//        ChooseDirection(1);
//    }

//    private void ChoosePrimaryDirection(InputAction.CallbackContext obj)
//    {
//        currentPlayerInput.actions["PrimaryButton"].started -= ChoosePrimaryDirection;
//        ChooseDirection(0);
//    }

//    public void ChooseDirection(int index)
//    {
//        ClearArrow();
//        toMoveNode = currentNode.nextNodes[index];
//        Wizard.instance.AddPlayerChoseNode(toMoveNode);
//        isWaitingForChoice = false;
//        StopAllCoroutines();
//        moveCoroutine = StartCoroutine(MoveToNextNode());
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if(other.TryGetComponent<IMinigame>(out var minigame))
//        {
//            minigame.Init(this);
//        }
//    }

//    public override PlayerInput GetPlayerInput()
//    {
//        return currentPlayerInput;
//    }

//    public override void SetInput(PlayerInput input)
//    {
        
//    }
//}
