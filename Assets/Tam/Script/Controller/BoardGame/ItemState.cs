using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemState : BoardState
{
    public ItemState(NewBoardGameController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        controller.RequestSetUsingItem(0);
    }

    public override void Exit()
    {
    }

    public override void FixedUpdateNetwork()
    {
        if(controller.currentItem != null)
        {
            controller.currentItem.Tick(controller);
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
        return "Item";
    }
}
