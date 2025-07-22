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

    private void Start()
    {
        foreach(var player in PlayerManager.instance.players)
        {
            playerLives.Add(player, 6);
        }
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
        if (live < 0) live = 0;
        playerLives[input] = live;
    }
}
