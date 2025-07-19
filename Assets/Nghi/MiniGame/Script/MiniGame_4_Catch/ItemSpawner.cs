using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Area Settings")]
    public float spawnRadius = 5f;              // Bán kính vùng spawn hình tròn
    public float spawnHeight = 10f;             // Độ cao bắt đầu spawn trên trời

    [Header("Item Spawn Settings")]
    public int itemsPerBatch = 10;              // Bao nhiêu item spawn cùng lúc
    public float spawnInterval = 1f;            // Bao lâu spawn 1 đợt
    public List<GameObject> itemPrefabs;        // List các prefab item

    [Header("Item Physics Settings")]
    public float minGravity = -5f;
    public float maxGravity = -20f;

    public float minFallSpeed = 1f;
    public float maxFallSpeed = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnItems();
            timer = 0f;
        }

        
    }

    void SpawnItems()
    {
        if (itemPrefabs == null || itemPrefabs.Count == 0) return;

        for (int i = 0; i < itemsPerBatch; i++)
        {
            // Random vị trí trong vòng tròn
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = new Vector3(circle.x, spawnHeight, circle.y) + transform.position;

            // Random chọn prefab
            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
            GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);

            // Gán thông số vật lý ngẫu nhiên nếu có Rigidbody
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float gravity = Random.Range(minGravity, maxGravity);
                float fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);

                rb.useGravity = false; // Tắt gravity mặc định
                rb.velocity = Vector3.down * fallSpeed; // Set tốc độ rơi ban đầu

                // Thêm trọng lực tùy chỉnh trong Update
                FallingItem fallingScript = item.GetComponent<FallingItem>();
                if (fallingScript != null)
                {
                    fallingScript.customGravity = gravity;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        int segments = 60;
        float angle = 0f;

        Vector3 center = transform.position;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0) * spawnRadius, 0, Mathf.Sin(0) * spawnRadius);

        for (int i = 1; i <= segments; i++)
        {
            angle = i * Mathf.PI * 2f / segments;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * spawnRadius, 0, Mathf.Sin(angle) * spawnRadius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }

        // Vẽ luôn đường lên cao (spawnHeight) cho dễ thấy
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * spawnHeight);
    }

}
