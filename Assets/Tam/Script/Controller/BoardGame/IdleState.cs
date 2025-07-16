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
        controller.ChangeAnimation("Idle");
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
        if (!controller.isMyTurn) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            controller.RPC_RequestRollDice();
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            controller.RequestChangeState(NewBoardGameController.NetworkState.Item);
        }
    }

    public override string ToString()
    {
        return "Idle";
    }
}
