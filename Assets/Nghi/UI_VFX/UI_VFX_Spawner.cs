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
    public class VFXPrefabConfig
    {
        public GameObject prefab;
        public bool useCustomSettings;

        // RandomInArea
        public RectTransform customSpawnArea;

        // SequentialAtPoints
        public List<Transform> customSpawnPoints;
        public bool playInOrder;
        public bool playOnAwakeOnly;
        public bool simultaneousSpawnAtAllPoints;

        // MoveAlongPath
        public List<Transform> customPathPoints;
        public float moveSpeed = 100f;
        public float waitBetweenMoves = 1f;
        public bool loopPath;

        // Common per-VFX settings
        public float appearDuration = 0.5f;
        public Ease appearEase = Ease.OutBack;
    }

    [System.Serializable]
    public class ModeConfig
    {
        public SpawnMode mode;
        public bool enabled = true;
        public List<GameObject> vfxPrefabs = new List<GameObject>();
    }

    [Header("🎯 Spawn Settings")]
    public RectTransform spawnArea;
    public float spawnInterval = 3f;
    public bool spawnOnStart = true;

    [Header("🧹 Cleanup")]
    public float destroyDelay = 5f;

    [Header("🎮 Mode Config List")]
    public List<ModeConfig> modeConfigs = new List<ModeConfig>();

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
            StartCoroutine(ParallelModeRoutine());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator ParallelModeRoutine()
    {
        List<Coroutine> activeCoroutines = new List<Coroutine>();

        foreach (var config in modeConfigs)
        {
            if (!config.enabled) continue;

            Coroutine coroutine = StartCoroutine(SpawnRoutine(config));
            activeCoroutines.Add(coroutine);
        }

        yield break;
    }

    private IEnumerator SpawnRoutine(ModeConfig config)
    {
        while (isSpawning)
        {
            switch (config.mode)
            {
                case SpawnMode.RandomInArea:
                    SpawnRandomVFX(config);
                    break;
                case SpawnMode.SequentialAtPoints:
                    if (simultaneousSpawnAtAllPoints)
                        SpawnSimultaneouslyAtAllPoints(config);
                    else
                        SpawnAtNextPoint(config);
                    break;
                case SpawnMode.MoveAlongPath:
                    yield return MoveVFXAlongPath(config);
                    break;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRandomVFX(ModeConfig config)
    {
        if (config.vfxPrefabs.Count == 0 || spawnArea == null) return;

        GameObject vfxPrefab = config.vfxPrefabs[Random.Range(0, config.vfxPrefabs.Count)];
        GameObject vfxInstance = Instantiate(vfxPrefab, spawnArea);
        DisablePlayOnAwake(vfxInstance);

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

    private void SpawnAtNextPoint(ModeConfig config)
    {
        if (config.vfxPrefabs.Count == 0 || spawnPoints.Count == 0) return;

        GameObject vfxPrefab = config.vfxPrefabs[
            playOnAwakeOnly ? currentIndex % config.vfxPrefabs.Count : Random.Range(0, config.vfxPrefabs.Count)
        ];

        Transform targetPoint = spawnPoints[currentIndex % spawnPoints.Count];

        GameObject vfxInstance = Instantiate(vfxPrefab, targetPoint.position, Quaternion.identity, spawnArea);
        DisablePlayOnAwake(vfxInstance);
        RectTransform rt = vfxInstance.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;
        rt.DOScale(Vector3.one, appearDuration).SetEase(appearEase);

        Destroy(vfxInstance, destroyDelay);

        if (playInOrder)
            currentIndex++;
        else
            currentIndex = Random.Range(0, spawnPoints.Count);
    }

    private void SpawnSimultaneouslyAtAllPoints(ModeConfig config)
    {
        if (config.vfxPrefabs.Count == 0 || spawnPoints.Count == 0) return;

        foreach (Transform point in spawnPoints)
        {
            GameObject vfxPrefab = config.vfxPrefabs[Random.Range(0, config.vfxPrefabs.Count)];
            GameObject vfxInstance = Instantiate(vfxPrefab, point.position, Quaternion.identity, spawnArea);
            DisablePlayOnAwake(vfxInstance);
            RectTransform rt = vfxInstance.GetComponent<RectTransform>();
            rt.localScale = Vector3.zero;
            rt.DOScale(Vector3.one, appearDuration).SetEase(appearEase);
            Destroy(vfxInstance, destroyDelay);
        }
    }

    private IEnumerator MoveVFXAlongPath(ModeConfig config)
    {
        if (config.vfxPrefabs.Count == 0 || pathPoints.Count < 2) yield break;

        GameObject vfxPrefab = config.vfxPrefabs[Random.Range(0, config.vfxPrefabs.Count)];
        GameObject vfxInstance = Instantiate(vfxPrefab, pathPoints[0].position, Quaternion.identity, spawnArea);
        DisablePlayOnAwake(vfxInstance);

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

    private void DisablePlayOnAwake(GameObject vfxInstance)
    {
        ParticleSystem[] particleSystems = vfxInstance.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem ps in particleSystems)
        {
            var main = ps.main;
            main.playOnAwake = false;
        }
    }
}
