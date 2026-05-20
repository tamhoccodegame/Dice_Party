using UnityEngine;

public class CrowdAgent : MonoBehaviour
{
    public Transform player;

    public float avoidRadius = 2f;
    public float avoidForce = 40f;
    public float returnSpeed = 20f;

    Vector3 originalPos;
    Vector3 velocity;

    void Start()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < avoidRadius)
        {
            // Né player
            Vector3 away = (transform.position - player.position).normalized;

            float force = 1 - (dist / avoidRadius);

            velocity += away * force * avoidForce * Time.deltaTime;
        }
        else
        {
            // Quay về vị trí cũ
            Vector3 returnDir = (originalPos - transform.position);

            velocity += returnDir * returnSpeed * Time.deltaTime;
        }

        transform.position += velocity * Time.deltaTime;

        // làm mượt
        velocity *= 0.9f;
    }
}