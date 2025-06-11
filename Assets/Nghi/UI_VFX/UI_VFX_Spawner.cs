using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_VFX_Spawner : MonoBehaviour
{
    public enum SpawnMode
    {
        RandomInArea,
        SequentialAtPoints,
        MoveAlongPath
    }

    [System.Serializable]
    public class ModeConfig
    {
        public SpawnMode mode;
        public bool enabled = true;
    }

    [Header("⚡ List of VFX Prefabs")]
    public List<GameObject> vfxPrefabs;

    [Header("🎯 Spawn Settings")]
    public RectTransform spawnArea;
    public float spawnInterval = 3f;
    public bool spawnOnStart = true;

    [Header("🧹 Cleanup")]
    public float destroyDelay = 5f;

    [Header("🎮 Mode Config List")]
    public List<ModeConfig> modeConfigs = new List<ModeConfig>();

    [Header("🔀 Mode Execution")]
    public bool executeModesSequentially = false; // False = chạy song song; True = chạy lần lượt từ trên xuống.

    [Header("🎞 UI Animation")]
    public float appearDuration = 0.5f;
    public Ease appearEase = Ease.OutBack;

    [Header("📍 For SequentialAtPoints Mode")]
    public List<Transform> spawnPoints;
    public bool playInOrder = true;
    public bool playOnAwakeOnly = false;
    public bool simultaneousSpawnAtAllPoints = false;

    [Header("🚀 For MoveAlongPath Mode")]
    public List<Transform> pathPoints;
    public float moveSpeed = 100f;
    public float waitBetweenMoves = 1f;
    public bool loopPath = false;
    public bool playVFXAtEachPoint = true;

    private bool isSpawning = false;
    private int currentIndex = 0;

    void Start()
    {
        if (spawnOnStart)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;

            if (executeModesSequentially)
                StartCoroutine(SequentialModeRoutine());
            else
                StartCoroutine(ParallelModeRoutine());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator SequentialModeRoutine()
    {
        foreach (var config in modeConfigs)
        {
            if (!config.enabled) continue;

            yield return StartCoroutine(SpawnRoutine(config.mode));
        }
    }

    private IEnumerator ParallelModeRoutine()
    {
        List<Coroutine> activeCoroutines = new List<Coroutine>();

        foreach (var config in modeConfigs)
        {
            if (!config.enabled) continue;

            Coroutine coroutine = StartCoroutine(SpawnRoutine(config.mode));
            activeCoroutines.Add(coroutine);
        }

        // Wait all coroutines finish (never ending loops excluded unless stopped externally)
        yield break;
    }

    private IEnumerator SpawnRoutine(SpawnMode mode)
    {
        while (isSpawning)
        {
            switch (mode)
            {
                case SpawnMode.RandomInArea:
                    SpawnRandomVFX();
                    break;
                case SpawnMode.SequentialAtPoints:
                    if (simultaneousSpawnAtAllPoints)
                        SpawnSimultaneouslyAtAllPoints();
                    else
                        SpawnAtNextPoint();
                    break;
                case SpawnMode.MoveAlongPath:
                    yield return MoveVFXAlongPath();
                    break;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRandomVFX()
    {
        if (vfxPrefabs.Count == 0 || spawnArea == null) return;

        GameObject vfxPrefab = vfxPrefabs[Random.Range(0, vfxPrefabs.Count)];
        GameObject vfxInstance = Instantiate(vfxPrefab, spawnArea);

        Vector2 size = spawnArea.rect.size;
        Vector2 randomPos = new Vector2(
            Random.Range(-size.x / 2f, size.x / 2f),
            Random.Range(-size.y / 2f, size.y / 2f)
        );

        RectTransform rt = vfxInstance.GetComponent<RectTransform>();
        rt.anchoredPosition = randomPos;
        rt.localScale = Vector3.zero;
        rt.DOScale(Vector3.one, appearDuration).SetEase(appearEase);

        Destroy(vfxInstance, destroyDelay);
    }

    private void SpawnAtNextPoint()
    {
        if (vfxPrefabs.Count == 0 || spawnPoints.Count == 0) return;

        GameObject vfxPrefab = vfxPrefabs[playOnAwakeOnly ? currentIndex % vfxPrefabs.Count : Random.Range(0, vfxPrefabs.Count)];
        Transform targetPoint = spawnPoints[currentIndex % spawnPoints.Count];

        GameObject vfxInstance = Instantiate(vfxPrefab, targetPoint.position, Quaternion.identity, spawnArea);
        RectTransform rt = vfxInstance.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;
        rt.DOScale(Vector3.one, appearDuration).SetEase(appearEase);

        Destroy(vfxInstance, destroyDelay);

        if (playInOrder)
            currentIndex++;
        else
            currentIndex = Random.Range(0, spawnPoints.Count);
    }

    private void SpawnSimultaneouslyAtAllPoints()
    {
        if (vfxPrefabs.Count == 0 || spawnPoints.Count == 0) return;

        foreach (Transform point in spawnPoints)
        {
            GameObject vfxPrefab = vfxPrefabs[Random.Range(0, vfxPrefabs.Count)];
            GameObject vfxInstance = Instantiate(vfxPrefab, point.position, Quaternion.identity, spawnArea);
            RectTransform rt = vfxInstance.GetComponent<RectTransform>();
            rt.localScale = Vector3.zero;
            rt.DOScale(Vector3.one, appearDuration).SetEase(appearEase);
            Destroy(vfxInstance, destroyDelay);
        }
    }

    private IEnumerator MoveVFXAlongPath()
    {
        if (vfxPrefabs.Count == 0 || pathPoints.Count < 2) yield break;

        GameObject vfxPrefab = vfxPrefabs[Random.Range(0, vfxPrefabs.Count)];
        GameObject vfxInstance = Instantiate(vfxPrefab, pathPoints[0].position, Quaternion.identity, spawnArea);

        if (loopPath)
        {
            ParticleSystem[] particleSystems = vfxInstance.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particleSystems)
            {
                var main = ps.main;
                main.loop = true;
            }
        }

        RectTransform rt = vfxInstance.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;
        rt.DOScale(Vector3.one, appearDuration).SetEase(appearEase);

        int index = 0;

        yield return new WaitForSeconds(appearDuration);

        while (true)
        {
            int nextIndex = (index + 1) % pathPoints.Count;

            Vector2 currentPos = rt.anchoredPosition;
            Vector2 targetPos = pathPoints[nextIndex].GetComponent<RectTransform>().anchoredPosition;
            float distance = Vector2.Distance(currentPos, targetPos);
            float duration = distance / moveSpeed;

            yield return rt.DOAnchorPos(targetPos, duration).SetEase(Ease.Linear).WaitForCompletion();

            if (playVFXAtEachPoint)
                PlayVFX(rt);

            if (waitBetweenMoves > 0f)
                yield return new WaitForSeconds(waitBetweenMoves);

            index = nextIndex;

            if (!loopPath && index == pathPoints.Count - 1)
                break;
        }

        if (!loopPath)
        {
            Destroy(vfxInstance, destroyDelay);
        }
    }

    private void PlayVFX(RectTransform rt)
    {
        ParticleSystem ps = rt.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
            ps.Play();
    }

}
