using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDir = Camera.main.transform.position - transform.position;
        lookDir.y = 0; // nếu chỉ muốn xoay ngang
        transform.forward = lookDir.normalized;

        // Quay lại 180 độ vì TextMesh quay mặt theo -Z
        transform.Rotate(0, 180f, 0);
    }
}
