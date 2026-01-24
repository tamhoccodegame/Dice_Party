using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float amplitude = 10f;
    public float speed = 0.01f;
    float startY;
    float offset;

    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
        offset = Random.Range(0, 100);
        amplitude *= Random.Range(0.8f, 1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        float y = startY + Mathf.Sin(Time.time * speed * offset) * amplitude;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
