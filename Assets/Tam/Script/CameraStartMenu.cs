using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.Sockets.NetBitBuffer;

public class CameraStartMenu : MonoBehaviour
{
    public Transform target;      // Vật thể cần focus
    public float distance = 5f;   // Khoảng cách từ camera đến target
    public float orbitSpeed = 30f; // Tốc độ xoay quanh (độ/giây)
    public float heightOffset = 0f; // Độ cao so với object

    private Vector3 offset;         // Lưu khoảng cách ban đầu giữa camera và target
    private float currentAngle;     // Góc quay

    void Start()
    {
        if (target == null) return;

        // Lưu khoảng cách ban đầu và góc quay ban đầu (theo vị trí bạn setup camera)
        offset = transform.position - target.position;
        Vector3 flatOffset = new Vector3(offset.x, 0f, offset.z);
        currentAngle = Mathf.Atan2(flatOffset.z, flatOffset.x) * Mathf.Rad2Deg;

        if (distance <= 0f)
        {
            distance = flatOffset.magnitude;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Cập nhật góc quay
        currentAngle += orbitSpeed * Time.deltaTime;

        // Tính toán vị trí mới
        float rad = currentAngle * Mathf.Deg2Rad;
        float x = target.position.x + distance * Mathf.Cos(rad);
        float z = target.position.z + distance * Mathf.Sin(rad);
        float y = target.position.y + heightOffset;

        transform.position = new Vector3(x, y, z);
        transform.LookAt(target);
    }
}
