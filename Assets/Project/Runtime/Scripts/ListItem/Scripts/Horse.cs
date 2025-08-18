using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : BoardItem
{
    public GameObject horsePrefab;
    public Transform spawnPoint;
    public Transform player;

    private bool isUsing = false;
    private bool isAttack = false;

    private GameObject currentHorse;
    private NewBoardGameController controller;

    public override void Use(NewBoardGameController controller)
    {
        this.controller = controller;
        spawnPoint = controller.horseSpawnPoint;
        player = controller.transform;
        if (spawnPoint == null)
        {
            Debug.LogError("HorseSpawnPoint not assigned in controller!");
            return;
        }
        controller.StartCoroutine(SpawnHorse());
    }
    public IEnumerator SpawnHorse()
    {
        if (currentHorse != null) yield break;

        currentHorse = Instantiate(horsePrefab, spawnPoint.position + new Vector3(0, -2, 0), spawnPoint.rotation);


        yield return new WaitForSeconds(0.5f);

        Transform mountPoint = currentHorse.transform.Find("MountPoint");
        if (mountPoint == null)
        {
            Debug.LogError("Ngựa chưa có MountPoint!");
            yield break;
        }

        // Đặt player vào ngồi
        player.SetParent(mountPoint);
        player.localPosition = Vector3.zero;
        player.localRotation = Quaternion.identity;
        player.GetComponent<PlayerController>().enabled = false;
        isUsing = true;

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        while (isUsing)
        {
            if (Input.GetMouseButtonDown(0) && !isAttack)
            {
                yield return controller.StartCoroutine(HorseHandle());
            }
            yield return null;
        }
        player.SetParent(null);
        player.GetComponent<PlayerController>().enabled = true;
        Destroy(currentHorse, 2f);
    }
    private IEnumerator HorseHandle()
    {
        isAttack = true;

        Debug.Log("Ngựa tấn công!");

        yield return new WaitForSeconds(1f);

        isAttack = false;
        Debug.Log("Ngựa dừng tấn công");
    }

}
