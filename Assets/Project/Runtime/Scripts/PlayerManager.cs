using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public List<PlayerInput> players = new List<PlayerInput>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (Keyboard.current != null)
        {
            Debug.Log("Keyboard Detected!");
        }

        foreach (var pad in Gamepad.all)
        {
            if (pad is XInputController)
            {
                Debug.Log(pad.GetType().Name);
            }
        }
    }

    public void AddPlayer(PlayerInput input)
    {
        players.Add(input);
        input.transform.SetParent(transform);
    }

    public void RemovePlayer(PlayerInput input)
    {
        if (players.Contains(input))
        {
            players.Remove(input);
        }
    }
}
