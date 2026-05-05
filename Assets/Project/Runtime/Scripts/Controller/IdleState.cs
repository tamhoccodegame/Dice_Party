using UnityEngine;

public class IdleState : BoardState
{
    bool isRoll = false;
    public IdleState(NewBoardGameController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        controller.ChangeAnimation("Idle");
        controller.splineAnimate.Pause();
        if (controller == TurnManager.instance.playerControllers[controller.playerInput])
            controller.dice.SetActive(true);
    }

    public override void Exit()
    {
        controller.dice.SetActive(false);
    }

    public override void Update()
    {
        if (controller.playerInput.actions["Trigger"].triggered && !isRoll && controller.readyForInput)
        {
            isRoll = true;
            controller.RollDice();
        }
        else if (controller.playerInput.actions["UseItem"].triggered && !isRoll && controller.readyForInput)
        {
            controller.ChangeState(controller.itemState);
        }
    }

    public override string ToString()
    {
        return "Idle";
    }
}
