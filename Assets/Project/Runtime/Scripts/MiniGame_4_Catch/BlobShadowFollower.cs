using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobShadowFollower : MonoBehaviour
{
    public Transform target;          // Vật thể cần follow
    public float heightOffset = 0.02f; // Đặt bóng hơi nổi lên để tránh z-fighting
    public LayerMask groundMask;       // Chỉ bắt ground
    public float minScale = 0.3f;      // Bóng nhỏ nhất khi ở cao
    public float maxScale = 1.0f;      // Bóng lớn nhất khi gần đất
    public float maxHeight = 10f;      // Chiều cao tối đa để tính scale

    void Update()
    {
        if (target == null) return;

        // Raycast xuống dưới từ target để tìm mặt đất
        if (Physics.Raycast(target.position, Vector3.down, out RaycastHit hit, 50f, groundMask))
        {
            // Đặt bóng tại vị trí va chạm
            transform.position = hit.point + Vector3.up * heightOffset;

            // Scale bóng theo độ cao (cao → nhỏ, thấp → to)
            float height = Mathf.Clamp(target.position.y - hit.point.y, 0, maxHeight);
            float t = 1f - (height / maxHeight); // 0 = cao nhất, 1 = sát đất
            float scale = Mathf.Lerp(minScale, maxScale, t);

            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            // Không thấy mặt đất → disable bóng
            transform.localScale = Vector3.zero;
        }
    }
}
