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
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (controller.playerInput.actions["Trigger"].triggered && !isRoll && controller.readyForInput)
        {
            isRoll = true;
            controller.RollDice();
        }
    }

    public override string ToString()
    {
        return "Idle";
    }
}
