using UnityEngine;

public class KeyPickupMover : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 15f;
    public System.Action onArrive;

    private bool isMoving = false;
    private Rigidbody rb;
    private float delayBeforeMove = 3f; // Đợi nửa giây sau khi spawn rồi mới bay về player
    private float timer = 0f;

    Collider col;

    public void Init(Transform targetTransform, System.Action onDone)
    {
        col = GetComponentInChildren<Collider>();
        target = targetTransform;
        onArrive = onDone;
        isMoving = false;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Ban đầu bị vật lý tác động
        }

        timer = delayBeforeMove;
    }

    void Update()
    {
        if (target == null) return;

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                StartMoveToPlayer();
            }
            return;
        }

        if (!isMoving) return;

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 newPos = transform.position + direction * moveSpeed * Time.deltaTime;

        if (rb != null)
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            transform.position = newPos;
        }

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            isMoving = false;
            onArrive?.Invoke();
            Destroy(gameObject);
        }
    }

    void StartMoveToPlayer()
    {
        isMoving = true;
        col.enabled = false;
    }
}
