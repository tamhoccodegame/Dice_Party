using PlasticGui.WebApi.Responses;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class T_TireSpawner_MNGNhay : MonoBehaviour
{
    public GameObject tirePrefab;
    public Transform leftSpawnPosition;
    public Transform rightSpawnPosition;

    public Dictionary<float, float> sizeAndSpeed = new Dictionary<float, float>
    {
        { 9f, 320f },
        { 8f, 310f },
        { 7f, 300f },
        { 6f, 290f }
    };

    public List<GameObject> activeObjects = new List<GameObject>();

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
        if (activeObjects.Count >= 5) return;
        int index = Random.Range(0, sizeAndSpeed.Count);
        float newSize = sizeAndSpeed.ElementAt(index).Key;
        float newSpeed = sizeAndSpeed.ElementAt(index).Value;
        int randomIndex = Random.Range(0, 2);
        Vector3 spawnPosition = randomIndex == 0 ? leftSpawnPosition.position : rightSpawnPosition.position;

        var go = Instantiate(tirePrefab, spawnPosition, Quaternion.identity);
        go.transform.localScale = new Vector3(newSize, newSize, newSize);

        go.GetComponent<T_Tire_MNGNhay>().rollSpeed = newSpeed;
        go.GetComponent<T_Tire_MNGNhay>().Init(randomIndex == 0 ? 1 : -1, () => activeObjects.Remove(go));
        activeObjects.Add(go);
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
