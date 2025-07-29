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
        controller.ChangeAnimation("Idle");
        controller.currentNode.ProcessNode(controller.playerInput, controller.transform);
    }

    public override void Exit()
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
