using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpawnObject : MonoBehaviour
{
    public GameObject[] objects;
    public Transform spawnPosition;

    public float spawnInterval;
    public float speed = 100f;

    public void Start()
    {
        InvokeRepeating(nameof(SpawnObject), 0, spawnInterval);
    }

    void SpawnObject()
    {
        GameObject obj = objects[Random.Range(0, objects.Length)];
        var go = Instantiate(obj, spawnPosition.position, spawnPosition.rotation).GetComponent<Rigidbody>();
        Vector3 newPos = obj.transform.position;
        newPos.x = go.transform.position.x;
        newPos.z = go.transform.position.z;
        go.transform.position = newPos;

        go.velocity = go.transform.forward * speed;
        Destroy(go.gameObject, 5f);
    }
}
