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
        _controller = controller;
        stepRemain = step;
        _controller.chooseDirectionState.onDirectionChose += OnDirectionChose;
    }

    void OnDirectionChose()
    {
        _controller.ChangeState(_controller.itemState);
        _controller.chooseDirectionState.onDirectionChose -= OnDirectionChose;
    }

    public override void Use()
    {
        if (stepRemain <= 0)
        {
            itemEndUse?.Invoke();
            _controller.ChangeState(_controller.idleState);
        }

        if (_controller.playerInput.actions["Trigger"].triggered && !isMoving)
        {
            isMoving = true;
        }

        if (!isMoving)
            return;

        MoveStep();
    }

    public void MoveStep()
    {
        if (_controller.toMoveNode == null || stepRemain <= 0) return;

        _controller.moveDir = (_controller.toMoveNode.transform.position - _controller.feet.position).normalized;
        _controller.moveDir.y = 0;

        if (Vector3.Distance(_controller.feet.position, _controller.toMoveNode.transform.position) < 0.3f)
        {
            _controller.moveDir = Vector3.zero;
            _controller.currentNode = _controller.toMoveNode;
            WizardPartyData.instance.UpdatePlayerNode(_controller.playerInput, _controller.currentNode);
            stepRemain--;

            if (_controller.currentNode is ChestGoldNode chest)
            {
                _controller.ChangeState(_controller.nodeState);
            }

            if (stepRemain > 0)
            {
                if (_controller.currentNode.nextNodes.Count > 1)
                {
                    _controller.ChangeState(_controller.chooseDirectionState);
                }
                else
                {
                    _controller.toMoveNode = _controller.currentNode.nextNodes[0];
                }
            }
        }
    }


}
