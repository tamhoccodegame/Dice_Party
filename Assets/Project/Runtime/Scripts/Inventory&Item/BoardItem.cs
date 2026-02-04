using System;
using System.Collections;
using UnityEngine;

public abstract class BoardItem : MonoBehaviour
{
    public Action itemStartUse;
    public Action itemEndUse;

    public abstract void Init(NewBoardGameController controller);
    public abstract void Use();
}
