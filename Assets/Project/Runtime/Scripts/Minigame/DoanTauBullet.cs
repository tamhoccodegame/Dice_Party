using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoanTauBullet : MonoBehaviour
{
    private Rigidbody rb;
    public float initSpeed;
    public float maxSpeed;
    public float currentSpeed;

    private GachaGun gun;
    private Transform target;

    public float trackingDuration = 2.5f;
    private float trackingTimer = 0f;
    private bool isTracking = true;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(Transform _target, GachaGun _gun)
    {
        target = _target;
        gun = _gun;
        currentSpeed = initSpeed;
        Invoke(nameof(DestroyAfterDelay), 8f);
    }

    void Update()
    {
        if (target == null) return;

        currentSpeed += 2 * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        if (isTracking)
        {
            trackingTimer += Time.deltaTime;
            if (trackingTimer > trackingDuration)
            {
                isTracking = false;
            }

            Vector3 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * currentSpeed;
            Quaternion newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 10 * Time.deltaTime);
        }
        // Nếu không tracking thì giữ nguyên rb.velocity cũ
    }

    void DestroyAfterDelay()
    {
        gun.readyToChooseTarget = true;
        DoanTauManager.instance.StartAllFakeGacha();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerController>(out var player))
        {
            gun.readyToChooseTarget = true;
            DoanTauManager.instance.StartAllFakeGacha();
            Destroy(gameObject);
        }
    }
}
