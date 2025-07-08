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

        float rotationValue = 0;
        rotationValue += Mathf.Sign(h) * 10 * Time.deltaTime;

        if(h != 0)
        controller.transform.rotation = Quaternion.Euler(0, rotationValue, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            controller.RPC_FireGun();
            Object.Destroy(spawnedGun);
            controller.ClearUsingItem();
        }
    }
}
