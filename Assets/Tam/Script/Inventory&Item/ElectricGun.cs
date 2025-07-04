using UnityEngine;

public class ElectricGun : BoardItem
{
    private GameObject spawnedGun;
    public override void Use(BoardGameController controller)
    {
        if (!controller.HasInputAuthority) return;

        itemPrefab = ItemDatabase.instance.GetItemPrefab("ElectricGun");

        spawnedGun = Object.Instantiate(itemPrefab,
            controller.gunSpawnPoint.position,
            controller.gunSpawnPoint.rotation,
            controller.gunSpawnPoint);

        controller.SetUsingItem(this);
    }

    public override void Tick(BoardGameController controller)
    {
        controller.GetComponent<CharacterController>().enabled = false;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v);

        if (inputDir.sqrMagnitude > 0.1f)
        {
            controller.transform.forward = inputDir;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            controller.RPC_FireGun();
            Object.Destroy(spawnedGun);
            controller.ClearUsingItem();
        }
    }
}
