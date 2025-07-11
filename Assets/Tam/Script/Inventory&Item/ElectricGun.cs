using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ElectricGun : BoardItem
{
    private GameObject spawnedGun;
    public override void Use(NewBoardGameController controller)
    {
        if (!controller.HasInputAuthority) return;
        controller.RPC_ChangeAnimation("GunAim");
        itemPrefab = ItemDatabase.instance.GetItemPrefab("ElectricGun");

        spawnedGun = Object.Instantiate(itemPrefab,
            controller.gunSpawnPoint.position,
            controller.gunSpawnPoint.rotation,
            controller.gunSpawnPoint);
        spawnedGun.GetComponent<VisualEffect>().Stop();
    }

    public override void Tick(NewBoardGameController controller)
    {
        controller.GetComponent<CharacterController>().enabled = false;

        float h = Input.GetAxisRaw("Horizontal");

        float rotationSpeed = 90f; // độ/giây, quay 90 độ mỗi giây nếu giữ A hoặc D

        if (Mathf.Abs(h) > 0.01f)
        {
            controller.transform.Rotate(0f, h * rotationSpeed * Time.deltaTime, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            controller.StartCoroutine(FireGunCoroutine(controller));
        }
    }

    IEnumerator FireGunCoroutine(NewBoardGameController controller)
    {
        spawnedGun.GetComponent<VisualEffect>().Play();
        yield return new WaitForSecondsRealtime(5f);
        Object.Destroy(spawnedGun);
        controller.ChangeState(controller.idleState);
    }
}
