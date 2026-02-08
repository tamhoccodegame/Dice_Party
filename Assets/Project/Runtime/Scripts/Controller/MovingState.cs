using UnityEngine;

public class MovingState : BoardState
{
    public int stepLeft;

    public MovingState(NewBoardGameController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        controller.chooseDirectionState.onDirectionChose += OnDirectionChose;
        controller.ChangeAnimation("Run");
    }

    void OnDirectionChose()
    {
        controller.ChangeState(controller.movingState);
        controller.chooseDirectionState.onDirectionChose -= OnDirectionChose;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (stepLeft > 0)
            MoveStep();
        else
            controller.ChangeState(controller.nodeState);
    }

    public void MoveStep()
    {
        if (controller.toMoveNode == null || stepLeft <= 0) return;

        controller.moveDir = (controller.toMoveNode.transform.position - controller.feet.position).normalized;
        controller.moveDir.y = 0;

        if (Vector3.Distance(controller.feet.position, controller.toMoveNode.transform.position) < 0.3f)
        {
            controller.moveDir = Vector3.zero;
            controller.currentNode = controller.toMoveNode;
            WizardPartyData.instance.UpdatePlayerNode(controller.playerInput, controller.currentNode);
            stepLeft--;

            if (controller.currentNode is ChestGoldNode chest)
            {
                controller.ChangeState(controller.nodeState);
            }

            if (stepLeft > 0)
            {
                if (controller.currentNode.nextNodes.Count > 1)
                {
                    controller.ChangeState(controller.chooseDirectionState);
                }
                else
                {
                    controller.toMoveNode = controller.currentNode.nextNodes[0];
                }
            }
        }
    }

    public override string ToString()
    {
        return "Run";
    }
}
