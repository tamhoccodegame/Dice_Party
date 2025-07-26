using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerController : MonoBehaviour
{
    public abstract PlayerInput GetPlayerInput();
    public abstract void SetInput(PlayerInput input);
}
