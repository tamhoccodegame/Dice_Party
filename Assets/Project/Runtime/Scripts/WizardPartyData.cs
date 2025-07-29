using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBoardStat
{
    public int health;
    public int keyQty;
    public int cupQty;
}

public class WizardPartyData : MonoBehaviour
{
    public static WizardPartyData instance;
    public List<string> minigames;
    public Dictionary<PlayerInput, PlayerBoardStat> playersStat = new Dictionary<PlayerInput, PlayerBoardStat>();

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

        foreach (var player in PlayerManager.instance.players)
        {
            playersKey.Add(player, 0);
            playersNode.Add(player, null);
            playersStat.Add(player, new PlayerBoardStat { cupQty = 0, keyQty = 0, health = 30 }); 
        }
    }

    private void Start()
    {
       
    }

    public string carNode;

    public string wizardNode;

    public Dictionary<PlayerInput, int> playersKey = new Dictionary<PlayerInput, int>();

    public Dictionary<PlayerInput, string> playersNode = new Dictionary<PlayerInput, string>();

    public void UpdatePlayerNode(PlayerInput player, BoardNode node)
    {
        playersNode[player] = node.name;
    }

    public void UpdateCarNode(BoardNode node)
    {
        carNode = node.name;
    }

    public void UpdateWizardNode(BoardNode node)
    {
        wizardNode = node.name;
    }

    public void UpdatePlayerKey(PlayerInput input, int live)
    {
        if (live < 0) live = 0;
        playersKey[input] = live;
    }

    public string GetMinigame()
    {
        string pendingMinigame = minigames[0];
        minigames.RemoveAt(0);
        return pendingMinigame;
    }
}
