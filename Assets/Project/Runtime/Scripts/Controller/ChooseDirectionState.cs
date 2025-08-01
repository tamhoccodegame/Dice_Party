using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDirectionState : BoardState
{
    private Vector2 moveInput;

    float inputCooldown = 0.25f;
    float inputTimer;

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
        moveInput = controller.playerInput.actions["Move"].ReadValue<Vector2>();

        inputTimer -= Time.deltaTime;

        if(inputTimer <= 0)
        {
            if (moveInput.x > 0.5)
            {
                controller.NextHoverArrow();
                inputTimer = inputCooldown;
            }
            else if (moveInput.x < -0.5f)
            {
                controller.PrevHoverArrow();
                inputTimer = inputCooldown;
            }
        }

        if (controller.playerInput.actions["Trigger"].triggered)
        {
            controller.ChooseDirection();
        }

    }

    public override string ToString()
    {
        return "Idle";
    }
}
