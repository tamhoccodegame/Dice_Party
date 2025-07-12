using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeState : BoardState
{
    public NodeState(NewBoardGameController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        controller.RequestChangeAnimation("Idle");
        controller.currentNode.ProcessNode(controller.Object.InputAuthority, controller.Object.Id);
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
