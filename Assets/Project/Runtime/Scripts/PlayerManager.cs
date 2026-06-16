using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public List<GameObject> players = new List<GameObject>();

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

    public void AddPlayer(GameObject player)
    {
        players.Add(player);
        player.transform.SetParent(transform);
    }

    public void RemovePlayer(GameObject player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }
}
