using UnityEngine;
public class NoiseRotation : MonoBehaviour
{
    public float speed = 1f;
    public float rotationAmount = 5f;

    private Vector3 baseRotation;

    void Start()
    {
        baseRotation = transform.eulerAngles;
    }

    void Update()
    {
        float time = Time.time * speed;

        float x = (Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(0f, time) - 0.5f) * 2f;
        float z = (Mathf.PerlinNoise(time, time) - 0.5f) * 2f;

        Vector3 noiseRot = new Vector3(x, y, z) * rotationAmount;

        transform.rotation = Quaternion.Euler(baseRotation + noiseRot);
    }
}