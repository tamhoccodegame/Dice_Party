using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy__Spawner : MonoBehaviour
{
    [Header("Config")]
    public Enemy__Pool pool;
    public float baseSpeed = 5f;
    public float speedIncreasePerWave = 0.5f;
    public float baseSpawnInterval = 1.5f;
    public float minSpawnInterval = 0.4f;
    public float intervalDecreasePerWave = 0.1f;
    public int linesPerSide = 5;
    public float gridSpacing = 1.5f; // khoảng cách giữa các line

    [Header("Spawn Points (Edges)")]
    public Transform topEdge;
    public Transform bottomEdge;
    public Transform leftEdge;
    public Transform rightEdge;

    private float currentSpeed;
    private float currentInterval;

    void Start()
    {
        currentSpeed = baseSpeed;
        currentInterval = baseSpawnInterval;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(currentInterval);
        }
    }

    void SpawnEnemy()
    {
        // Chọn cạnh ngẫu nhiên
        int side = Random.Range(0, 4);
        Transform edge = null;
        Vector3 dir = Vector3.zero;

        switch (side)
        {
            case 0: // Top -> xuống
                edge = topEdge;
                dir = Vector3.back;
                break;
            case 1: // Bottom -> lên
                edge = bottomEdge;
                dir = Vector3.forward;
                break;
            case 2: // Left -> phải
                edge = leftEdge;
                dir = Vector3.right;
                break;
            case 3: // Right -> trái
                edge = rightEdge;
                dir = Vector3.left;
                break;
        }

        // Chọn line (offset) trong 5 line
        int lineIndex = Random.Range(0, linesPerSide);
        Vector3 offset = Vector3.zero;

        if (side == 0 || side == 1) // Spawn ngang (Top/Bottom)
            offset = Vector3.right * ((lineIndex - (linesPerSide / 2)) * gridSpacing);
        else // Spawn dọc (Left/Right)
            offset = Vector3.forward * ((lineIndex - (linesPerSide / 2)) * gridSpacing);

        Vector3 spawnPos = edge.position + offset;

        // Spawn từ pool
        Enemy__Controller enemy = pool.Get();
        enemy.transform.position = spawnPos;
        enemy.transform.rotation = Quaternion.identity;
        enemy.Init(dir, currentSpeed);
    }

    public void NextWave()
    {
        currentSpeed += speedIncreasePerWave;
        currentInterval = Mathf.Max(minSpawnInterval, currentInterval - intervalDecreasePerWave);
    }
}
