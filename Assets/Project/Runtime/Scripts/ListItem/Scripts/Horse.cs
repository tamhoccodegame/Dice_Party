using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Horse : BoardItem
{
    public int step = 3;
    public int stepRemain;
    private NewBoardGameController _controller;

    private bool isMoving = false;

    public override void Init(NewBoardGameController controller)
    {
        if (controller.currentHoverArrowIndex == 1)
        {
            controller.splineAnimate.Container = controller.currentNode.nextNodes[1].splineContainer;
            controller.splineAnimate.NormalizedTime = controller.currentNode.normalizeTime;
        }

        Debug.Log(controller.currentNode.name);
        Debug.Log(controller.currentNode.splineContainer);
        Debug.Log(controller.currentNode.normalizeTime);

        _controller = controller;
        stepRemain = step;

        _controller.ChangeAnimation("Sit");
    }

    void OnDirectionChose()
    {
        _controller.ChangeAnimation("Sit");
        _controller.ChangeState(_controller.itemState);
        _controller.chooseDirectionState.onDirectionChose -= OnDirectionChose;
        if (_controller.currentHoverArrowIndex == 1)
        {
            _controller.splineAnimate.Container = _controller.currentNode.nextNodes[1].splineContainer;
            _controller.splineAnimate.NormalizedTime = _controller.currentNode.normalizeTime;
        }
        isMoving = true;
    }

    public override void Use()
    {
        if (stepRemain <= 0 && isMoving)
        {
            isMoving = false;
            itemEndUse?.Invoke();
            return;
        }

        if (_controller.playerInput.actions["Trigger"].triggered && !isMoving)
        {
            if(_controller.currentNode.nextNodes.Count > 1)
            {
                _controller.chooseDirectionState.onDirectionChose += OnDirectionChose;
                _controller.ChangeState(_controller.chooseDirectionState);
            }
            else
            {
                isMoving = true;
            }
        }

        if (!isMoving)
            return;

        MoveStep();
    }

    public void MoveStep()
    {
        _controller.splineAnimate.Play();
        if (Vector3.Distance(_controller.toMoveNode.transform.position, _controller.feet.position) < 0.2f)
        {
            _controller.currentNode = _controller.toMoveNode;
            _controller.toMoveNode = _controller.currentNode.nextNodes[0];
            stepRemain--;
            WizardPartyData.instance.UpdatePlayerNode(_controller.gameObject, _controller.currentNode);
            Debug.Log(stepRemain);


            if (_controller.currentNode.nextNodes.Count > 1 && stepRemain > 0)
            {
                _controller.chooseDirectionState.onDirectionChose += OnDirectionChose;
                _controller.ChangeState(_controller.chooseDirectionState);
            }
            else
            {
                if (_controller.currentNode.splineContainer != _controller.splineAnimate.Container)
                {
                    _controller.splineAnimate.Container = _controller.currentNode.splineContainer;
                    _controller.splineAnimate.NormalizedTime = _controller.currentNode.normalizeTime;
                }

            }
        }
    }
}
