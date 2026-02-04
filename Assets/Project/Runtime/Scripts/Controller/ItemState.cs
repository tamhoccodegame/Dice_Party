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
        controller.itemController.enabled = true;
        controller.itemController.UseItem(1);
    }

    public override void Exit()
    {
        controller.itemController.enabled = false;
        controller.dice.SetActive(true);
    }

    public override void Update()
    {
        
    }

    public override string ToString()
    {
        return "Item";
    }
}
