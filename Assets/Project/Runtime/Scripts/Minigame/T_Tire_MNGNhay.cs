using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_Tire_MNGNhay : MonoBehaviour
{
    public float rollSpeed;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -rollSpeed * Time.deltaTime);
        rb.velocity = Vector3.right * rollSpeed / 10;
    }
}
