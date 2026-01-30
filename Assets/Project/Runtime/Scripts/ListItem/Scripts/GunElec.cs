using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class GunElec : BoardItem
{
    [SerializeField] float rotateSpeed = 90f;

    private Transform playerModel;
    private GameObject currentGun;
    private VisualEffect shootVFX;
    public LaserBeam laserBeam;
    private bool isUsingGun = false;
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
            itemEndUse?.Invoke();
        }
    }

    void RotatePlayerToKeyboard()
    {
        float rotateInput = controller.playerInput.actions["Move"].ReadValue<Vector2>().x;

        if (rotateInput != 0f && playerModel != null)
        {
            playerModel.Rotate(Vector3.up, rotateSpeed * rotateInput * Time.deltaTime);
        }
    }
}
