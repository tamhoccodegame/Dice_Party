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
        controller.RequestChangeAnimation("Run");
    }

    public override void Exit()
    {
    }

    public override void FixedUpdateNetwork()
    {
        if (!controller.MoveStep())
        {
            controller.RequestChangeState(NewBoardGameController.NetworkState.Node);
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
