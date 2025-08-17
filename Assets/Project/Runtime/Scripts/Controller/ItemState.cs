using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemState : BoardState
{
    public ItemState(NewBoardGameController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering ItemState");

        if (controller.gun != null)
        {
            controller.gun.Use(controller);
        }
        else
        {
            Debug.LogWarning("No item found in controller.");
            controller.ChangeState(controller.idleState);
        }
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }

    public override string ToString()
    {
        return "Item";
    }
}
