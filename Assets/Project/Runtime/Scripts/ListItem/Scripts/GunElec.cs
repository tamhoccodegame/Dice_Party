using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GunElec : BoardItem, IRotatableItem
{
    [SerializeField] float rotateSpeed = 90f;
    public GameObject gunPrefab;
    public Transform gunHoldPoint;

    private Transform playerModel;
    private GameObject currentGun;
    private VisualEffect shootVFX;
    private bool isUsingGun = false;
    private bool isShooting = false;
    private NewBoardGameController controller;

    public override void Use(NewBoardGameController controller)
    {
        this.controller = controller;
        gunHoldPoint = controller.handSpawnPoint;
        playerModel = controller.GetComponent<Animator>().transform;
        
        if (gunHoldPoint == null)
        {
            Debug.LogError("GunSpawnPoint not assigned in controller!");
            return;
        }
        controller.StartCoroutine(HandleGunUsage());
    }

    private IEnumerator HandleGunUsage()
    {
        currentGun = Instantiate(gunPrefab, gunHoldPoint.position, gunHoldPoint.rotation, gunHoldPoint);
        isUsingGun = true;
        shootVFX = currentGun.GetComponentInChildren<VisualEffect>();
        if (shootVFX != null) shootVFX.Stop();

        while (isUsingGun)
        {
            if (Input.GetMouseButton(0))
            {
                yield return Shoot();
            }

            yield return null;
        }
        isUsingGun = false;
        Destroy(currentGun);
    }
    public IEnumerator Shoot()
    {
        isShooting = true;
        if (shootVFX != null && !shootVFX.aliveParticleCount.Equals(0))
        {
            shootVFX.Play();
            yield return new WaitForSeconds(5f);
        }
        isShooting = false;
        isUsingGun = false;
    }
    public void Rotate(float direction)
    {
        if (isShooting)
            return;
        if (playerModel != null)
        {
            playerModel.Rotate(Vector3.up * direction * rotateSpeed * Time.deltaTime);
        }
    }
}
