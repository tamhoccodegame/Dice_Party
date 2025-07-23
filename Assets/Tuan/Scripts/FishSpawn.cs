using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawn : MonoBehaviour
{
    public GameObject[] fishPrefabs;
    public Transform spawnArea;
    public float minDistance = 1.5f;
    public int numberOfFish = 10;
    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnFish();
    }

    void SpawnFish()
    {
        Vector3 center = spawnArea.position;
        Vector3 size = spawnArea.localScale;

        int attempts = 0;

        for (int i = 0; i < numberOfFish; i++)
        {
            bool placed = false;

            while (!placed && attempts < 1000)
            {
                attempts++;

                Vector3 randomPos = new Vector3(
                    Random.Range(center.x - size.x / 2f, center.x + size.x / 2f),
                    center.y,
                    Random.Range(center.z - size.z / 2f, center.z + size.z / 2f)
                );

                if (IsFarEnough(randomPos))
                {
                    Quaternion randomRot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    GameObject fish = Instantiate(GetRandomFish(), randomPos, randomRot);
                    fish.AddComponent<FishMovement>().spawnArea = spawnArea;
                    spawnedPositions.Add(randomPos);
                    placed = true;
                }
            }
        }
    }
    bool IsFarEnough(Vector3 newPos)
    {
        foreach (var pos in spawnedPositions)
        {
            if (Vector3.Distance(pos, newPos) < minDistance)
                return false;
        }
        return true;
    }
    GameObject GetRandomFish()
    {
        if (fishPrefabs.Length == 0) return null;
        return fishPrefabs[Random.Range(0, fishPrefabs.Length)];
    }


    // Vẽ vùng spawn trong Scene
    private void OnDrawGizmosSelected()
    {
        if (spawnArea == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(spawnArea.position, spawnArea.localScale);
    }
}
