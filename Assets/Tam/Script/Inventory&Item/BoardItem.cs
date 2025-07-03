using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardItem : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab;

    public abstract void Use(NetworkId playerObject);
}
