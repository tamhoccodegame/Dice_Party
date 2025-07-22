using Dreamteck.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//Lưu trữ danh sách enemy.
//Random chọn group.
//Điều khiển từng nhóm chạy theo turn.
public class EnemyWaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public string groupName;
        public List<Wave_AI> enemies = new List<Wave_AI>();
    }

    public List<EnemyGroup> groups = new List<EnemyGroup>();
    public float cycleTime = 5f;
    private int previousGroupIndex = -1;

    private void Start()
    {
        if (groups.Count == 0)
        {
            Debug.LogError("[EnemyWaveManager] ❌ Không có group nào được setup!");
            return;
        }

        // Chạy group đầu tiên ngay khi bắt đầu
        StartCoroutine(CycleGroups());
    }

    private IEnumerator CycleGroups()
    {
        while (true)
        {
            int groupIndex = PickNextGroup();
            if (groupIndex == -1)
            {
                Debug.LogWarning("[EnemyWaveManager] ❗Không tìm thấy group hợp lệ.");
                yield return new WaitForSeconds(cycleTime);
                continue;
            }

            Debug.Log($"[EnemyWaveManager] 🌀 Activating group: {groups[groupIndex].groupName}");

            // Tắt tất cả group
            for (int i = 0; i < groups.Count; i++)
            {
                bool isActive = (i == groupIndex);
                foreach (var enemy in groups[i].enemies)
                {
                    enemy.SetActiveState(isActive);
                }
            }

            previousGroupIndex = groupIndex;

            yield return new WaitForSeconds(cycleTime);
        }
    }

    private int PickNextGroup()
    {
        List<int> validIndices = new List<int>();

        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].enemies.Count >= 2 && i != previousGroupIndex)
            {
                validIndices.Add(i);
            }
        }

        if (validIndices.Count == 0)
        {
            // fallback: nếu không còn group nào hợp lệ (tất cả đều đã dùng hoặc chỉ có 1 enemy), cho phép trùng
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].enemies.Count >= 2)
                {
                    validIndices.Add(i);
                }
            }
        }

        if (validIndices.Count == 0)
        {
            return -1;
        }

        int chosen = validIndices[Random.Range(0, validIndices.Count)];
        return chosen;
    }
}
