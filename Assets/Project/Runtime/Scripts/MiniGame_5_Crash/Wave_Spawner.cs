using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wave_Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemySchedule
    {
        public GameObject enemy;
        public float delay;
        public float speed;
        public float arrivalTime;
    }

    public List<GameObject> horizontalEnemies = new List<GameObject>();
    public List<GameObject> verticalEnemies = new List<GameObject>();

    public int minGroup = 2, maxGroup = 5;
    public float speedMin = 1f, speedMax = 5f;
    public float safeGap = 1f;      // thời gian cách nhau an toàn
    public float maxDelay = 1.5f;

    private List<GameObject> current = new List<GameObject>();
    private Vector3 centerPoint = Vector3.zero;

    void Start()
    {
        var all = FindObjectsOfType<Wave_AI>(true);
        foreach (var e in all)
        {
            if (e.direction == EnemyDirection.Horizontal)
                horizontalEnemies.Add(e.gameObject);
            else
                verticalEnemies.Add(e.gameObject);

            e.gameObject.SetActive(false);
            e.tag = "Enemy";
        }

        StartCoroutine(NextGroup());
    }

    IEnumerator NextGroup()
    {
        yield return new WaitForSeconds(1f);
        SpawnGroup();
    }

    void SpawnGroup()
    {
        // clear group cũ
        foreach (var e in current) e.SetActive(false);
        current.Clear();

        int count = Random.Range(minGroup, maxGroup + 1);
        int vertCount = Mathf.Max(1, Random.Range(1, count));
        int horCount = count - vertCount;
        if (horCount <= 0) horCount = 1;

        var verts = SelectValidEnemies(verticalEnemies, vertCount, EnemyDirection.Vertical);
        var hors = SelectValidEnemies(horizontalEnemies, horCount, EnemyDirection.Horizontal);
        current.AddRange(verts);
        current.AddRange(hors);

        Debug.Log($"[WaveManager] New group: Vert={verts.Count}, Hor={hors.Count}");

        // Tính toán lịch di chuyển an toàn
        var schedule = CalculateSchedule(current);

        // Bắt đầu di chuyển với delay + speed riêng
        foreach (var s in schedule)
        {
            s.enemy.SetActive(true);
            var ai = s.enemy.GetComponent<Wave_AI>();
            ai.StartMoving(s.speed, OnFinished);
        }
    }

    void OnFinished(GameObject go)
    {
        go.SetActive(false);
        current.Remove(go);
        Debug.Log($"[WaveManager] Enemy finished: {go.name}, Remaining={current.Count}");
        if (current.Count == 0) StartCoroutine(NextGroup());
    }

    List<GameObject> SelectValidEnemies(List<GameObject> list, int needed, EnemyDirection dir)
    {
        var temp = new List<GameObject>(list);
        var valid = new List<GameObject>();

        while (valid.Count < needed && temp.Count > 0)
        {
            var pick = temp[Random.Range(0, temp.Count)];
            temp.Remove(pick);

            bool conflict = false;
            foreach (var e in valid)
            {
                if (dir == EnemyDirection.Horizontal &&
                    Mathf.Abs(pick.transform.position.z - e.transform.position.z) < 0.1f)
                    conflict = true;

                if (dir == EnemyDirection.Vertical &&
                    Mathf.Abs(pick.transform.position.x - e.transform.position.x) < 0.1f)
                    conflict = true;
            }

            if (!conflict) valid.Add(pick);
        }

        return valid;
    }

    List<EnemySchedule> CalculateSchedule(List<GameObject> enemies)
    {
        List<EnemySchedule> result = new List<EnemySchedule>();

        // Tạo schedule ban đầu
        foreach (var go in enemies)
        {
            float speed = Random.Range(speedMin, speedMax);
            float delay = Random.Range(0f, 0.5f);
            float dist = Vector3.Distance(go.transform.position, centerPoint);

            result.Add(new EnemySchedule
            {
                enemy = go,
                speed = speed,
                delay = delay,
                arrivalTime = delay + dist / speed
            });
        }

        bool conflict;
        int loop = 0;
        do
        {
            conflict = false;
            loop++;

            for (int i = 0; i < result.Count; i++)
            {
                for (int j = i + 1; j < result.Count; j++)
                {
                    var a = result[i];
                    var b = result[j];

                    // Nếu arrival gần nhau → chỉnh
                    if (Mathf.Abs(a.arrivalTime - b.arrivalTime) < safeGap)
                    {
                        conflict = true;
                        var adjust = (a.arrivalTime < b.arrivalTime) ? a : b;

                        if (adjust.delay < maxDelay)
                        {
                            adjust.delay += 0.3f; // ưu tiên tăng delay
                        }
                        else
                        {
                            adjust.speed = Mathf.Max(speedMin, adjust.speed - 0.5f); // giảm speed nếu delay max
                        }

                        float distAdj = Vector3.Distance(adjust.enemy.transform.position, centerPoint);
                        adjust.arrivalTime = adjust.delay + distAdj / adjust.speed;

                        // Update lại list
                        if (a.arrivalTime < b.arrivalTime) result[i] = adjust;
                        else result[j] = adjust;
                    }
                }
            }

        } while (conflict && loop < 50);

        return result;
    }
}
