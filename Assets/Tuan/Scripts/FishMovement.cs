using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public Transform spawnArea;       // Tham chiếu tới SpawnPosition (vùng bơi)
    [Header("Speed Settings")]
    public float minSpeed = 0.5f;
    public float maxSpeed = 2.0f;
    public float speedChangeRate = 0.5f; // càng cao thì đổi tốc độ càng nhanh

    [Header("Direction Change Settings")]
    public float minChangeTime = 2f;
    public float maxChangeTime = 5f;

    private float currentSpeed;
    private float targetSpeed;
    private float timer;
    private float changeTime;
    void Start()
    {
        PickNewDirection();
        PickNewSpeed();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Tự động đổi hướng theo thời gian
        if (timer >= changeTime)
        {
            PickNewDirection();
            timer = 0f;
        }

        // Tốc độ thay đổi dần đến targetSpeed
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);

        // Thi thoảng chọn lại tốc độ mục tiêu mới
        if (Random.value < 0.05f)
        {
            PickNewSpeed();
        }

        MoveForward();
    }


    void PickNewDirection()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            0f, // Giữ hướng Y = 0
            Random.Range(-1f, 1f)
        ).normalized;
        Quaternion newRotation = Quaternion.LookRotation(randomDirection);
        transform.rotation = newRotation;

        changeTime = Random.Range(minChangeTime, maxChangeTime);
    }
    void PickNewSpeed()
    {
        targetSpeed = Random.Range(minSpeed, maxSpeed);
    }
    void MoveForward()
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }
 
}
