using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class Bullman : MonoBehaviour
{
    public Animator animator;
    public CharacterController controller;
    public Transform lockedTarget;

    public List<Transform> availabeTargets;

    public Transform centerPoint;

    public bool isHitWall = false;

    public ShakeData shakeData;

    public enum State
    {
        Idle,
        Run,
        MoveToCenter
    }

    public State state;

    // Start is called before the first frame update
    void Start()
    {
        availabeTargets.Clear();
        foreach(var p in WizardMiniGameManager.instance.playerObjects)
        {
            availabeTargets.Add(p.Value.transform);
        }

        state = State.Idle;
        StartCoroutine(StateMachine());
    }

    IEnumerator StateMachine()
    {
        while (!WizardMiniGameManager.instance.isGameStarted || WizardMiniGameManager.instance.isGameOver)
        {
            yield return null;
        }

        while (true)
        {
            switch (state)
            {
                case State.Idle:
                    yield return StartCoroutine(Idle());
                    break;
                case State.Run:
                    yield return StartCoroutine(Run());
                    break;
                case State.MoveToCenter:
                    yield return StartCoroutine(MoveToCenter());
                    break;
            }
            yield return null;
        }

    }

    IEnumerator Idle()
    {
        isHitWall = false;
        animator.CrossFade("Idle", 0.25f);

        lockedTarget = availabeTargets[Random.Range(0, availabeTargets.Count)];
        yield return null;

        float elaspedTime = 0;

        while (elaspedTime < 5)
        {
            Vector3 lookDir = (lockedTarget.position - transform.position).normalized;

            lookDir.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 10 * Time.deltaTime);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        state = State.Run;
    }

    IEnumerator Run()
    {
        isHitWall = false;
        animator.CrossFade("Run", 0.25f);

        while (!isHitWall)
        {
            controller.Move(transform.forward * 20 * Time.deltaTime);
            yield return null;
        }

        CameraShakerHandler.Shake(shakeData);

        animator.CrossFade("Idle", 0.25f);

        yield return new WaitForSeconds(2f);


        state = State.MoveToCenter;
    }

    IEnumerator MoveToCenter()
    {
        isHitWall = false;
        animator.CrossFade("Move", 0.25f);

        while (true)
        {
            // vector khoảng cách, bỏ Y
            Vector3 offset = centerPoint.position - transform.position;
            offset.y = 0;

            // nếu đã tới gần
            if (offset.sqrMagnitude <= 0.5f * 0.5f)
                break;

            Vector3 movDir = offset.normalized;

            controller.Move(movDir * 4 * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(movDir),
                10 * Time.deltaTime
            );

            yield return null;
        }

        state = State.Idle;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        isHitWall = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger with {other.name}");
        if(other.TryGetComponent<MNGPlayerController>(out var player))
        {
            player.GetComponent<PlayerBlinking>().OnHitByObstacle(other.ClosestPoint(transform.position));
        }
    }
}
