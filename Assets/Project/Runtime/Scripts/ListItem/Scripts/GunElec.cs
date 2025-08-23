using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class GunElec : BoardItem
{
    [SerializeField] float rotateSpeed = 90f;
    public GameObject gunPrefab;
    public Transform gunHoldPoint;

    private Transform playerModel;
    private GameObject currentGun;
    private VisualEffect shootVFX;
    public LaserBeam laserBeam;
    private bool isUsingGun = false;
    private NewBoardGameController controller;

    public override void Use(NewBoardGameController controller)
    {
        this.controller = controller;
        gunHoldPoint = controller.gunSpawnPoint;
        playerModel = controller.GetComponent<Animator>().transform;
        

        if (gunHoldPoint == null)
        {
            Debug.LogError("GunSpawnPoint not assigned in controller!");
            //controller.EndTurn();
            return;
        }
        controller.StartCoroutine(HandleGunUsage());
    }

    private IEnumerator HandleGunUsage()
    {
        controller.ChangeAnimation("GunAim");
        currentGun = Instantiate(gameObject, gunHoldPoint.position, gunHoldPoint.rotation, gunHoldPoint);
        currentGun.transform.localRotation = Quaternion.Euler(180, 90, 0);
        isUsingGun = true;

        shootVFX = currentGun.GetComponentInChildren<VisualEffect>();
        if (shootVFX != null) shootVFX.Stop();

        while (isUsingGun)
        {
            RotatePlayerToKeyboard();
            NewBoardGameController target = currentGun.GetComponent<LaserBeam>().hitTarget;

            yield return null;
            if (controller.playerInput.actions["Trigger"].triggered)
            {
                if (shootVFX != null)
                {
                    shootVFX.Play();
                    yield return new WaitForSeconds(5f);
                    {
                        target.enabled = true;
                        target.readyForInput = false;
                        target.EnableRagdoll();
                        WizardPartyData.instance.UpdatePlayerHealth(target.playerInput, -10);
                        TurnManager.instance.UpdatePlayerDataUI();
                        Debug.Log("Enable Ragdoll");
                    }
                }

                isUsingGun = false;
            }

            yield return null;
        }
        isUsingGun = false;
        Destroy(currentGun);

        controller.ChangeState(controller.idleState);
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
