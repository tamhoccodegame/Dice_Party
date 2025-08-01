using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GunElec : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 90f;

    public GameObject gunPrefab;
    public Transform gunHoldPoint;
    public GameObject player;
    public CharacterController controller;
    private VisualEffect shootVFX;
    private GameObject currentGun;
    private bool isUsingGun = false;

    
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isUsingGun)
            {
                currentGun = Instantiate(gunPrefab, gunHoldPoint.position, gunHoldPoint.rotation, gunHoldPoint);
                currentGun.transform.localRotation = Quaternion.Euler(180, 90, 0);
                isUsingGun = true;

                shootVFX = currentGun.GetComponentInChildren<VisualEffect>();
                if (shootVFX != null)
                {
                    shootVFX.Stop();
                }

                if (controller != null )
                    controller.enabled = false;
            }

        }
        if (isUsingGun)
        {
            RotatePlayerToMouse();
        }
        if (isUsingGun && Input.GetMouseButtonDown(0))
        {
            if (shootVFX != null)
            {
                shootVFX.Play();
            }
        }
    }
    void RotatePlayerToMouse()
    {
        float rotateInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            rotateInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            rotateInput = 1f;

        if (rotateInput != 0f)
        {
            player.transform.Rotate(Vector3.up, rotateSpeed * rotateInput * Time.deltaTime);
        }
    }
}
