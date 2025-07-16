using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepText : MonoBehaviour
{
    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        Destroy(gameObject, 4f);
    }

    public void Init(string currentStep)
    {
        GetComponentInChildren<TextMeshPro>().text = currentStep;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDir = cam.position - transform.position;
        lookDir.y = 0; // nếu chỉ muốn xoay ngang
        transform.forward = lookDir.normalized;

        // Quay lại 180 độ vì TextMesh quay mặt theo -Z
        transform.Rotate(0, 180f, 0);
    }
}
