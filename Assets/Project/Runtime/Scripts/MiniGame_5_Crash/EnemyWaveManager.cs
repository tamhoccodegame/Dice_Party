using Dreamteck.Splines;
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
    public List<GameObject> horizontalEnemies = new List<GameObject>();
    public List<GameObject> verticalEnemies = new List<GameObject>();
    public int minGroup = 2, maxGroup = 4;
    public float speedMin = 2f, speedMax = 5f;

    private List<GameObject> current = new List<GameObject>();

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
        foreach (var e in current) e.SetActive(false);
        current.Clear();

        int count = Random.Range(minGroup, maxGroup + 1);
        int vertCount = Random.Range(1, count);
        int horCount = count - vertCount;

        var verts = GetValidEnemies(verticalEnemies, vertCount, EnemyDirection.Vertical);
        var hors = GetValidEnemies(horizontalEnemies, horCount, EnemyDirection.Horizontal);
        current.AddRange(verts); current.AddRange(hors);

        foreach (var go in current)
        {
            go.SetActive(true);
            var ai = go.GetComponent<Wave_AI>();
            float speed = Random.Range(speedMin, speedMax);
            //ai.StartMoving(speed, OnFinished);
        }
    }

    void OnFinished(GameObject go)
    {
        go.SetActive(false);
        current.Remove(go);
        if (current.Count == 0) StartCoroutine(NextGroup());
    }

    List<GameObject> GetValidEnemies(List<GameObject> list, int needed, EnemyDirection dir)
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
                var ai1 = pick.GetComponent<Wave_AI>();
                var ai2 = e.GetComponent<Wave_AI>();
                // nếu cùng chiều và trên cùng line => conflict
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
}
