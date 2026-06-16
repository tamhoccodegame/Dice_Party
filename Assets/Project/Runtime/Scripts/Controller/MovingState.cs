using System.Collections.Generic;
using UnityEngine;

public class MovingState : BoardState
{
    public int stepLeft;
    public List<BoardNode> juctionChoices;
    public bool isInJuction = false;
    public bool isReachedNode = false;

    public MovingState(NewBoardGameController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        CameraFollow.instance.SwitchCamera(CameraFollow.CameraState.Default);

        if(controller.currentHoverArrowIndex == 1)
        {
            controller.splineAnimate.Container = controller.currentNode.nextNodes[1].splineContainer;
            controller.splineAnimate.NormalizedTime = controller.currentNode.normalizeTime;
        }

        Debug.Log(controller.currentNode.name);
        Debug.Log(controller.currentNode.splineContainer);
        Debug.Log(controller.currentNode.normalizeTime);
        controller.splineAnimate.Play();
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
        controller.splineAnimate.Pause();
    }

    public override void Update()
    {
        if (stepLeft > 0)
        {
            if(Vector3.Distance(controller.toMoveNode.transform.position, controller.feet.position) < 0.2f)
            {
                controller.currentNode = controller.toMoveNode;
                stepLeft--;
                WizardPartyData.instance.UpdatePlayerNode(controller.gameObject, controller.currentNode);
                Debug.Log(stepLeft);

                if(controller.currentNode.nextNodes.Count > 1)
                {
                    controller.ChangeState(controller.chooseDirectionState);
                }
                else
                {
                    controller.toMoveNode = controller.currentNode.nextNodes[0];

                    if(controller.currentNode.splineContainer != controller.splineAnimate.Container)
                    {
                        controller.splineAnimate.Container = controller.currentNode.splineContainer;
                        controller.splineAnimate.NormalizedTime = controller.currentNode.normalizeTime;
                    }
                    
                }
            }
        }
        else
            controller.ChangeState(controller.nodeState);
    }

    //public void MoveStep()
    //{
    //    if (controller.toMoveNode == null || stepLeft <= 0) return;

    //    controller.moveDir = (controller.toMoveNode.transform.position - controller.feet.position).normalized;
    //    controller.moveDir.y = 0;

    //    if (Vector3.Distance(controller.feet.position, controller.toMoveNode.transform.position) < 0.3f)
    //    {
    //        controller.moveDir = Vector3.zero;
    //        controller.currentNode = controller.toMoveNode;
    //        WizardPartyData.instance.UpdatePlayerNode(controller.playerInput, controller.currentNode);
    //        stepLeft--;

    //        if (controller.currentNode is ChestGoldNode chest)
    //        {
    //            controller.ChangeState(controller.nodeState);
    //        }

    //        if (stepLeft > 0)
    //        {
    //            if (controller.currentNode.nextNodes.Count > 1)
    //            {
    //                controller.ChangeState(controller.chooseDirectionState);

    //            }
    //            else
    //            {
    //                controller.toMoveNode = controller.currentNode.nextNodes[0];
    //            }
    //        }
    //    }
    //}

    public override string ToString()
    {
        return "Run";
    }
}
