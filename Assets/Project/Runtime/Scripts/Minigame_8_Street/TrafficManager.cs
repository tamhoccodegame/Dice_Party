using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    [System.Serializable]
    public class LaneSlot
    {
        public GameObject lanePlane;
        public bool movePositiveX = true;

        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public Vector3 endPos;
        [HideInInspector] public List<GameObject> activeCars = new List<GameObject>();
        [HideInInspector] public float nextSpawnTime = 0f;
        [HideInInspector] public float laneLength = 0f;
    }

    [Header("Prefabs")]
    public List<GameObject> carPrefabs;

    [Header("Slots")]
    public LaneSlot[] slots;

    [Header("Global speed/spawn tuning")]
    public float minSpeed = 6f;
    public float maxSpeed = 14f;
    public float minVehicleLength = 3.2f;
    public float maxVehicleLength = 5f;

    [Header("Spawn timing (seconds)")]
    public float minSpawnDelay = 0.35f;
    public float maxSpawnDelay = 2.2f;

    [Header("Distance tuning (meters)")]
    public float defaultMinGap = 1.8f; // s0
    public float defaultHeadway = 1.1f; // T

    [Header("Gap / player crossing logic")]
    [Range(0f, 0.6f)] public float gapChance = 0.14f; // chance to create longer gap for player crossing
    public float gapMultiplier = 2.5f;

    [Header("Debug")]
    public bool drawSpawnGizmo = true;

    // prefab bag randomizer
    private Queue<GameObject> prefabBag = new Queue<GameObject>();

    private void Start()
    {
        if (carPrefabs == null || carPrefabs.Count == 0)
            Debug.LogWarning("[TrafficManager] No carPrefabs assigned.");

        // setup slots start/end using lanePlane bounds
        foreach (var slot in slots)
        {
            if (slot.lanePlane == null) continue;
            Renderer r = slot.lanePlane.GetComponent<Renderer>();
            if (r == null) continue;
            Bounds b = r.bounds;

            if (slot.movePositiveX)
            {
                slot.startPos = new Vector3(b.min.x, b.center.y, b.center.z);
                slot.endPos = new Vector3(b.max.x, b.center.y, b.center.z);
            }
            else
            {
                slot.startPos = new Vector3(b.max.x, b.center.y, b.center.z);
                slot.endPos = new Vector3(b.min.x, b.center.y, b.center.z);
            }

            slot.laneLength = Vector3.Distance(slot.startPos, slot.endPos);

            // initial spawn for visual
            SpawnCarInSlot(slot);
            // small randomized delay before next spawn
            slot.nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
        }
    }

    private void Update()
    {
        foreach (var slot in slots)
            UpdateSlot(slot);
    }

    private void UpdateSlot(LaneSlot slot)
    {
        if (slot.lanePlane == null) return;

        Vector3 dir = (slot.endPos - slot.startPos).normalized;
        float dt = Time.deltaTime;

        // Clean nulls first
        slot.activeCars.RemoveAll(c => c == null);

        // Build ordering by distance along lane (from start)
        List<GameObject> ordered = new List<GameObject>(slot.activeCars);
        ordered.Sort((a, b) =>
        {
            float ta = Vector3.Dot(dir, a.transform.position - slot.startPos);
            float tb = Vector3.Dot(dir, b.transform.position - slot.startPos);
            return ta.CompareTo(tb); // ascending: 0 = nearest start (behind), last = leader
        });

        // Update movement leader -> follower: iterate from last -> first
        for (int i = ordered.Count - 1; i >= 0; i--)
        {
            GameObject go = ordered[i];
            if (go == null) continue;
            CarInfo ctrl = go.GetComponent<CarInfo>();
            if (ctrl == null) continue;

            // assign carAhead reference (if any)
            CarInfo aheadCtrl = (i == ordered.Count - 1) ? null : ordered[i + 1].GetComponent<CarInfo>();
            ctrl.carAhead = aheadCtrl;

            // Step movement
            ctrl.Step(dir, dt);

            // destroy if reach end (use projection distance)
            float tpos = Vector3.Dot(dir, go.transform.position - slot.startPos);
            if (tpos >= slot.laneLength - 0.5f)
            {
                Destroy(go);
                // will be pruned in next loop by RemoveAll
            }
        }

        // SPAWN logic: after movement check time and nearest car to start
        if (Time.time >= slot.nextSpawnTime)
        {
            // find nearest (smallest t) among ordered; if none -> big value
            float nearestT = float.MaxValue;
            if (ordered.Count > 0)
            {
                nearestT = Vector3.Dot(dir, ordered[0].transform.position - slot.startPos);
            }

            // pick a spawn speed for candidate
            float spawnSpeed = Random.Range(minSpeed, maxSpeed);

            // compute safe center distance for spawn (center-to-center)
            float spawnVehicleLength = Random.Range(minVehicleLength, maxVehicleLength);
            float spawnMinGap = Random.Range(defaultMinGap * 0.8f, defaultMinGap * 1.3f); // variability
            float spawnHeadway = Random.Range(defaultHeadway * 0.9f, defaultHeadway * 1.4f);

            float safeCenterDistance = spawnVehicleLength + spawnMinGap + spawnSpeed * spawnHeadway;

            // can we spawn? check nearestT (which is center-distance from start to nearest car center)
            if (nearestT >= safeCenterDistance || ordered.Count == 0)
            {
                // optionally create a gap to allow player crossing
                if (Random.value < gapChance)
                {
                    slot.nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay) * gapMultiplier;
                }
                else
                {
                    // spawn and set nextSpawnTime based on time gap (safeCenterDistance/spawnSpeed)
                    float timeGap = safeCenterDistance / Mathf.Max(0.001f, spawnSpeed);
                    timeGap = Mathf.Clamp(timeGap, minSpawnDelay, maxSpawnDelay);
                    SpawnCarInSlot(slot, spawnSpeed, spawnVehicleLength, spawnMinGap, spawnHeadway);
                    slot.nextSpawnTime = Time.time + timeGap * Random.Range(0.9f, 1.2f);
                }
            }
            else
            {
                // too close, wait a bit then re-evaluate
                slot.nextSpawnTime = Time.time + minSpawnDelay * 0.8f;
            }
        }
    }

    /// <summary>
    /// Spawn a car with optional parameter overrides (if default: randomize).
    /// Returns the created GameObject or null.
    /// </summary>
    private GameObject SpawnCarInSlot(LaneSlot slot, float overrideSpeed = -1f, float overrideLength = -1f, float overrideMinGap = -1f, float overrideHeadway = -1f)
    {
        GameObject prefab = DrawPrefab();
        if (prefab == null) return null;

        GameObject newCar = Instantiate(prefab, slot.startPos, Quaternion.identity);
        Vector3 dir = (slot.endPos - slot.startPos).normalized;
        newCar.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        // ensure CarController exists
        CarInfo ctrl = newCar.GetComponent<CarInfo>();
        if (ctrl == null) ctrl = newCar.AddComponent<CarInfo>();

        // derive parameters
        float length = (overrideLength > 0f) ? overrideLength : Random.Range(minVehicleLength, maxVehicleLength);
        float desired = (overrideSpeed > 0f) ? overrideSpeed : Random.Range(minSpeed, maxSpeed);
        float gap = (overrideMinGap > 0f) ? overrideMinGap : Random.Range(defaultMinGap * 0.9f, defaultMinGap * 1.3f);
        float headway = (overrideHeadway > 0f) ? overrideHeadway : Random.Range(defaultHeadway * 0.9f, defaultHeadway * 1.3f);
        float accel = Random.Range(1.5f, 3.5f);
        float decel = Random.Range(4f, 8f);

        ctrl.Configure(desired, gap, headway, accel, decel, length);

        slot.activeCars.Add(newCar);

        // IMPORTANT: don't set carAhead here; will be rebuilt in next UpdateSlot ordering step (so ordering consistent)

        // ensure prefab has collider if you want physical collision (optional),
        // but this system handles overlaps via clamping, so collider is optional.
        return newCar;
    }

    private void RefillBag()
    {
        List<GameObject> temp = new List<GameObject>(carPrefabs);
        for (int i = 0; i < temp.Count; i++)
        {
            int r = Random.Range(i, temp.Count);
            (temp[i], temp[r]) = (temp[r], temp[i]);
        }
        foreach (var p in temp) prefabBag.Enqueue(p);
    }

    private GameObject DrawPrefab()
    {
        if (prefabBag.Count == 0) RefillBag();
        if (prefabBag.Count == 0) return null;
        return prefabBag.Dequeue();
    }

    private void OnDrawGizmos()
    {
        if (!drawSpawnGizmo) return;
        if (slots == null) return;

        Gizmos.color = Color.cyan;
        foreach (var s in slots)
        {
            if (s == null || s.lanePlane == null) continue;
            Vector3 a = s.startPos;
            Vector3 b = s.endPos;
            Gizmos.DrawLine(a, b);
            Gizmos.DrawSphere(a, 0.15f);
            Gizmos.DrawSphere(b, 0.15f);
        }
    }
}

