using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public PlayerInput GetColorPlayer(int colorIndex)
    {
        foreach(var customData in customs)
        {
            if(customData.Value.colorIndex == colorIndex)
            {
                return customData.Key;
            }
        }
        return null;
    }

    public bool IsColorChoosen(int colorIndex)
    {
        Debug.Log($"Check {colorIndex}");
        foreach(var customData in customs.Values)
        {
            if (customData.colorIndex == colorIndex)
                return true;
        }

        return false;
    }
}
