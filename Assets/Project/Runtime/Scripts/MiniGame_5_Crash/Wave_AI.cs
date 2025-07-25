using DG.Tweening;
using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Di chuyển theo spline riêng (Dùng Dreamteck.Splines).
//Kiểm tra có va chạm với Enemy khác không (sắp giao nhau).
//Điều chỉnh tốc độ nếu cần.

public enum EnemyDirection
{
    Horizontal,
    Vertical
}

public class Wave_AI : MonoBehaviour
{
    public EnemyDirection direction;
    public SplineFollower follower;
    [HideInInspector] public float baseSpeed;

    [Range(0f, 1f)]
    public float stopPercent = 0.2f;   // Dừng tại 20% đường spline, chỉnh được trong Inspector

    [Header("Start Delay Config")]
    public Vector2 startDelayRange = new Vector2(0f, 0.5f); // Khoảng random delay lúc spawn

    private Action<GameObject> onFinish;
    private bool isMoving = false;
    private bool hasStopped = false;   // Đảm bảo chỉ dừng 1 lần
    private bool isPaused = false;

    public float minDelayTime;
    public float maxDelayTime;

    public void StartMoving(float speed, Action<GameObject> onFinishCallback)
    {
        baseSpeed = speed;
        onFinish = onFinishCallback;
        follower.SetPercent(0);
        follower.follow = false;
        isMoving = false;
        hasStopped = false;
        isPaused = false;


        // Random delay từ range
        float delay = UnityEngine.Random.Range(startDelayRange.x, startDelayRange.y);
        Invoke(nameof(BeginMove), delay);
    }

    void BeginMove()
    {
        follower.followSpeed = baseSpeed;
        follower.follow = true;
        isMoving = true;
        Debug.Log($"[Wave_AI] {name} START moving after delay {Time.time} → Speed {baseSpeed}");
    }

    void Update()
    {
        if (!isMoving || isPaused) return;

        float percent = (float)follower.GetPercent();


        // Dừng 1 lần ở vị trí stopPercent
        if (!hasStopped && percent >= stopPercent)
        {
            hasStopped = true;
            StartCoroutine(PauseAtPoint());
        }

        // Check hoàn thành spline
        if (percent >= 1f)
        {
            isMoving = false;
            follower.follow = false;
            onFinish?.Invoke(gameObject);
            Debug.Log($"[Wave_AI] {name} FINISHED spline.");
        }
    }

    IEnumerator PauseAtPoint()
    {
        isPaused = true;
        float pauseTime = UnityEngine.Random.Range(minDelayTime, maxDelayTime);

        Debug.Log($"[Wave_AI] {name} Pausing at {stopPercent * 100}% for {pauseTime} seconds");

        // Dừng di chuyển
        follower.follow = false;

        yield return new WaitForSeconds(pauseTime);

        // Tiếp tục
        follower.follow = true;
        isPaused = false;

        Debug.Log($"[Wave_AI] {name} Resumed movement at {Time.time}");
    }




    //public EnemyDirection direction;
    //public SplineFollower follower;
    //private System.Action<GameObject> onFinish;
    //private bool isMoving = false;
    //private float baseSpeed;

    //public void StartMoving(float speed, System.Action<GameObject> onFinishCallback)
    //{
    //    baseSpeed = speed;
    //    onFinish = onFinishCallback;
    //    follower.Restart(0.0);
    //    follower.followSpeed = speed;
    //    follower.follow = true;
    //    isMoving = true;
    //}

    //void Update()
    //{
    //    if (!isMoving) return;

    //    // Raycast check 4 hướng để tránh đụng kẻo xuyên nhau
    //    float adjust = 1f;
    //    Vector3 dir = transform.forward;
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, dir, out hit, 1f))
    //        adjust = 0.5f; // nếu có enemy phía trước, giảm tốc
    //    follower.followSpeed = baseSpeed * adjust;

    //    // Kiểm tra kết thúc spline
    //    if (follower.GetPercent() >= 1f)
    //    {
    //        isMoving = false;
    //        follower.follow = false;
    //        onFinish?.Invoke(gameObject);
    //    }
    //}

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1f);
    //}
}
