using System.Collections;
using UnityEngine;

public abstract class BoardItem : MonoBehaviour
{
    //public abstract void Use(NewBoardGameController controller);
    //public abstract void Tick(NewBoardGameController controller);

    //public abstract IEnumerator ProcessCoroutine(NewBoardGameController controller);
    public string itemName;

    public abstract void Use(NewBoardGameController controller);
}
