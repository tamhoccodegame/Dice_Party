using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public List<PlayerInput> players = new List<PlayerInput>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
 
    public void AddPlayer(PlayerInput input)
    {
        players.Add(input);
    }

    public void RemovePlayer(PlayerInput input)
    {
        if (players.Contains(input))
        {
            players.Remove(input);
        }
    }
}
