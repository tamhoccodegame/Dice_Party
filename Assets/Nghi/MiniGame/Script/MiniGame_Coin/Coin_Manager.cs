using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_Manager : MonoBehaviour
{
    public static Coin_Manager Instance { get; private set; }

    //[Header("Coin Settings")]
    public int TotalCoins { get; private set; } = 0;
    public GameObject coinPrefab;
    public GameObject pickupVFX;

    [Header("Drop Settings")]
    public int coinsToDropOnHit = 3;
    public float coinSpawnHeight = 1.2f;
    public float coinLifetime = 5f;
    public float spawnForce = 2.5f;

    public enum DropPattern { Line, Triangle, Diagonal }
    public DropPattern pattern = DropPattern.Triangle;
    public float spacing = 0.5f;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        Debug.Log($"[🟢 COIN ADDED] +{amount} -> Total: {TotalCoins}");
    }

    public void RemoveCoins(int amount)
    {
        int actual = Mathf.Min(TotalCoins, amount);
        TotalCoins -= actual;
        Debug.Log($"[🔴 COIN REMOVED] -{actual} -> Total: {TotalCoins}");
    }

    //public void DropCoins(Vector3 origin)
    //{
    //    int dropCount = Mathf.Min(TotalCoins, coinsToDropOnHit);
    //    if (dropCount <= 0)
    //    {
    //        Debug.Log("[⚠️ DROP] Not enough coins to drop.");
    //        return;
    //    }

    //    RemoveCoins(dropCount);

    //    List<Vector3> positions = GeneratePatternPositions(origin, dropCount, pattern);

    //    for (int i = 0; i < dropCount; i++)
    //    {
    //        Vector3 spawnPos = positions[i] + Vector3.up * coinSpawnHeight;
    //        GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

    //        // Setup coin script
    //        Coins coinScript = coin.GetComponent<Coins>();
    //        if (coinScript != null)
    //        {
    //            coinScript.SetLifetime(coinLifetime);
    //            coinScript.value = 1;
    //            coinScript.pickupVFX = pickupVFX;
    //        }

    //        // Push coin away with force
    //        Rigidbody rb = coin.GetComponent<Rigidbody>();
    //        if (rb != null)
    //        {
    //            Vector3 dir = (Vector3.up + Random.insideUnitSphere).normalized;
    //            rb.AddForce(dir * spawnForce, ForceMode.Impulse);
    //        }
    //    }

    //    Debug.Log($"[💥 COINS DROPPED] Dropped {dropCount} coins at {origin}");
    //}

    public void DropCoins(Vector3 origin)
    {
        int dropCount = Mathf.Min(TotalCoins, coinsToDropOnHit);
        if (dropCount <= 0)
        {
            Debug.Log("[⚠️ DROP] Not enough coins to drop.");
            return;
        }

        RemoveCoins(dropCount);

        List<Vector3> positions = GeneratePatternPositions(origin, dropCount, pattern);

        for (int i = 0; i < dropCount; i++)
        {
            Vector3 spawnPos = positions[i] + Vector3.up * coinSpawnHeight;
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

            Coins coinScript = coin.GetComponent<Coins>();
            if (coinScript != null)
            {
                coinScript.SetLifetime(coinLifetime);
                coinScript.value = 1;
                coinScript.pickupVFX = pickupVFX;
            }

            Rigidbody rb = coin.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 👉 bay đúng hướng từ tâm ra coin, giữ hình đẹp
                Vector3 dir = (positions[i] - origin).normalized + Vector3.up * 0.2f;
                rb.AddForce(dir.normalized * spawnForce, ForceMode.Impulse);
            }
        }

        Debug.Log($"[💥 COINS DROPPED] Dropped {dropCount} coins at {origin} in shape: {pattern}");
    }


    private List<Vector3> GeneratePatternPositions(Vector3 origin, int count, DropPattern pattern)
    {
        List<Vector3> positions = new List<Vector3>();

        switch (pattern)
        {
            case DropPattern.Line:
                for (int i = 0; i < count; i++)
                    positions.Add(origin + Vector3.right * ((i - (count - 1) / 2f) * spacing));
                break;

            case DropPattern.Diagonal:
                for (int i = 0; i < count; i++)
                    positions.Add(origin + new Vector3(i * spacing, 0, i * spacing));
                break;

            case DropPattern.Triangle:
                int rows = Mathf.CeilToInt(Mathf.Sqrt(2 * count));
                int spawned = 0;
                for (int row = 0; row < rows && spawned < count; row++)
                {
                    int rowCount = row + 1;
                    float startX = -((rowCount - 1) * spacing) / 2f;
                    for (int j = 0; j < rowCount && spawned < count; j++)
                    {
                        Vector3 pos = origin + new Vector3(startX + j * spacing, 0, row * spacing);
                        positions.Add(pos);
                        spawned++;
                    }
                }
                break;
        }

        return positions;
    }
}
