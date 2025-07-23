using PlasticGui.WebApi.Responses;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class T_TireSpawner_MNGNhay : MonoBehaviour
{
    public GameObject tirePrefab;
    public Transform spawnPosition;
    public Dictionary<float, float> sizeAndSpeed = new Dictionary<float, float>
    {
        { 6f, 180f },
        { 7f, 170f },
        { 8f, 150f },
        { 9f, 130f }
    };

    public float spawnInterval = 3f;

    public float timeToChangeSpawnInterval = 6f;
    public float lasTimeChangeInterval;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(SpawnTire), 0f, spawnInterval);
    }

    void SpawnTire()
    {
        int index = Random.Range(0, sizeAndSpeed.Count);
        float newSize = sizeAndSpeed.ElementAt(index).Key;
        float newSpeed = sizeAndSpeed.ElementAt(index).Value;
        var go = Instantiate(tirePrefab, spawnPosition.position, Quaternion.identity);
        go.transform.localScale = new Vector3(newSize, newSize, newSize);
        go.GetComponent<T_Tire_MNGNhay>().rollSpeed = newSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lasTimeChangeInterval >= timeToChangeSpawnInterval)
        {
            lasTimeChangeInterval = Time.time;
            spawnInterval = Random.Range(2f, 5f);
            CancelInvoke();
            InvokeRepeating(nameof(SpawnTire), 2f, spawnInterval);
        }
    }
}
