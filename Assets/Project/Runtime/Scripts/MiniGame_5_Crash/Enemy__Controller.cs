using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy__Controller : MonoBehaviour
{
    public Action<Enemy__Controller> OnDespawn;

    private Enemy__Pool pool;
    private Vector3 moveDir;
    private float baseSpeed;
    private float currentSpeed;
    private bool active = false;

    private Vector3 gridCenter;
    private float despawnDistance = 30f;

    // Traffic
    [Header("Traffic Config")]
    public float minSpeed = 0.5f;       // tốc độ thấp nhất khi chậm
    public float lerpSmooth = 5f;       // tốc độ mượt
    public float detectRange = 2f;      // khoảng detect trước mặt
    public LayerMask enemyMask;

    // Priority
    public bool isHorizontal;           // ngang hay dọc
    private static bool horizontalPriority = true;  // mặc định ngang ưu tiên
    private static float priorityTimer = 0f;
    private const float PRIORITY_SWITCH_TIME = 1.0f;

    public void SetPool(Enemy__Pool poolRef) => pool = poolRef;

    public void Init(Vector3 dir, float moveSpeed, Vector3 center, float distanceLimit = 30f)
    {
        moveDir = dir.normalized;
        baseSpeed = moveSpeed;
        currentSpeed = moveSpeed;
        active = true;

        gridCenter = center;
        despawnDistance = distanceLimit;
    }

    void Update()
    {
        if (!active) return;

        UpdatePriority();
        //HandleTraffic();

        // Move
        transform.position += moveDir * currentSpeed * Time.deltaTime;

        // Despawn check
        if (Vector3.Distance(transform.position, gridCenter) > despawnDistance)
        {
            Despawn();
        }
    }

    void UpdatePriority()
    {
        // Mỗi 1 giây đổi ưu tiên ngang/dọc để không bị kẹt lâu
        priorityTimer += Time.deltaTime;
        if (priorityTimer > PRIORITY_SWITCH_TIME)
        {
            priorityTimer = 0f;
            horizontalPriority = !horizontalPriority;
        }
    }

    //void HandleTraffic()
    //{
    //    Vector3 pos = transform.position + Vector3.up * 0.5f; // nâng ray lên
    //    Vector3[] directions =
    //    {
    //    moveDir,
    //    -moveDir,
    //    new Vector3(moveDir.z, 0, -moveDir.x),  // phải
    //    new Vector3(-moveDir.z, 0, moveDir.x)   // trái
    //};

    //    bool shouldSlow = false;

    //    foreach (var dir in directions)
    //    {
    //        if (Physics.Raycast(pos, dir, out RaycastHit hit, detectRange, enemyMask))
    //        {
    //            if (hit.collider.GetComponent<Enemy__Controller>() != null)
    //                shouldSlow = true;

    //            Debug.DrawRay(pos, dir * detectRange, Color.red);
    //        }
    //        else
    //        {
    //            Debug.DrawRay(pos, dir * detectRange, Color.green);
    //        }
    //    }

    //    float targetSpeed = shouldSlow ? minSpeed : baseSpeed;
    //    currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * lerpSmooth);
    //}



    void Despawn()
    {
        active = false;
        OnDespawn?.Invoke(this);
        pool.Return(this);
    }

    public void Deactivate()
    {
        active = false;
        gameObject.SetActive(false);
        OnDespawn = null;
    }

    void FixedUpdate()
    {
        if (!active) return;

        HandleTraffic(); // Check & điều chỉnh tốc độ

        // Debug tốc độ hiện tại
        Debug.Log($"[{name}] Speed: {currentSpeed:F2}");

        // Di chuyển
        transform.position += moveDir * currentSpeed * Time.fixedDeltaTime;

        // Check despawn
        if (Vector3.Distance(transform.position, gridCenter) > despawnDistance)
        {
            Debug.Log($"[{name}] Despawn (ra khỏi phạm vi)");
            Despawn();
        }
    }

    void HandleTraffic()
    {
        if (enemyMask == 0)
        {
            Debug.LogWarning($"[{name}] enemyMask chưa được set! Raycast sẽ không detect gì cả!");
        }

        Vector3 pos = transform.position + Vector3.up * 0.5f;
        Vector3[] directions =
        {
        moveDir,                                       // Trước
        -moveDir,                                      // Sau
        new Vector3(moveDir.z, 0, -moveDir.x),         // Phải
        new Vector3(-moveDir.z, 0, moveDir.x)          // Trái
    };

        bool shouldSlow = false;

        foreach (var dir in directions)
        {
            if (Physics.Raycast(pos, dir, out RaycastHit hit, detectRange, enemyMask))
            {
                var enemy = hit.collider.GetComponent<Enemy__Controller>();
                if (enemy != null && enemy != this)
                {
                    shouldSlow = true;
                    Debug.Log($"[{name}] Ray hit {enemy.name} hướng {dir}, giảm tốc!");
                }

                Debug.DrawRay(pos, dir * detectRange, Color.red);
            }
            else
            {
                Debug.DrawRay(pos, dir * detectRange, Color.green);
            }
        }

        float targetSpeed = shouldSlow ? minSpeed : baseSpeed;

        // Log khi đổi targetSpeed
        if (Mathf.Abs(currentSpeed - targetSpeed) > 0.01f)
        {
            Debug.Log($"[{name}] Target Speed đổi từ {currentSpeed:F2} -> {targetSpeed:F2}");
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * lerpSmooth);
    }




    //public Action<Enemy__Controller> OnDespawn;

    //private Enemy__Pool pool;
    //private Vector3 moveDir;
    //private float speed;
    //private bool active = false;

    //private Vector3 gridCenter;
    //private float despawnDistance = 30f;

    //public void SetPool(Enemy__Pool poolRef)
    //{
    //    pool = poolRef;
    //}

    //public void Init(Vector3 dir, float moveSpeed, Vector3 center, float distanceLimit = 30f)
    //{
    //    moveDir = dir.normalized;
    //    speed = moveSpeed;
    //    active = true;

    //    gridCenter = center;
    //    despawnDistance = distanceLimit;

    //    OnDespawn = null; // Clear callback cũ
    //}

    //void Update()
    //{
    //    if (!active) return;

    //    // Di chuyển thẳng
    //    transform.position += moveDir * speed * Time.deltaTime;

    //    // Check nếu ra khỏi phạm vi grid
    //    if (Vector3.Distance(transform.position, gridCenter) > despawnDistance)
    //    {
    //        Despawn();
    //    }
    //}

    //void Despawn()
    //{
    //    active = false;
    //    OnDespawn?.Invoke(this);
    //    pool.Return(this);
    //}

    //public void Deactivate()
    //{
    //    active = false;
    //    gameObject.SetActive(false);
    //}
}
