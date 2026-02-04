using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Horse : BoardItem
{
    public int moveStep = 3;
    private NewBoardGameController controller;

    private bool isMoving = false;

    public override void Init(NewBoardGameController controller)
    {
        this.controller = controller;
        controller.SetStepLeft(moveStep);
    }

    public override void Use()
    {
        if (controller.playerInput.actions["Trigger"].triggered && !isMoving)
        {
            isMoving = true;
        }

        if (!isMoving) return;
        
        if (!controller.MoveStep())
            itemEndUse?.Invoke();
    }

    private float EaseInOutQuad(float x)
    {
        if (x < 0f) return 0f;
        if (x > 1f) return 1f;

        return x < 0.5f
            ? 2f * x * x
            : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
    }

}
