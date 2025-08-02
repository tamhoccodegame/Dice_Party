using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy__Spawner : MonoBehaviour
{
    [Header("Config")]
    public Enemy__Pool pool;
    public GenerateGrid grid;

    public float baseSpeed = 5f;
    public float speedIncreasePerWave = 0.5f;
    public int minGroupSize = 2;
    public int maxGroupSize = 5;

    [Tooltip("Khoảng cách spawn ra ngoài grid")]
    [SerializeField] float spawnOffset = 10f;

    [Tooltip("Khoảng cách từ center để despawn")]
    [SerializeField] float despawnDistanceBuffer = 10f;

    [Tooltip("Delay sau khi group xong để spawn group mới")]
    [SerializeField] float nextGroupDelay = 0.5f;

    private float currentSpeed;
    private List<Enemy__Controller> activeEnemies = new List<Enemy__Controller>();

    void Start()
    {
        currentSpeed = baseSpeed;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnGroupAndWait());
            yield return new WaitForSeconds(nextGroupDelay);
        }
    }

    IEnumerator SpawnGroupAndWait()
    {
        SpawnCombinedGroup();

        // Đợi cho đến khi tất cả enemy despawn
        while (activeEnemies.Count > 0)
            yield return null;
    }

    void SpawnCombinedGroup()
    {
        int totalCount = Random.Range(minGroupSize, maxGroupSize + 1);

        // Chọn ít nhất 1 ngang + 1 dọc
        int horizontalCount = Random.Range(1, totalCount);
        int verticalCount = totalCount - horizontalCount;
        if (verticalCount == 0) verticalCount = 1;

        SpawnEnemiesInDirection(horizontalCount, true);
        SpawnEnemiesInDirection(verticalCount, false);
    }

    //void SpawnEnemiesInDirection(int count, bool horizontal)
    //{
    //    if (count <= 0) return;

    //    // Chọn hướng spawn
    //    List<Vector3> lineList;
    //    Vector3 dir;
    //    if (horizontal)
    //    {
    //        bool fromLeft = Random.Range(0, 2) == 0;
    //        lineList = fromLeft ? grid.leftLines : grid.rightLines;
    //        dir = fromLeft ? Vector3.right : Vector3.left;
    //    }
    //    else
    //    {
    //        bool fromTop = Random.Range(0, 2) == 0;
    //        lineList = fromTop ? grid.topLines : grid.bottomLines;
    //        dir = fromTop ? Vector3.back : Vector3.forward;
    //    }

    //    if (lineList.Count == 0) return;

    //    // Tính khoảng cách spawn từ center
    //    float halfCore = grid.coreSize / 2f;
    //    float spawnDistanceFromCenter = (halfCore + grid.wingLength + 1) * grid.GetGridSpacing() + spawnOffset;

    //    for (int i = 0; i < count; i++)
    //    {
    //        int idx = Random.Range(0, lineList.Count);
    //        Vector3 spawnPos = lineList[idx] + dir * spawnDistanceFromCenter;
    //        spawnPos.y = grid.groundY; // Fix lún đất

    //        // Hướng vào tâm
    //        Vector3 moveDir = -dir;

    //        Enemy__Controller enemy = pool.Get();
    //        enemy.transform.position = spawnPos;
    //        enemy.transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);

    //        // Speed random mỗi enemy
    //        float randomSpeed = currentSpeed * Random.Range(0.8f, 1.2f);

    //        // Set distance limit đủ lớn
    //        enemy.Init(moveDir, randomSpeed, grid.transform.position, spawnDistanceFromCenter + despawnDistanceBuffer);

    //        enemy.OnDespawn += HandleEnemyDespawn;
    //        activeEnemies.Add(enemy);
    //    }
    //}

    void SpawnEnemiesInDirection(int count, bool horizontal)
    {
        if (count <= 0) return;

        // Chọn hướng spawn
        List<Vector3> lineList;
        Vector3 dir;
        if (horizontal)
        {
            bool fromLeft = Random.Range(0, 2) == 0;
            lineList = fromLeft ? grid.leftLines : grid.rightLines;
            dir = fromLeft ? Vector3.right : Vector3.left;
        }
        else
        {
            bool fromTop = Random.Range(0, 2) == 0;
            lineList = fromTop ? grid.topLines : grid.bottomLines;
            dir = fromTop ? Vector3.back : Vector3.forward;
        }

        if (lineList.Count == 0) return;

        // Khoảng cách spawn
        float halfCore = grid.coreSize / 2f;
        float spawnDistanceFromCenter = (halfCore + grid.wingLength + 1) * grid.GetGridSpacing() + spawnOffset;

        float spacingBetweenEnemies = 2.0f; // khoảng cách giữa các enemy cùng line

        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, lineList.Count);
            Vector3 spawnPos = lineList[idx] + dir * spawnDistanceFromCenter;

            // Offset dọc theo moveDir để tránh dính chùm
            spawnPos += dir * (i * spacingBetweenEnemies);

            spawnPos.y = grid.groundY;

            Vector3 moveDir = -dir;

            Enemy__Controller enemy = pool.Get();
            enemy.transform.position = spawnPos;
            enemy.transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);

            enemy.Init(moveDir, currentSpeed, grid.transform.position, spawnDistanceFromCenter + despawnDistanceBuffer);

            enemy.OnDespawn += HandleEnemyDespawn;
            activeEnemies.Add(enemy);
        }

    }


    void HandleEnemyDespawn(Enemy__Controller enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);
    }

    public void NextWave()
    {
        currentSpeed += speedIncreasePerWave;
    }

    //[Header("Config")]
    //public Enemy__Pool pool;
    //public GenerateGrid grid;

    //public float baseSpeed = 5f;
    //public float speedIncreasePerWave = 0.5f;
    //public int minGroupSize = 2;
    //public int maxGroupSize = 5;

    //[Tooltip("Khoảng cách spawn ra ngoài grid")]
    //[SerializeField] float spawnOffset = 10f;

    //[Tooltip("Khoảng cách từ center để despawn")]
    //[SerializeField] float despawnDistanceBuffer = 5f;

    //[Tooltip("Delay sau khi group xong để spawn group mới")]
    //[SerializeField] float nextGroupDelay = 0.5f;

    //private float currentSpeed;
    //private List<Enemy__Controller> activeEnemies = new List<Enemy__Controller>();

    //void Start()
    //{
    //    currentSpeed = baseSpeed;
    //    StartCoroutine(SpawnLoop());
    //}

    //IEnumerator SpawnLoop()
    //{
    //    while (true)
    //    {
    //        yield return StartCoroutine(SpawnGroupAndWait());
    //        yield return new WaitForSeconds(nextGroupDelay);
    //    }
    //}

    //IEnumerator SpawnGroupAndWait()
    //{
    //    SpawnCombinedGroup();

    //    // Đợi cho đến khi tất cả enemy despawn
    //    while (activeEnemies.Count > 0)
    //        yield return null;
    //}

    ///// <summary>
    ///// Spawn group đảm bảo gồm cả ngang và dọc
    ///// </summary>
    //void SpawnCombinedGroup()
    //{
    //    int totalCount = Random.Range(minGroupSize, maxGroupSize + 1);

    //    // Chọn ít nhất 1 ngang + 1 dọc
    //    int horizontalCount = Random.Range(1, totalCount);
    //    int verticalCount = totalCount - horizontalCount;
    //    if (verticalCount == 0) verticalCount = 1;

    //    // Spawn ngang + dọc
    //    SpawnEnemiesInDirection(horizontalCount, true);
    //    SpawnEnemiesInDirection(verticalCount, false);
    //}

    ////void SpawnEnemiesInDirection(int count, bool horizontal)
    ////{
    ////    if (count <= 0) return;

    ////    // Chọn hướng spawn (Left/Right hoặc Top/Bottom)
    ////    List<Vector3> lineList;
    ////    Vector3 dir;
    ////    if (horizontal)
    ////    {
    ////        bool fromLeft = Random.Range(0, 2) == 0;
    ////        lineList = fromLeft ? grid.leftLines : grid.rightLines;
    ////        dir = fromLeft ? Vector3.right : Vector3.left;
    ////    }
    ////    else
    ////    {
    ////        bool fromTop = Random.Range(0, 2) == 0;
    ////        lineList = fromTop ? grid.topLines : grid.bottomLines;
    ////        dir = fromTop ? Vector3.back : Vector3.forward;
    ////    }

    ////    if (lineList.Count == 0) return;

    ////    // Random không trùng line
    ////    List<int> indices = new List<int>();
    ////    for (int i = 0; i < lineList.Count; i++) indices.Add(i);
    ////    for (int i = 0; i < indices.Count; i++)
    ////    {
    ////        int rand = Random.Range(i, indices.Count);
    ////        (indices[i], indices[rand]) = (indices[rand], indices[i]);
    ////    }

    ////    // Tính khoảng cách spawn từ center
    ////    float halfCore = grid.coreSize / 2f;
    ////    float spawnDistanceFromCenter = (halfCore + grid.wingLength + 1) * grid.GetGridSpacing() + spawnOffset;

    ////    for (int i = 0; i < count && i < indices.Count; i++)
    ////    {
    ////        Vector3 spawnPos = lineList[indices[i]] + dir * spawnDistanceFromCenter;
    ////        spawnPos.y = grid.groundY;

    ////        // Hướng chạy vào trong grid
    ////        Vector3 moveDir = -dir;

    ////        Enemy__Controller enemy = pool.Get();
    ////        enemy.transform.position = spawnPos;
    ////        enemy.transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    ////        enemy.Init(moveDir, currentSpeed, grid.transform.position, spawnDistanceFromCenter + despawnDistanceBuffer);

    ////        enemy.OnDespawn += HandleEnemyDespawn;
    ////        activeEnemies.Add(enemy);
    ////    }
    ////}

    //void SpawnEnemiesInDirection(int count, bool horizontal)
    //{
    //    if (count <= 0) return;

    //    // Chọn hướng spawn
    //    List<Vector3> lineList;
    //    Vector3 dir;
    //    if (horizontal)
    //    {
    //        bool fromLeft = Random.Range(0, 2) == 0;
    //        lineList = fromLeft ? grid.leftLines : grid.rightLines;
    //        dir = fromLeft ? Vector3.right : Vector3.left;
    //    }
    //    else
    //    {
    //        bool fromTop = Random.Range(0, 2) == 0;
    //        lineList = fromTop ? grid.topLines : grid.bottomLines;
    //        dir = fromTop ? Vector3.back : Vector3.forward;
    //    }

    //    if (lineList.Count == 0) return;

    //    // Tính khoảng cách spawn từ center
    //    float halfCore = grid.coreSize / 2f;
    //    float spawnDistanceFromCenter = (halfCore + grid.wingLength + 1) * grid.GetGridSpacing() + spawnOffset;

    //    // Spawn count enemy random line (KHÔNG chặn, KHÔNG skip)
    //    for (int i = 0; i < count; i++)
    //    {
    //        int idx = Random.Range(0, lineList.Count);
    //        Vector3 spawnPos = lineList[idx] + dir * spawnDistanceFromCenter;
    //        spawnPos.y = grid.groundY;

    //        // Hướng vào tâm
    //        Vector3 moveDir = -dir;

    //        Enemy__Controller enemy = pool.Get();
    //        enemy.transform.position = spawnPos;
    //        enemy.transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);

    //        // Set distance limit đủ lớn (spawnDistanceFromCenter + buffer)
    //        enemy.Init(moveDir, currentSpeed, grid.transform.position, spawnDistanceFromCenter + despawnDistanceBuffer);

    //        // Add callback
    //        enemy.OnDespawn += HandleEnemyDespawn;
    //        activeEnemies.Add(enemy);
    //    }
    //}


    //void HandleEnemyDespawn(Enemy__Controller enemy)
    //{
    //    if (activeEnemies.Contains(enemy))
    //        activeEnemies.Remove(enemy);
    //}

    //public void NextWave()
    //{
    //    currentSpeed += speedIncreasePerWave;
    //}
}
