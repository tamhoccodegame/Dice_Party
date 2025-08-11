using UnityEngine;

public class KeyPickupMover : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 15f;
    public System.Action onArrive;

    private bool isMoving = false;
    private Rigidbody rb;
    private float delayBeforeMove = 5f;
    private float timer = 0f;

    Collider col;

    private void Awake()
    {
        col = GetComponentInChildren<Collider>();
    }

    public void Init(Transform targetTransform, System.Action onDone)
    {
        
        target = targetTransform;
        onArrive = onDone;
        isMoving = false;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Ban đầu bị vật lý tác động
            rb.AddTorque(new Vector3(1, 50, 1), ForceMode.Impulse);
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

        if (rb != null)
        {
            rb.velocity = direction * moveSpeed;
        }

        if (Vector3.Distance(transform.position, target.position) < 0.8f)
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
