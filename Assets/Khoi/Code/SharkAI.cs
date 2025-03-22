using UnityEngine;

public class SharkAI : MonoBehaviour
{
    public float normalSpeed = 2f;
    public float attackSpeed = 6f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float roamRadius = 15f;
    public float turnDelay = 5f;
    public float turnSpeed = 2f;

    private Vector3 roamPoint;
    private Transform target;
    private bool isAttacking = false;
    private float lastTurnTime;
    private Quaternion targetRotation;
    private Animator sharkAnimator;

    void Start()
    {
        PickNewRoamPoint();
        lastTurnTime = Time.time;
        targetRotation = transform.rotation;
        sharkAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        FindTarget();

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= detectionRange)
            {
                AttackTarget();
            }
            else
            {
                StopAttack();
            }
        }
        else
        {
            StopAttack();
        }
    }

    void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            target = null;
            StopAttack();
        }
    }

    void AttackTarget()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            sharkAnimator.ResetTrigger("Move");
            sharkAnimator.SetTrigger("Attack");
        }

        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        transform.position += transform.forward * attackSpeed * Time.deltaTime;
    }

    void StopAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            sharkAnimator.ResetTrigger("Attack");
            sharkAnimator.SetTrigger("Move");
        }
        RoamFreely();
    }

    void RoamFreely()
    {
        if (Vector3.Distance(transform.position, roamPoint) < 1f)
        {
            PickNewRoamPoint();
            lastTurnTime = Time.time;
        }

        targetRotation = Quaternion.LookRotation(roamPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        transform.position += transform.forward * normalSpeed * Time.deltaTime;

        if (!isAttacking)
        {
            sharkAnimator.ResetTrigger("Attack");
            sharkAnimator.SetTrigger("Move");
        }
    }

    void PickNewRoamPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection.y = 0;
        roamPoint = transform.position + randomDirection;
    }
}
