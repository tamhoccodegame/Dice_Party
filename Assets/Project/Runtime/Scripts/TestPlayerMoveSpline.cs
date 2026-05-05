using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class TestPlayerMoveSpline : MonoBehaviour
{
    public SplineAnimate splineAnimate;

    public TextMeshProUGUI fpsText;

    public int stepLeft;

    public BoardNode currentTriggerNode;
    public List<BoardNode> juctionChoices;

    public bool isInJuction = false;
    public bool isReachedNode = false;

    public string currentAnimation = null;

    // Start is called before the first frame update
    void Awake()
    {

    }

    private void Start()
    {
        CameraFollow.instance.StartFollowTarget(transform);
    }

    // Update is called once per frame
    void Update()
    {
        fpsText.text = (1f / Time.smoothDeltaTime).ToString("00");
        if ((stepLeft <= 0 || isInJuction) && splineAnimate.IsPlaying)
        {
            splineAnimate.Pause();
        }
        else if (stepLeft > 0 && !splineAnimate.IsPlaying && !isInJuction)
        {
            splineAnimate.Play();
        }

        Animator animator = GetComponent<Animator>();
        if(!splineAnimate.enabled || !splineAnimate.IsPlaying)
        {
            if (currentAnimation == "Idle") return;
            currentAnimation = "Idle";
            animator.CrossFade("Idle", 0.25f);
        }
        else if(splineAnimate.enabled || splineAnimate.IsPlaying)
        {
            if (currentAnimation == "Run") return;
            currentAnimation = "Run";
            animator.CrossFade("Run", 0.25f);
        }
    }

    IEnumerator InJuctionProcess()
    {
        CameraFollow.instance.SwitchCamera(CameraFollow.CameraState.Juction);
        bool isChooseDirection = false;
        while (isInJuction)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //splineAnimate chuyển qua spline 1
                splineAnimate.Container = juctionChoices[0].splineContainer;
                isChooseDirection = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //splineAnimate chuyển qua spline 2
                splineAnimate.enabled = false;
                yield return StartCoroutine(LookAt(juctionChoices[1]));
                isChooseDirection = true;
            }

            if (isChooseDirection)
            {
                isInJuction = false;
                CameraFollow.instance.SwitchCamera(CameraFollow.CameraState.Default);
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BoardNode>(out var node))
        {
            currentTriggerNode = node;
            Debug.Log(currentTriggerNode.nextNodes.Count);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BoardNode>(out var node))
        {
            if (node == currentTriggerNode)
            {
                isReachedNode = false;
                currentTriggerNode = null;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentTriggerNode == null) return;
        if (Vector3.Distance(transform.position, currentTriggerNode.transform.position) < 0.2f && !isReachedNode)
        {
            stepLeft--;
            isReachedNode = true;

            if(currentTriggerNode.nextNodes.Count > 1)
            {
                Debug.Log("Juction Baby");
                isInJuction = true;
                juctionChoices = currentTriggerNode.nextNodes;
                StartCoroutine(InJuctionProcess());
            }
            else if (currentTriggerNode.splineContainer != splineAnimate.Container)
            {
                Debug.Log("Juction Baby 2");
                splineAnimate.enabled = false;  
                StartCoroutine(LookAtNextNode(currentTriggerNode));
            }
        }
    }

    IEnumerator LookAt(BoardNode node)
    {
        Quaternion newRotation = Quaternion.LookRotation(node.transform.position - this.transform.position); 

        float speed = 10f;

        while (Quaternion.Angle(this.transform.rotation, newRotation) > 0.1f)
        {
            this.transform.rotation = Quaternion.Slerp(
                this.transform.rotation,
                newRotation,
                speed * Time.deltaTime
            );
            yield return null;
        }

        isReachedNode = false;

        splineAnimate.enabled = true;

        splineAnimate.Container = node.splineContainer;
        splineAnimate.NormalizedTime = node.normalizeTime;
        splineAnimate.Update();
        currentTriggerNode = null;
    }

    IEnumerator LookAtNextNode(BoardNode node)
    {
        Quaternion newRotation = Quaternion.LookRotation(node.nextNodes[0].transform.position - this.transform.position); 

        float speed = 30f;

        while (Quaternion.Angle(transform.rotation, newRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                newRotation,
                speed * Time.deltaTime
            );
        }

        // SNAP luôn (quan trọng)
        transform.rotation = newRotation;

        isReachedNode = false;

        splineAnimate.enabled = true;

        splineAnimate.Container = node.splineContainer;
        splineAnimate.NormalizedTime = node.normalizeTime;
        splineAnimate.Update();
        currentTriggerNode = null;
        yield return null;
    }
}
