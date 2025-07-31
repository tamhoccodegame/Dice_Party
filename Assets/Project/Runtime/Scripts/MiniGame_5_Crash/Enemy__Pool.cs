using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy__Pool : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int poolSize = 30;

    private Queue<Enemy__Controller> pool = new Queue<Enemy__Controller>();

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj.GetComponent<Enemy__Controller>());
        }
    }

    public Enemy__Controller Get()
    {
        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            return obj.GetComponent<Enemy__Controller>();
        }
        return pool.Dequeue();
    }

    public void Return(Enemy__Controller enemy)
    {
        enemy.Deactivate();
        pool.Enqueue(enemy);
    }
}
