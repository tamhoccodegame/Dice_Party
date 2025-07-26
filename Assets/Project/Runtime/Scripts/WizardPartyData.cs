using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WizardPartyData : MonoBehaviour
{
    public static WizardPartyData instance;
    public List<string> minigames;

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

    private void Start()
    {
        foreach(var player in PlayerManager.instance.players)
        {
            playerLives.Add(player, 6);
        }
    }

    public string carNode;
    public string wizardNode;

    public Dictionary<PlayerInput, int> playerLives = new Dictionary<PlayerInput, int>();

    public void UpdateCarNode(BoardNode node)
    {
        carNode = node.name;
    }

    public void UpdateWizardNode(BoardNode node)
    {
        wizardNode = node.name;
    }

    public void UpdatePlayerLive(PlayerInput input, int live)
    {
        if (live < 0) live = 0;
        playerLives[input] = live;
    }

    public string GetMinigame()
    {
        string pendingMinigame = minigames[0];
        minigames.RemoveAt(0);
        return pendingMinigame;
    }
}
