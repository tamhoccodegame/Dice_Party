using System;
using System.Collections;
using UnityEngine;

public abstract class BoardItem : MonoBehaviour
{
    //public abstract void Use(NewBoardGameController controller);
    //public abstract void Tick(NewBoardGameController controller);

    //public abstract IEnumerator ProcessCoroutine(NewBoardGameController controller);
    public string itemName;
    public GameObject itemPrefab;

    public Action itemStartUse;
    public Action itemEndUse;

    public abstract void Init(NewBoardGameController controller);
    public abstract void Use();
}
