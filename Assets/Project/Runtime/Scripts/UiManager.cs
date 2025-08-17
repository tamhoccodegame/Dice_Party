using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Serialization;
using UnityEngine;

public class UiManager : WizardMiniGameManager
{
    public Transform[] topPositions;
    public Transform[] rightPositions;
    public Transform[] bottomPositions;
    public Transform[] leftPositions;

    public GameObject enemyPrefab;

    public int activeEnemys;

    protected override void Awake()
    {
        //base.Awake();
    }

    protected override void Start()
    {
        //base.Start();

        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (time > 0)
        {
            while (activeEnemys >= 7) yield return null;

            HashSet<Transform> usedPositions = new HashSet<Transform>();

            int spawnCount = Mathf.Min(7 - activeEnemys, 3); // spawn 3 con 1 batch
            int spawned = 0;

            while (spawned < spawnCount)
            {
                Transform[] direction = GetRandomSpawnDirection();
                Transform pos = direction[Random.Range(0, direction.Length)];

                // Check trùng vị trí
                if (!usedPositions.Contains(pos))
                {
                    Instantiate(enemyPrefab, pos.position, pos.rotation);
                    usedPositions.Add(pos);
                    activeEnemys++;
                    spawned++;
                }
                else
                {
                    // Trùng thì random lại
                    yield return null;
                    continue;
                }

                yield return null; // tránh freeze
            }

            yield return new WaitForSeconds(0.5f);
        }
    }



    bool IsPositionFree(Vector3 pos, float checkRadius = 0.5f)
    {
        Collider[] hits = Physics.OverlapSphere(pos, checkRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy")) return false;
        }
        return true;
    }


    Transform[] GetRandomSpawnDirection()
    {
        int index = Random.Range(1, 5);

        switch(index)
        {
            case 1:  return topPositions;
            case 2: return rightPositions;
            case 3: return bottomPositions;
            case 4: return leftPositions;
        }
        return null;
    }

    public override bool CheckGameOver()
    {
        return base.CheckGameOver();
    }

    public override void UpdateHUD()
    {
        base.UpdateHUD();
    }

    public override void ShowGameOverPanel()
    {
        base.ShowGameOverPanel();
    }

    public override void SpawnRewardAvatar()
    {
        base.SpawnRewardAvatar();
    }
}
