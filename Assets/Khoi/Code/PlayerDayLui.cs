using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDayLui : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float pushForce = 10f;
    public float attackCooldown = 0.8f;

    [Header("Animator")]
    public Animator animator;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private float lastAttackTime = -999f;
    private bool isAttacking = false;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator chưa được gán cho PlayerDayLui!");
    }

    void Update()
    {
        HandleMovement();
        HandleAttack();
        HandleJump();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A, D
        float v = Input.GetAxisRaw("Vertical");   // W, S
        Debug.Log($"Input H: {h}, V: {v}, MoveDir: {moveDirection}");

        moveDirection = new Vector3(h, 0f, v).normalized;

        if (!isAttacking) // Nếu không đang đánh thì mới cho di chuyển
        {
            if (moveDirection.magnitude >= 0.1f)
            {
                // Di chuyển
                Vector3 moveVelocity = moveDirection * moveSpeed;
                rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

                // Xoay theo hướng di chuyển
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

                animator.CrossFade("Run", 0.1f);
            }
            else
            {
                // Đứng yên
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
                animator.CrossFade("Idle", 0.1f);
            }
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false; // Chờ rơi xuống mới nhảy tiếp
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.CrossFade("Jump", 0.1f);
        }
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(1) && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            isAttacking = true;

            // Ngắt di chuyển, animation chuyển sang đánh
            animator.CrossFade("hit", 0.1f);

            Invoke(nameof(ResetAttack), 0.5f); // Tùy thời gian animation đánh
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    // Xử lý va chạm khi nhảy tiếp đất
    void OnCollisionEnter(Collision collision)
    {
        // Check tiếp đất
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }

        // Kiểm tra va chạm với Player khác khi đang đánh
        if (isAttacking && collision.gameObject.CompareTag("Player") && collision.gameObject != gameObject)
        {
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRb != null)
            {
                Vector3 pushDir = (collision.transform.position - transform.position).normalized;
                otherRb.AddForce(pushDir * pushForce, ForceMode.Impulse);
            }
        }

    }
}
