using UnityEngine;

public class Dice : MonoBehaviour
{
    public float rotateSpeed = 100f;

    public void Update()
    {
        transform.Rotate(Vector3.one * rotateSpeed * Time.deltaTime);
    }
}
