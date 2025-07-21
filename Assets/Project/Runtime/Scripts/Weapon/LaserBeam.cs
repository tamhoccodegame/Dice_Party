using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform firePoint;
    public float laserLength = 5f;
    public LayerMask hitMask;

    public NewBoardGameController hitTarget;

    void Update()
    {
        Vector3 start = firePoint.position;
        Vector3 end = start + firePoint.forward * laserLength;

        // Nếu tia trúng vật thể
        if (Physics.Raycast(start, firePoint.forward, out RaycastHit hit, laserLength, hitMask))
        {
            end = hit.point;
            hitTarget = hit.transform.GetComponent<NewBoardGameController>();
        }
        else
        {
            hitTarget = null;
        }

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    public void ApplyDamage()
    {
    }
}
