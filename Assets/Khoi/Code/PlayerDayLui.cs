using UnityEngine;

// Script điều khiển nhân vật có animation đánh theo nút: chuột trái (Attack01), E, R, T
// Và có di chuyển bằng Rigidbody

[RequireComponent(typeof(Rigidbody))]
public class PlayerDayLui : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Combat Settings")]
    public float attackRange = 2f;
    public float pushForce = 10f;
    public float attackCooldown = 0.5f;

    [Header("Animator")]
    public Animator animator;

    private Rigidbody rb;
    private float lastAttackTime = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("Animator chưa được gán vào PlayerDayLui!");
        }
    }

    void Update()
    {
        HandleMovement();
        HandleAttackInputs();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveDir.magnitude > 0.1f)
        {
            rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);

            // Gọi animation chạy (chỉ cần gõ đúng tên state trong Animator Controller)
            if (animator != null)
            {
                animator.Play("Run01FWD");
            }
        }
        else
        {
            // Đứng yên
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

            if (animator != null)
            {
                animator.Play("Idle01");
            }
        }
    }

    void HandleAttackInputs()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        // Chuột trái → Attack01
        if (Input.GetMouseButtonDown(0))
        {
            PlayAttack("Attack01");
        }

        // Phím E → Attack02
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayAttack("Attack02");
        }

        // Phím R → Attack03
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayAttack("Attack03");
        }

        // Phím T → Push
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayAttack("Push");
        }
    }

    void PlayAttack(string animName)
    {
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.Play(animName);
        }

        // Gây lực đẩy nếu đánh trúng player khác
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && hit.CompareTag("Player"))
            {
                PlayerDayLui other = hit.GetComponent<PlayerDayLui>();
                if (other != null)
                {
                    other.TakeHit(transform.position);
                }
            }
        }
    }

    public void TakeHit(Vector3 attackerPos)
    {
        Vector3 pushDir = (transform.position - attackerPos).normalized;
        rb.AddForce(pushDir * pushForce, ForceMode.Impulse);

        Debug.Log($"{gameObject.name} bị đẩy lùi!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
