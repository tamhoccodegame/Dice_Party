using System.Collections;
using System.Collections.Generic;
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

    public override void FixedUpdateNetwork()
    {
        if (!controller.MoveStep())
        {
            controller.ChangeState(controller.nodeState);
        }
    }

    public override void HandleInput()
    {
    }

    public override void Update()
    {
    }

    public override string ToString()
    {
        return "Run";
    }
}
