using System;
using UnityEngine;

public class ChooseDirectionState : BoardState
{
    private Vector2 moveInput;

    float inputCooldown = 0.25f;
    float inputTimer;

    public Action onDirectionChose;

    public ChooseDirectionState(NewBoardGameController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        CameraFollow.instance.SwitchCamera(CameraFollow.CameraState.Juction);
        controller.splineAnimate.Pause();
        controller.ChangeAnimation("Idle");
        ShowDirectionChoices();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        moveInput = controller.playerInput.actions["Move"].ReadValue<Vector2>();

        inputTimer -= Time.deltaTime;

        if (inputTimer <= 0)
        {
            if (moveInput.x > 0.5)
            {
                NextHoverArrow();
                inputTimer = inputCooldown;
            }
            else if (moveInput.x < -0.5f)
            {
                PrevHoverArrow();
                inputTimer = inputCooldown;
            }
        }

        if (controller.playerInput.actions["Trigger"].triggered)
        {
            ChooseDirection();
            CameraFollow.instance.SwitchCamera(CameraFollow.CameraState.Default);
        }
    }

    // --- Spawn các mũi tên chọn hướng khi tới ngã ba ---
    public void ShowDirectionChoices()
    {
        ClearArrow();
        for (int i = 0; i < controller.currentNode.nextNodes.Count; i++)
        {
            BoardNode next = controller.currentNode.nextNodes[i];
            Vector3 midPoint = (controller.currentNode.transform.position + next.transform.position) / 2;
            midPoint.y = controller.arrowDirectionPrefab.transform.position.y;

            ArrowPointer arrow = GameObject.Instantiate(controller.arrowDirectionPrefab, midPoint, Quaternion.identity).GetComponent<ArrowPointer>();
            if (i == 0)
            {
                controller.currentHoverArrowIndex = 0;
                controller.hoverArrow = arrow;
                arrow.Hover();
            }
            else
                arrow.UnHover();

            arrow.transform.rotation = Quaternion.LookRotation((next.transform.position - controller.currentNode.transform.position), Vector3.up);
            controller.spawnedArrows.Add(arrow.gameObject);

        }
    }

    // --- Clear các mũi tên chọn hướng cũ ---
    void ClearArrow()
    {
        foreach (var go in controller.spawnedArrows)
        {
            GameObject.Destroy(go);
        }
        controller.spawnedArrows.Clear();
    }


    public void NextHoverArrow()
    {
        controller.currentHoverArrowIndex = (controller.currentHoverArrowIndex + 1) % controller.spawnedArrows.Count;

        if (controller.hoverArrow != null)
            controller.hoverArrow.UnHover();

        controller.hoverArrow = controller.spawnedArrows[controller.currentHoverArrowIndex].GetComponent<ArrowPointer>();
        controller.hoverArrow.Hover();
    }

    public void PrevHoverArrow()
    {
        controller.currentHoverArrowIndex -= 1;
        if (controller.currentHoverArrowIndex < 0) controller.currentHoverArrowIndex = controller.spawnedArrows.Count - 1;

        if (controller.hoverArrow != null)
            controller.hoverArrow.UnHover();

        controller.hoverArrow = controller.spawnedArrows[controller.currentHoverArrowIndex].GetComponent<ArrowPointer>();
        controller.hoverArrow.Hover();
    }

    public void ChooseDirection()
    {
        ClearArrow();
        controller.toMoveNode = controller.currentNode.nextNodes[controller.currentHoverArrowIndex];
        if (onDirectionChose != null)
        {
            onDirectionChose.Invoke();
        }
        else
        {
            controller.ChangeState(controller.movingState);
        }
    }

    public override string ToString()
    {
        return "Idle";
    }
}
