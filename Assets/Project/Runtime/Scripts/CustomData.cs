using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Custom
{
    public int hairIndex;
    public int colorIndex;
    public int bodyPartIndex;
}

public class CustomData : MonoBehaviour
{
    private Dictionary<PlayerInput, Custom> customs = new Dictionary<PlayerInput, Custom>();

    public void SaveCustom(PlayerInput playerInput, Custom custom)
    {
        if(!customs.ContainsKey(playerInput))
        customs.Add(playerInput, custom);
        else
        {
            customs[playerInput] = custom;
        }
    }

    public Custom GetCustom(PlayerInput playerInput)
    {
        return customs[playerInput];
    }
}
