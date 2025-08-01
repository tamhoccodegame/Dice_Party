using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GunElec : MonoBehaviour
{
    public GameObject gunPrefab;         
    public Transform gunHoldPoint;
    public GameObject player;

    private GameObject currentGun;
    private VisualEffect shootVFX;

    private bool isUsingGun = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isUsingGun)
        {
            EquipGun();
        }

        if (isUsingGun && currentGun != null)
        {
            RotateGunToMouse();

            if (Input.GetMouseButtonDown(0))
            {
                shootVFX?.SendEvent("OnPlay");
            }
        }
    }

    void EquipGun()
    {
        currentGun = Instantiate(gunPrefab, gunHoldPoint.position, Quaternion.Euler(0,0,0), gunHoldPoint);
        shootVFX = currentGun.GetComponentInChildren<VisualEffect>();
        isUsingGun = true;
    }

    void RotateGunToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, currentGun.transform.position);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookDir = hitPoint - currentGun.transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.01f)
            {
                Quaternion rotation = Quaternion.LookRotation(lookDir);
                currentGun.transform.rotation = rotation;
            }
        }
    }
}
