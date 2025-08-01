using UnityEngine;

public class MovingState : BoardState
{
    public MovingState(NewBoardGameController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        controller.ChangeAnimation("Run");
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (!controller.MoveStep())
        {
            controller.ChangeState(controller.nodeState);
        }
    }

    public override string ToString()
    {
        return "Run";
    }
}
