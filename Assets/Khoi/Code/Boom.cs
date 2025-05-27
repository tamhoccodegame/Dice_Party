using UnityEngine;

public class Boom : MonoBehaviour
{
    // public GameObject explosionEffect;
    public int damage = 50;
    public float minDelay = 3f;
    public float maxDelay = 5f;

    [HideInInspector]
    public GameObject owner; // Player tạo bomb này

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Gọi phát nổ sau khoảng thời gian ngẫu nhiên
        float delay = Random.Range(minDelay, maxDelay);
        Invoke("Explode", delay);
    }

    void Explode()
    {
        if (animator != null)
        {
            animator.SetTrigger("Explode");
        }

        // Gây sát thương cho các đối tượng trong vùng lân cận
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.5f); // Phạm vi nổ

        foreach (Collider2D col in hitColliders)
        {
            if (col.CompareTag("Player"))
            {
                GameObject target = col.gameObject;

                // ✅ Nếu không phải chủ nhân boom mới gây sát thương
                if (target != owner)
                {
                    Player2D player = target.GetComponent<Player2D>();
                    if (player != null)
                    {
                        player.TakeDamage(damage);
                    }
                }
            }
        }

        // Hủy boom sau 1 giây (cho animation nổ xong)
        Destroy(gameObject, 1f);
    }

    // Optional: vẽ vùng nổ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
