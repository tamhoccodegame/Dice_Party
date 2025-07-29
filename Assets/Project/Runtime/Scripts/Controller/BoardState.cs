using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardState
{
    protected NewBoardGameController controller;

    public BoardState(NewBoardGameController controller)
    {
        this.controller = controller;
    }
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
}
