using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Area Settings")]
    public float spawnRadius = 5f;
    public float spawnHeight = 10f;

    [Header("Item Spawn Settings")]
    public int itemsPerBatch = 10;         // Bao nhiêu item trong 1 batch
    public float batchInterval = 2f;       // Thời gian giữa các batch

    [Header("Random Delay Per Item")]
    public float minDelay = 0.05f;         // Delay tối thiểu giữa 2 item
    public float maxDelay = 0.3f;          // Delay tối đa giữa 2 item

    [Header("Item Physics Random")]
    public float minGravity = -5f;
    public float maxGravity = -20f;
    public float minFallSpeed = 1f;
    public float maxFallSpeed = 5f;

    public List<GameObject> itemPrefabs;

    void Start()
    {
        // Tự động spawn theo batch
        InvokeRepeating(nameof(StartSpawnBatch), 0f, batchInterval);
    }

    void StartSpawnBatch()
    {
        if (!WizardMiniGameManager.instance.isGameStarted || WizardMiniGameManager.instance.isGameOver) return;
        StartCoroutine(SpawnBatchCoroutine());
    }

    IEnumerator SpawnBatchCoroutine()
    {
        if (itemPrefabs == null || itemPrefabs.Count == 0) yield break;

        for (int i = 0; i < itemsPerBatch; i++)
        {
            SpawnSingleItem();

            // Random delay giữa các item
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }

    void SpawnSingleItem()
    {
        // Random vị trí spawn trong vòng tròn
        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(circle.x, spawnHeight, circle.y) + transform.position;

        // Random chọn prefab
        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
        GameObject item = Instantiate(prefab, spawnPos, Quaternion.Euler(0, Random.Range(0f, 360f), 0));

        // Gán physics random
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float gravity = Random.Range(minGravity, maxGravity);
            float fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);

            rb.useGravity = false;
            rb.velocity = Vector3.down * fallSpeed;

            // Set custom gravity cho script FallingItem
            FallingItem fallingScript = item.GetComponent<FallingItem>();
            if (fallingScript != null)
            {
                fallingScript.customGravity = gravity;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        int segments = 60;
        Vector3 center = transform.position;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0) * spawnRadius, 0, Mathf.Sin(0) * spawnRadius);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * spawnRadius, 0, Mathf.Sin(angle) * spawnRadius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * spawnHeight);
    }

}
