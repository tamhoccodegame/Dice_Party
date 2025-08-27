using System.Collections.Generic;
using UnityEditor;
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
    public int chestToWin;
    public int currentChestIndex = -1;

    public int currentMinigameIndex = 0;

    public bool isFirstTry = true;

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
        playersStat[input].health = Mathf.Min(playersStat[input].health, 30);
        if (playersStat[input].health <= 0)
        {
            playersStat[input].health = 30;
            playersStat[input].keyQty = 0;
            MatchAwardSystem.instance.keysCount[input] = playersStat[input].keyQty;
        }
    }
    public void UpdatePlayerCup(PlayerInput input, int qty)
    {
        Debug.Log($"Update cup for {input.name} at {Time.time}");
        playersStat[input].cupQty += qty;
        CheckWin();
    }

    public void CheckWin()
    {
        foreach(var player in playersStat)
        {
            if(player.Value.cupQty >= chestToWin)
            {
                Debug.Log(player.Value.cupQty);
                winner = player.Key;
                LevelLoader.instance.LoadScene("Win");
                return;
            }
        }
    }

    public void UpdatePlayerKey(PlayerInput input, int qty)
    {
        playersStat[input].keyQty += qty;
        MatchAwardSystem.instance.keysCount[input] = playersStat[input].keyQty;
    }

    public string GetMinigame()
    {
        string pendingMinigame = minigames[currentMinigameIndex];
        currentMinigameIndex = (currentMinigameIndex + 1) % minigames.Count;
        return pendingMinigame;
    }
}
