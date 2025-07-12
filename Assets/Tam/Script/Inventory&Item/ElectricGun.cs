using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ElectricGun : BoardItem
{
    public override void Use(NewBoardGameController controller)
    {
        controller.RequestChangeAnimation("GunAim");

        controller.RequestSetItemPosition(0);

        GetComponent<VisualEffect>().Stop();
    }

    public override void Tick(NewBoardGameController controller)
    {

        controller.GetComponent<CharacterController>().enabled = false;

        if(GetInput(out NetworkInputData inputData))
        {
            float rotationSpeed = 90f; // độ/giây, quay 90 độ mỗi giây nếu giữ A hoặc D

            Vector2 direction = inputData.direction;

            if (direction.x != 0)
            {
                controller.transform.Rotate(0f, direction.x * rotationSpeed * Time.deltaTime, 0f);
            }
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
        controller.RequestChangeState(NewBoardGameController.NetworkState.Idle);
        ItemDatabase.instance.ReturnItemPosition(this);
    }
}
