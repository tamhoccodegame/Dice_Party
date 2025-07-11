using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ElectricGun : BoardItem
{
    public override void Use(NewBoardGameController controller)
    {
        if (!controller.HasInputAuthority) return;

        //controller.RequestChangeAnimation("GunAim");

        controller.RPC_SetItemPosition(0);

        GetComponent<VisualEffect>().Stop();
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
            controller.RPC_RequestTriggerItem();
        }
    }

    public override IEnumerator ProcessCoroutine(NewBoardGameController controller)
    {
        GetComponent<VisualEffect>().Play();
        yield return new WaitForSecondsRealtime(5f);
        controller.ChangeState(controller.idleState);
        ItemDatabase.instance.ReturnItemPosition(this);
    }
}
