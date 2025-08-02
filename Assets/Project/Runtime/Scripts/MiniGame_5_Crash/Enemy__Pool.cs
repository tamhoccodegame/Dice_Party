using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy__Pool : MonoBehaviour
{
    [Header("Pool Config")]
    public GameObject enemyPrefab;
    public int poolSize = 30;

    private Queue<Enemy__Controller> pool = new Queue<Enemy__Controller>();

    void Awake()
    {
        // Tạo sẵn pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            obj.SetActive(false);

            Enemy__Controller controller = obj.GetComponent<Enemy__Controller>();
            controller.SetPool(this);
            pool.Enqueue(controller);
        }
    }

    public Enemy__Controller Get()
    {
        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            obj.SetActive(false);

            Enemy__Controller controller = obj.GetComponent<Enemy__Controller>();
            controller.SetPool(this);
            return controller;
        }

        Enemy__Controller enemy = pool.Dequeue();
        enemy.gameObject.SetActive(true);
        return enemy;
    }

    public void Return(Enemy__Controller enemy)
    {
        enemy.Deactivate();
        pool.Enqueue(enemy);
    }
}
