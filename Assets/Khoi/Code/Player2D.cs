using UnityEngine;

public class Player2D : MonoBehaviour
{
    public float moveSpeed = 5f;         // Tốc độ di chuyển trái/phải
    public float jumpForce = 7f;         // Lực nhảy
    public int maxHealth = 100;          // Máu tối đa
    private int currentHealth;           // Máu hiện tại

    private Rigidbody2D rb;              // Thành phần vật lý
    private Animator animator;           // Thành phần animation
    private bool isGrounded;             // Đang đứng trên mặt đất?
    private bool facingRight = true;     // Hướng nhân vật
    private bool isDead = false;         // Đã chết?

    public GameObject boomPrefab;        // Prefab quả bom
    public Transform boomSpawnPoint;     // Vị trí đặt bom

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;            // Không xoay nhân vật
        currentHealth = maxHealth;           // Gán máu ban đầu
    }

    void Update()
    {
        if (isDead) return;

        HandleMovement();        // Di chuyển ngang
        HandleJump();            // Nhảy
        HandleIdle();            // Đứng yên
        HandleDropBomb();        // Thả bomb
        ApplyExtraGravity();     // Tăng lực hút khi rơi
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("IsRun", Mathf.Abs(moveInput));

        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }
    }

    private void HandleJump()
    {
        // Chỉ cho phép nhảy khi đang đứng trên mặt đất
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("IsJumping", true);
            isGrounded = false; // Không còn trên mặt đất nữa
        }
    }

    private void HandleIdle()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && isGrounded)
        {
            animator.SetFloat("IsRun", 0);
            animator.SetBool("IsJumping", false);
        }
    }

    private void HandleDropBomb()
    {
        if (Input.GetKeyDown(KeyCode.F) && boomPrefab != null && boomSpawnPoint != null)
        {
            GameObject newBoom = Instantiate(boomPrefab, transform.position, Quaternion.identity);
            Boom boomScript = newBoom.GetComponent<Boom>();
            if (boomScript != null)
            {
                boomScript.owner = this.gameObject; // Gán người thả bom
            }
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;                            // Khi tiếp đất → cho nhảy lại
            animator.SetBool("IsJumping", false);         // Tắt trạng thái nhảy
        }
    }

    /// <summary>
    /// ➕ Lực hút xuống để không bị bay bổng khi nhảy
    /// </summary>
    private void ApplyExtraGravity()
    {
        if (rb.velocity.y < 0) // Đang rơi
        {
            // Tăng lực rơi để không bị bay lơ lửng
            rb.velocity += Vector2.up * Physics2D.gravity.y * 2f * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // Đang nhảy lên nhưng đã nhả nút → giảm bay
            rb.velocity += Vector2.up * Physics2D.gravity.y * 1.5f * Time.deltaTime;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("Player mất máu: " + amount + " | Còn lại: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("IsDead");     // Bật animation chết
        rb.velocity = Vector2.zero;        // Dừng di chuyển
        rb.isKinematic = true;             // Tắt vật lý
        GetComponent<Collider2D>().enabled = false; // Tùy chọn: tắt va chạm
    }
}
