using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Wizard : MonoBehaviour
{
    public AudioClip music;
    public static Wizard instance;

    private Animator animator;
    private CharacterController controller;

    public BoardNode currentNode;
    public BoardNode toMoveNode;

    public int stepLeft;
    private Queue<BoardNode> playerChoseNodeQueue = new Queue<BoardNode>();

    private bool canMove = false;

    public StepText stepTextPrefab;
    public GameObject dice;
    public ParticleSystem diceVFX;

    public BoardCar player;

    public Volume volume;
    private LensDistortion lens;
    private ChromaticAberration chroma;
    private Vignette vignette;


    public CinemachineCamera cam;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        dice.SetActive(false);

        volume.profile.TryGet(out lens);
        volume.profile.TryGet(out chroma);
        volume.profile.TryGet(out vignette);

        string savedNode = WizardPartyData.instance.wizardNode;
        if(!string.IsNullOrEmpty(savedNode))
        {
            currentNode = GameObject.Find(savedNode).GetComponent<BoardNode>();
        }
        transform.position = currentNode.transform.position;
        WizardPartyData.instance.UpdateWizardNode(currentNode);
    }

    // Update is called once per frame
    void Update()
    {
        if (toMoveNode != null)
        {
            Vector3 direction = toMoveNode.transform.position - transform.position;
            direction.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(direction);
            if (Quaternion.Angle(transform.rotation, newRotation) > 0.1f)
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 5 * Time.deltaTime);
        }
    }

    public void AddPlayerChoseNode(BoardNode choseNode)
    {
        playerChoseNodeQueue.Enqueue(choseNode);
    }

    [ContextMenu("Move")]
    public void MoveTest()
    {
        SetCanMove(true);
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
        if (canMove)
        {
            dice.SetActive(true);
            StartCoroutine(RollDice());
            //MusicManager.instance.PlayMusic(music);
        }
    }

    IEnumerator RollDice()
    {
        //CinecameraManager.instance.TriggerCamera(cam);
        yield return new WaitForSeconds(2.5f);
        animator.CrossFade("RollDice", 0.25f);
        yield return new WaitForSeconds(0.4f);
        diceVFX.Play();
        dice.SetActive(false);
        stepLeft = 20;
        var stepText = Instantiate(stepTextPrefab, dice.transform.position - new Vector3(0, 1.5f, 0), Quaternion.identity);
        stepText.Init(stepLeft.ToString());
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(MoveToNextNode());
        yield return new WaitForSeconds(0.5f);
        //CinecameraManager.instance.ResetCamera();
    }

    IEnumerator MoveToNextNode()
    {
        if(currentNode.nextNodes.Count > 1 && playerChoseNodeQueue.Count > 0)
        {
            toMoveNode = playerChoseNodeQueue.Dequeue();
        }
        else
        {
            toMoveNode = currentNode.nextNodes[0];
        }

        animator.CrossFade("Run", 0.25f);
        while(stepLeft > 0)
        {
            while(Vector3.Distance(transform.position, toMoveNode.transform.position) > 0.4f)
            {
                Vector3 direction = (toMoveNode.transform.position - transform.position).normalized;
                controller.Move(direction * 8f * Time.deltaTime);
                yield return null;
            }

            stepLeft--;

            currentNode = toMoveNode;
            WizardPartyData.instance.UpdateWizardNode(currentNode);

            if (currentNode.nextNodes.Count > 1 && playerChoseNodeQueue.Count > 0)
            {
                toMoveNode = playerChoseNodeQueue.Dequeue();
            }
            else
            {
                toMoveNode = currentNode.nextNodes[0];
            }

            if (toMoveNode == player.currentNode)
            {
                StartCoroutine(CastSpell()); //Chuyển vào minigame
                yield break;
            }

            yield return null;
        }
        yield return null;
        canMove = false;
        player.SetCanMove(true);
        animator.CrossFade("Idle", 0.25f);
    }

    IEnumerator CastSpell()
    {
        animator.CrossFade("Cast", 0.25f);
        yield return new WaitForSeconds(0.5f);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            lens.intensity.value = Mathf.Lerp(0f, -0.5f, t);
            chroma.intensity.value = Mathf.Lerp(0f, 1f, t);
            vignette.intensity.value = Mathf.Lerp(0f, 0.5f, t);

            yield return null;
        }

        SceneManager.LoadScene(WizardPartyData.instance.GetMinigame());
        //SceneManager.LoadScene("MNG3");
    }
}
