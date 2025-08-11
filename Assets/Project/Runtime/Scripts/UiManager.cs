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
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while(!isGameStarted || isGameOver) yield return null;
        while(time > 0)
        {
            while (activeEnemys > 7) yield return null;

            Transform[] spawnPositions = GetRandomSpawnDirection();

            Transform[] lastSpawnPositions = spawnPositions;

            int lastRandomPosition = -1;

            foreach(var pos in spawnPositions)
            {
                int randomPosition = lastRandomPosition;
                while(randomPosition == lastRandomPosition)
                {
                    randomPosition = Random.Range(0, spawnPositions.Length);
                    yield return null;
                }
                lastRandomPosition = randomPosition;

                var go = Instantiate(enemyPrefab, spawnPositions[randomPosition].position, Quaternion.identity);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            while(spawnPositions == lastSpawnPositions)
            {
                spawnPositions = GetRandomSpawnDirection();
                yield return null;
            }


            foreach (var pos in spawnPositions)
            {
                int randomPosition = lastRandomPosition;
                while (randomPosition == lastRandomPosition)
                {
                    randomPosition = Random.Range(0, spawnPositions.Length);
                    yield return null;
                }
                lastRandomPosition = randomPosition;

                var go = Instantiate(enemyPrefab, spawnPositions[randomPosition].position, Quaternion.identity);
                yield return null;
            }

            yield return null;
        }
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
