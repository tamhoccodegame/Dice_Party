using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TrapPreset
{
    public GameObject prefab;
    public float speedMin = 5f;
    public float speedMax = 15f;
    public Trap_Spawner.FaceAxis faceAxis = Trap_Spawner.FaceAxis.Forward;
    public int spawnCount = 1;
}

public class Trap_Spawner : MonoBehaviour
{
    public enum FaceAxis { Forward, Backward, Left, Right }

    [Header("Spline Settings")]
    public SplineComputer spline;
    [Range(0f, 1f)] public float visibleStart = 0.05f;
    [Range(0f, 1f)] public float visibleEnd = 0.95f;

    [Header("Trap Presets")]
    public List<TrapPreset> trapPresets;

    [Header("Wave Logic")]
    public float minSlotSpacing = 0.15f;
    public float phaseOffset = 0.4f;
    public float globalTrapSpacingDelay = 0.2f;

    private List<Trap_AI> allTraps = new();

    void Start()
    {
        StartCoroutine(SpawnAllTrapsWithLogic());
    }

    IEnumerator SpawnAllTrapsWithLogic()
    {
        List<float> usedSlots = new();

        foreach (var preset in trapPresets)
        {
            for (int i = 0; i < preset.spawnCount; i++)
            {
                float slot = GetValidSlot(usedSlots);
                if (slot < 0f) continue;

                SplineSample sample = new SplineSample();
                spline.Evaluate(slot, ref sample);

                GameObject trap = Instantiate(preset.prefab, sample.position, Quaternion.identity);
                usedSlots.Add(slot);

                // Gán layer tạm thời để camera không thấy trap này
                SetLayerRecursively(trap.transform, LayerMask.NameToLayer("Trap_Invisible"));

                var follower = trap.GetComponent<SplineFollower>();
                if (follower == null)
                {
                    Debug.LogError($"❌ Trap prefab '{preset.prefab.name}' thiếu SplineFollower!");
                    continue;
                }

                follower.spline = spline;
                follower.follow = false;
                follower.SetPercent(slot);
                follower.followSpeed = Random.Range(preset.speedMin, preset.speedMax);
                follower.wrapMode = SplineFollower.Wrap.PingPong;
                follower.motion.applyRotation = false;
                follower.direction = (i % 2 == 0) ? Spline.Direction.Forward : Spline.Direction.Backward;

                trap.transform.rotation = Quaternion.LookRotation(GetAxisVector(preset.faceAxis, trap.transform), Vector3.up);

                var runtime = trap.AddComponent<Trap_AI>();
                runtime.Setup(follower, allTraps, i * phaseOffset, preset.prefab.name);
                allTraps.Add(runtime);

                // Sau vài frame mới bật trở lại layer thường → camera mới render nó
                StartCoroutine(SwitchLayerDelayed(trap.transform, "Trap", 0.1f + i * 0.05f));

                yield return new WaitForSeconds(globalTrapSpacingDelay);
            }
        }
    }

    IEnumerator SwitchLayerDelayed(Transform target, string newLayerName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetLayerRecursively(target, LayerMask.NameToLayer(newLayerName));
    }

    void SetLayerRecursively(Transform trans, int layer)
    {
        trans.gameObject.layer = layer;
        foreach (Transform child in trans)
        {
            SetLayerRecursively(child, layer);
        }
    }


    IEnumerator EnableTrapDelayed(GameObject trap, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetTrapVisible(trap, true);
    }

    void SetTrapVisible(GameObject trap, bool visible)
    {
        // Renderer
        var renderers = trap.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers) r.enabled = visible;

        // Collider
        var colliders = trap.GetComponentsInChildren<Collider>(true);
        foreach (var c in colliders) c.enabled = visible;
    }

    float GetValidSlot(List<float> used)
    {
        int attempts = 30;
        while (attempts-- > 0)
        {
            float candidate = Random.Range(visibleStart, visibleEnd);
            bool valid = true;
            foreach (float usedSlot in used)
            {
                if (Mathf.Abs(candidate - usedSlot) < minSlotSpacing)
                {
                    valid = false;
                    break;
                }
            }
            if (valid) return candidate;
        }
        return -1f;
    }

    Vector3 GetAxisVector(FaceAxis axis, Transform t)
    {
        return axis switch
        {
            FaceAxis.Forward => t.forward,
            FaceAxis.Backward => -t.forward,
            FaceAxis.Left => -t.right,
            FaceAxis.Right => t.right,
            _ => t.forward
        };
    }


    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //public enum FaceAxis
    //{
    //    Forward, Backward, Left, Right
    //}

    //[Header("Spline Settings")]
    //public SplineComputer spline;

    //[Header("Trap Presets")]
    //public List<TrapPreset> trapPresets;

    //[Header("Wave Settings")]
    //public float minSlotSpacing = 0.1f;
    //public float spawnDelayBetweenTraps = 0.2f;

    //private List<float> usedSlots = new();

    //void Start()
    //{
    //    StartCoroutine(SpawnAllTraps());
    //}

    //IEnumerator SpawnAllTraps()
    //{
    //    usedSlots.Clear();

    //    // ✅ Lặp từng preset một
    //    foreach (TrapPreset preset in trapPresets)
    //    {
    //        for (int i = 0; i < preset.spawnCount; i++)
    //        {
    //            float slot = GetValidSlot();
    //            if (slot < 0f) continue;

    //            SplineSample sample = new SplineSample();
    //            spline.Evaluate(slot, ref sample);

    //            GameObject trap = Instantiate(preset.prefab, sample.position, Quaternion.identity);
    //            usedSlots.Add(slot);

    //            // ✅ Set SplineFollower
    //            SplineFollower follower = trap.GetComponent<SplineFollower>();
    //            if (follower == null)
    //            {
    //                Debug.LogError($"❌ Trap prefab '{preset.prefab.name}' thiếu SplineFollower!");
    //                continue;
    //            }

    //            follower.spline = spline;
    //            follower.SetPercent(slot);
    //            follower.followSpeed = Random.Range(preset.speedMin, preset.speedMax);
    //            follower.direction = Random.value < 0.5f ? Spline.Direction.Forward : Spline.Direction.Backward;
    //            follower.wrapMode = SplineFollower.Wrap.PingPong;
    //            follower.follow = true;
    //            follower.motion.applyRotation = false; // Không auto xoay

    //            // ✅ Xoay đúng hướng
    //            trap.transform.rotation = Quaternion.LookRotation(GetAxisVector(preset.faceAxis, trap.transform), Vector3.up);

    //            yield return new WaitForSeconds(spawnDelayBetweenTraps);
    //        }
    //    }

    //    Debug.Log($"✅ Đã spawn {usedSlots.Count} traps từ {trapPresets.Count} presets.");
    //}

    //float GetValidSlot()
    //{
    //    int attempts = 30;
    //    while (attempts-- > 0)
    //    {
    //        float candidate = Random.Range(0.05f, 0.95f);
    //        bool valid = true;

    //        foreach (float used in usedSlots)
    //        {
    //            if (Mathf.Abs(candidate - used) < minSlotSpacing)
    //            {
    //                valid = false;
    //                break;
    //            }
    //        }

    //        if (valid) return candidate;
    //    }

    //    Debug.LogWarning("⚠️ Không tìm được slot phù hợp.");
    //    return -1f;
    //}

    //Vector3 GetAxisVector(FaceAxis axis, Transform t)
    //{
    //    return axis switch
    //    {
    //        FaceAxis.Forward => t.forward,
    //        FaceAxis.Backward => -t.forward,
    //        FaceAxis.Left => -t.right,
    //        FaceAxis.Right => t.right,
    //        _ => t.forward
    //    };
    //}


}
