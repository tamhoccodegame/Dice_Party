using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CharSetup/New Setup")]
public class CharSetup : ScriptableObject
{
    public bool CharacterController;
    public bool Colliders;
    public bool Rigidbody;
    public Vector3 scale;

    public bool BoardGameController;
    public bool MNGPlayerController;
    public bool ItemController;
    public bool PickUpItem;
    public bool SplineAnimate;
}
