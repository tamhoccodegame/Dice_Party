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

    public bool isGoldChestOpened = true;
    public int currentChestIndex = -1;

    public PlayerInput winner;

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
            playersNode.Add(player, null);
            playersStat.Add(player, new PlayerBoardStat { cupQty = 0, keyQty = 0, health = 30 }); 
        }
    }

    private void Start()
    {
       
    }

    public string carNode;

    public string wizardNode;

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

    public void UpdatePlayerHealth(PlayerInput input, int qty)
    {
        playersStat[input].health += qty;
    }
    public void UpdatePlayerCup(PlayerInput input, int qty)
    {
        playersStat[input].cupQty += qty;
        winner = input;
        LevelLoader.instance.LoadScene("Win");
    }
    public void UpdatePlayerKey(PlayerInput input, int qty)
    {
        playersStat[input].keyQty += qty;
    }

    public string GetMinigame()
    {
        string pendingMinigame = minigames[0];
        minigames.RemoveAt(0);
        return pendingMinigame;
    }
}
