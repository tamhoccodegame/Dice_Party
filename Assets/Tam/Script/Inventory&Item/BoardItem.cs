using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardItem
{
    public string itemName;
    public GameObject itemPrefab;

    public abstract void Use(NewBoardGameController controller);
    public abstract void Tick(NewBoardGameController controller);
}
