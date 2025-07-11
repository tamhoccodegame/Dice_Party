using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkObject), typeof(NetworkTransform))]
public abstract class BoardItem : NetworkBehaviour
{
    public abstract void Use(NewBoardGameController controller);
    public abstract void Tick(NewBoardGameController controller);

    public abstract IEnumerator ProcessCoroutine(NewBoardGameController controller);
}
