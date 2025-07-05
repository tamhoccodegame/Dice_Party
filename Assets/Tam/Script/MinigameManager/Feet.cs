using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour
{
    private BoxCollider box;

    private void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (box == null) return;

        Vector3 center = box.transform.TransformPoint(box.center);
        Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);
        Quaternion rotation = box.transform.rotation;

        // Ví dụ: kiểm tra có ai trong vùng chân
        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation, LayerMask.GetMask("Ground"));

        if (hits.Length > 0)
        {
            hits[0].GetComponent<BreakGlass>().TryBreak();
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (box == null) box = GetComponent<BoxCollider>();
    //    if (box == null) return;

    //    Vector3 center = box.transform.TransformPoint(box.center);
    //    Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);
    //    Quaternion rotation = box.transform.rotation;

    //    Gizmos.color = Color.red;
    //    Gizmos.matrix = Matrix4x4.TRS(center, rotation, halfExtents * 2);
    //    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    //}
}
