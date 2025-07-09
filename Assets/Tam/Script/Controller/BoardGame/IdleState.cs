using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BoardState
{
    public IdleState(NewBoardGameController controller) : base(controller)
    {
    }

    public override void Enter()
    {
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
        if(Input.GetKeyDown(KeyCode.Space) && controller.isMyTurn)
        {
            controller.RPC_RequestRollDice();
        }
    }

    public override string ToString()
    {
        return "Idle";
    }
}
