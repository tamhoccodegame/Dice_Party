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
    public float waterLevel = 2.5f; // Điều chỉnh cho đúng với mực nước trong game
    public float playerHeight = 2f; // Chiều cao của nhân vật

    private Vector3 roamPoint;
    private Transform target;
    private bool isAttacking = false;
    private bool playerInWater = false;
    private float lastTurnTime;
    private Quaternion targetRotation;
    private Animator sharkAnimator;
    private Terrain terrain;

    void Start()
    {
        terrain = Terrain.activeTerrain;
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
            bool wasInWater = playerInWater;
            playerInWater = IsPlayerInWater();

            if (playerInWater && distanceToTarget <= detectionRange)
            {
                if (!wasInWater) // Chuyển ngay lập tức sang Attack khi vừa rơi xuống nước
                {
                    isAttacking = false; // Reset trạng thái
                }
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
        target = player != null ? player.transform : null;
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
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, attackSpeed * Time.deltaTime);
        transform.LookAt(targetPosition);
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
    }

    void PickNewRoamPoint()
    {
        if (terrain == null) return;

        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPos = terrain.transform.position;
        float minX = terrainPos.x;
        float maxX = terrainPos.x + terrainSize.x;
        float minZ = terrainPos.z;
        float maxZ = terrainPos.z + terrainSize.z;

        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection.y = 0;
            Vector3 candidatePoint = transform.position + randomDirection;

            if (candidatePoint.x >= minX && candidatePoint.x <= maxX &&
                candidatePoint.z >= minZ && candidatePoint.z <= maxZ)
            {
                roamPoint = candidatePoint;
                return;
            }
        }
        roamPoint = transform.position;
    }

    bool IsPlayerInWater()
    {
        if (target == null) return false;
        float playerMiddleY = target.position.y - (playerHeight / 2);
        return playerMiddleY < waterLevel;
    }
    void OnCollisionEnter(Collision collision)
    {
        // Nếu va chạm với đối tượng có tag là "Ground"
        if (collision.gameObject.CompareTag("Ground"))
        {
            PickNewRoamPoint(); // Chọn hướng đi mới
            lastTurnTime = Time.time; // Reset thời gian quay đầu
        }
    }

}
