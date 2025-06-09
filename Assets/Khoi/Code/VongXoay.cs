using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class VongXoay : NetworkBehaviour
{
    [Header("Cài đặt vòng xoay")]
    public float startSpeed = 50f;
    public float acceleration = 5f;

    [Header("Holder xoay cánh")]
    public Transform canh1Holder;  // Quay thuận chiều kim đồng hồ
    public Transform canh2Holder;  // Quay ngược chiều kim đồng hồ
    public Transform canh3Holder;  // Quay thuận chiều kim đồng hồ
    public Transform canh4Holder;  // Quay ngược chiều kim đồng hồ

    VongXoayManager manager;

    private float currentSpeed;

    public override void Spawned()
    {
        currentSpeed = startSpeed;
        manager = VongXoayManager.instance;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return; // Chỉ để Host điều khiển

        if (manager != null && manager.Object.IsValid && manager.isGameStarted)
        {
            // Tăng tốc dần theo thời gian
            currentSpeed += acceleration * Time.deltaTime;

            // Quay thuận chiều (kim đồng hồ)
            if (canh1Holder != null)
                canh1Holder.RotateAround(transform.position, Vector3.up, currentSpeed * Time.deltaTime);

            if (canh3Holder != null)
                canh3Holder.RotateAround(transform.position, Vector3.up, currentSpeed * Time.deltaTime);

            // Quay ngược chiều
            if (canh2Holder != null)
                canh2Holder.RotateAround(transform.position, Vector3.up, -currentSpeed * Time.deltaTime);

            if (canh4Holder != null)
                canh4Holder.RotateAround(transform.position, Vector3.up, -currentSpeed * Time.deltaTime);
        }

    }
}
