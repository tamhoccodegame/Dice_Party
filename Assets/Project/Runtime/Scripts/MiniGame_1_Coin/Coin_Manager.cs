using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Coin_Manager : MonoBehaviour
{
    public static Coin_Manager Instance { get; private set; }
    public int TotalCoins { get; private set; } = 0;
    public GameObject coinPrefab;
    public GameObject pickupVFX;

    [Header("Drop Settings")]
    public int coinsToDropOnHit = 3;
    public float coinSpawnHeight = 0.5f;
    public float coinLifetime = 5f;
    public float spawnForce = 2.5f;

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


    public void DropCoins(Vector3 origin)
    {
        int dropCount = Mathf.Min(TotalCoins, coinsToDropOnHit);
        if (dropCount <= 0)
        {
            Debug.Log("[⚠️ DROP] Not enough coins to drop.");
            return;
        }

        RemoveCoins(dropCount);

        for (int i = 0; i < dropCount; i++)
        {
            // 👉 spawn tại player, thêm chút chiều cao để không dính sàn
            Vector3 spawnPos = origin + Vector3.up * coinSpawnHeight;
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
                // 👉 Văng ra các hướng ngẫu nhiên, có hướng lên nhẹ để nảy
                Vector3 randomDir = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.4f, 1.2f),
                    Random.Range(-1f, 1f)
                ).normalized;

                rb.AddForce(randomDir * spawnForce, ForceMode.Impulse);

                // 👉 Add torque để coin xoay xoay mượt hơn
                Vector3 torque = new Vector3(
                    Random.Range(-200, 200),
                    Random.Range(-200, 200),
                    Random.Range(-200, 200)
                );
                rb.AddTorque(torque);
            }
        }

        Debug.Log($"[💥 COINS DROPPED] {dropCount} coins dropped at {origin}");
    }
}