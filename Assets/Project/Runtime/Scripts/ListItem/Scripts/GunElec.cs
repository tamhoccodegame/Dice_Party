using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GunElec : BoardItem
{
    [SerializeField] float rotateSpeed = 90f;
    public GameObject gunPrefab;
    public Transform gunHoldPoint;

    private Transform playerModel;
    private GameObject currentGun;
    private VisualEffect shootVFX;
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
            controller.EndTurn();
            return;
        }
        controller.StartCoroutine(HandleGunUsage());
    }

    private IEnumerator HandleGunUsage()
    {
        currentGun = Instantiate(gunPrefab, gunHoldPoint.position, gunHoldPoint.rotation, gunHoldPoint);
        currentGun.transform.localRotation = Quaternion.Euler(180, 90, 0);
        isUsingGun = true;

        shootVFX = currentGun.GetComponentInChildren<VisualEffect>();
        if (shootVFX != null) shootVFX.Stop();

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        while (isUsingGun)
        {
            RotatePlayerToKeyboard();
            yield return null;
            if (Input.GetMouseButtonDown(0))
            {
                if (shootVFX != null)
                {
                    shootVFX.Play();
                    yield return new WaitForSeconds(5f);
                }

                isUsingGun = false;
            }

            yield return null;
        }
        isUsingGun = false;
        Destroy(currentGun);

        controller.EndTurn();
    }

    void RotatePlayerToKeyboard()
    {
        float rotateInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            rotateInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            rotateInput = 1f;

        if (rotateInput != 0f && playerModel != null)
        {
            playerModel.Rotate(Vector3.up, rotateSpeed * rotateInput * Time.deltaTime);
        }
        Debug.Log(rotateInput);
    }
}
