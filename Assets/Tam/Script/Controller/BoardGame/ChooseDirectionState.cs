using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDirectionState : BoardState
{
    public ChooseDirectionState(NewBoardGameController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        controller.ChangeAnimation("Idle");
        controller.ShowDirectionChoices();
    }

    public override void Exit()
    {
    }

    public override void FixedUpdateNetwork()
    {
    }

    public override void HandleInput()
    {
    }

    public override void Update()
    {
    }

    public override string ToString()
    {
        return "Idle";
    }
}
