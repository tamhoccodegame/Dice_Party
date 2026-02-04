using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class GunElec : BoardItem
{
    [SerializeField] float rotateSpeed = 90f;

    public GameObject bulletPrefab;

    private Transform playerModel;
    public LaserBeam laserBeam;
    private NewBoardGameController controller;

    public override void Init(NewBoardGameController controller)
    {
        this.controller = controller;
        controller.ChangeAnimation("GunAim");
        playerModel = controller.transform;
    }

    public override void Use()
    {
        float rotateInput = controller.playerInput.actions["Move"].ReadValue<Vector2>().x;

        if (rotateInput != 0f && playerModel != null)
        {
            playerModel.Rotate(Vector3.up, rotateSpeed * rotateInput * Time.deltaTime);
        }

        if (controller.playerInput.actions["Trigger"].triggered)
        {
            Debug.Log("Fire!!");
            //Spawn Bullet
            var bullet = Instantiate(bulletPrefab, laserBeam.firePoint.position, laserBeam.firePoint.rotation).GetComponent<Rigidbody>();
            bullet.GetComponent<GunItemBullet>().Init(20);
            bullet.transform.up = -laserBeam.transform.forward;
            bullet.AddForce(laserBeam.firePoint.transform.forward * 150f, ForceMode.Impulse);

            itemEndUse?.Invoke();
        }
    }
}
