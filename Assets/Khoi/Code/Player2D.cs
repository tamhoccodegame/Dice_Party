using UnityEngine;

public class Player2D : MonoBehaviour
{
    public float moveSpeed = 5f;            // Tốc độ di chuyển trái/phải
    public float jumpForce = 7f;            // Lực nhảy khi nhấn Space
    public float fallMultiplier = 2.5f;     // Hệ số rơi nhanh khi nhấn nhảy và đang rơi
    public float lowJumpMultiplier = 2f;    // Hệ số rơi nhẹ hơn khi nhả nút Space sớm

    private Rigidbody2D rb;                 // Thành phần vật lý
    private Animator animator;              // Thành phần animator
    private bool isGrounded = false;        // Kiểm tra có đang chạm đất không
    private bool facingRight = true;        // Hướng mặt hiện tại (true = phải)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();       // Gán Rigidbody2D
        animator = GetComponent<Animator>();    // Gán Animator
        rb.freezeRotation = true;               // Ngăn Rigidbody2D xoay vòng
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal"); // Nhận input trái/phải

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); // Cập nhật vận tốc theo trục X

        animator.SetFloat("IsRun", Mathf.Abs(moveInput)); // Cập nhật trạng thái chạy

        // Lật mặt nhân vật nếu đổi hướng
        if (moveInput > 0 && !facingRight)
        {
            Flip(); // Quay mặt sang phải
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip(); // Quay mặt sang trái
        }

        // Nhảy khi nhấn Space và đang đứng đất
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Nhảy lên
            animator.SetBool("IsJumping", true);                 // Bật animation Jump
            isGrounded = false;                                  // Đánh dấu đang trên không
        }

        // Tăng tốc rơi nếu đang rơi xuống hoặc nhả nút Space
        if (rb.velocity.y < 0)
        {
            // Nếu đang rơi xuống thì tăng tốc rơi mạnh hơn (hút đất)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // Nếu đang đi lên mà nhả Space thì giảm độ cao, không "bay"
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Nếu không di chuyển và đang chạm đất → đứng yên (Idle)
        if (moveInput == 0 && isGrounded)
        {
            animator.SetFloat("IsRun", 0);             // Ngưng chạy
            animator.SetBool("IsJumping", false);      // Ngưng nhảy → Idle
        }
    }

    // Lật mặt nhân vật bằng cách đảo chiều scale X
    private void Flip()
    {
        facingRight = !facingRight; // Đảo hướng mặt
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Kiểm tra va chạm với mặt đất → cho phép nhảy lại
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;                        // Đã chạm đất
            animator.SetBool("IsJumping", false);     // Ngắt animation Jump
        }
    }

    // Khi rời khỏi mặt đất thì không được nhảy nữa
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // Rời khỏi mặt đất → không cho nhảy tiếp
        }
    }
}
