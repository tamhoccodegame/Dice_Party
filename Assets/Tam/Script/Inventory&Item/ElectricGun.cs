using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ElectricGun : BoardItem
{
    private LaserBeam laserBeam;

    private void Start()
    {
        laserBeam = GetComponent<LaserBeam>();
    }

    public override void Use(NewBoardGameController controller)
    {
        controller.RequestChangeAnimation("GunAim");

        controller.RequestSetItemPosition(0);

        GetComponent<VisualEffect>().Stop();
    }

    public override void Tick(NewBoardGameController controller)
    {

        controller.GetComponent<CharacterController>().enabled = false;

        float rotationSpeed = 90f; // độ/giây, quay 90 độ mỗi giây nếu giữ A hoặc D

        if(controller.GetInput(out NetworkInputData input))
        {
            float h = input.direction.x;

            if (Mathf.Abs(h) > 0.01f)
            {
                controller.transform.Rotate(0f, h * rotationSpeed * controller.Runner.DeltaTime, 0f);
            }

            if(input.buttons.IsSet(NetworkInputData.JUMPBUTTON))
            {
                controller.RequestTriggerItem();
            }
        }
    }

    public override IEnumerator ProcessCoroutine(NewBoardGameController controller)
    {
        GetComponent<VisualEffect>().Play();

        float elapsedTime = 0;
        while(Mathf.FloorToInt(elapsedTime) <= 5f)
        {
            if (controller.HasInputAuthority && laserBeam.hitTarget != null)
            {
                laserBeam.ApplyDamage();
            }
            elapsedTime += 1.5f;
            yield return new WaitForSecondsRealtime(1.5f);
        }
        
        controller.RequestChangeState(NewBoardGameController.NetworkState.Idle);
        ItemDatabase.instance.ReturnItemPosition(this);
    }
}
