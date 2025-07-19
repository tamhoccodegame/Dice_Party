using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WizardPartyData : MonoBehaviour
{
    public static WizardPartyData instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public BoardNode carNode;
    public BoardNode wizardNode;

    public Dictionary<PlayerInput, int> playerLives = new Dictionary<PlayerInput, int>();

    public void UpdateCarNode(BoardNode node)
    {
        carNode = node;
    }

    public void UpdateWizardNode(BoardNode node)
    {
        wizardNode = node;
    }

    public void UpdatePlayerLive(PlayerInput input, int live)
    {
        if(!playerLives.ContainsKey(input))
        {
            playerLives.Add(input, live);
        }
        else
        {
            playerLives[input] = live;
        }
    }
}
