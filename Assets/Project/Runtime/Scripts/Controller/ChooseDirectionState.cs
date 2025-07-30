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

    public override void Update()
    {
        if (controller.playerInput.actions["PrimaryButton"].triggered)
        {
            controller.ChooseDirection(0);
        }
        else if (controller.playerInput.actions["SecondaryButton"].triggered)
        {
            controller.ChooseDirection(1);
        }

    }

    public override string ToString()
    {
        return "Idle";
    }
}
