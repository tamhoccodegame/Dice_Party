using UnityEngine;
using UnityEngine.VFX;

public class ElectricGun : BoardItem
{
    public LaserBeam laserBeam;
    public VisualEffect laserEffect;

    //private void Start()
    //{
        
    //}

    //public override void Spawned()
    //{
    //    laserEffect.Stop();
    //    laserEffect.playRate = 3.5f;
    //}

    //public override void Use(NewBoardGameController controller)
    //{
    //    controller.RequestChangeAnimation("GunAim");

    //    controller.RequestSetItemPosition(0);

    //}

    //public override void Tick(NewBoardGameController controller)
    //{

    //    controller.GetComponent<CharacterController>().enabled = false;

    //    float rotationSpeed = 90f; // độ/giây, quay 90 độ mỗi giây nếu giữ A hoặc D

    //    if (controller.GetInput(out NetworkInputData input))
    //    {
    //        float h = input.direction.x;

    //        if (Mathf.Abs(h) > 0.01f)
    //        {
    //            controller.transform.Rotate(0f, h * rotationSpeed * controller.Runner.DeltaTime, 0f);
    //        }

    //        if (input.buttons.IsSet(NetworkInputData.JUMPBUTTON))
    //        {
    //            controller.RequestTriggerItem();
    //        }
    //    }
    //}

    //public override IEnumerator ProcessCoroutine(NewBoardGameController controller)
    //{
    //    laserEffect.Play();

    //    yield return new WaitForSecondsRealtime(2f);

    //    if (controller.HasInputAuthority && laserBeam.hitTarget != null)
    //        laserBeam.ApplyDamage();

    //    yield return new WaitForSecondsRealtime(1.5f);

    //    controller.RequestChangeState(NewBoardGameController.NetworkState.Idle);
    //    ItemDatabase.instance.ReturnItemPosition(this);
    //}
}
