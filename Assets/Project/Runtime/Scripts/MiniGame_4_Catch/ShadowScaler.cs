using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowScaler : MonoBehaviour
{
    [Header("Height Settings")]
    public Transform ground;       // Tham chiếu Ground hoặc đặt y = 0
    public float maxHeight = 10f;  // Chiều cao tối đa item có thể rơi
    public float minScale = 0.3f;  // Scale bóng nhỏ nhất khi ở cao
    public float maxScale = 1.0f;  // Scale bóng lớn nhất khi gần đất

    [Header("Internal")]
    private Vector3 originalScale;

    void Start()
    {
        // Lưu scale ban đầu (shape bóng thật theo model)
        originalScale = transform.localScale;

        if (ground == null)
        {
            var groundObj = GameObject.FindGameObjectWithTag("Ground");
            if (groundObj != null) ground = groundObj.transform;
        }
    }

    void LateUpdate()
    {
        if (ground == null) return;

        // Tính chiều cao hiện tại so với mặt đất
        float height = transform.position.y - ground.position.y;
        height = Mathf.Clamp(height, 0f, maxHeight);

        // Nội suy scale theo chiều cao
        float t = 1f - (height / maxHeight);
        float scaleFactor = Mathf.Lerp(minScale, maxScale, t);

        // Apply scale → bóng thật sẽ scale theo
        transform.localScale = originalScale * scaleFactor;
    }
}
