using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpawnObject : MonoBehaviour
{
    public GameObject[] objects;
    public Transform spawnPosition;

    public float spawnInterval;

    public void Start()
    {
        InvokeRepeating(nameof(SpawnObject), 0, spawnInterval);
    }

    void SpawnObject()
    {
        var go = Instantiate(objects[Random.Range(0, objects.Length)], spawnPosition.position, spawnPosition.rotation).GetComponent<Rigidbody>();
        go.velocity = go.transform.forward * 80f;
        Destroy(go.gameObject, 5f);
    }
}
