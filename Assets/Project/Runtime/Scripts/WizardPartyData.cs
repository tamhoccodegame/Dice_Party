using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

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
    public Dictionary<GameObject, PlayerBoardStat> playersStat = new Dictionary<GameObject, PlayerBoardStat>();

    public bool isGoldChestOpened = true;
    public int chestToWin;
    public int currentChestIndex = -1;

    public int currentMinigameIndex = 0;

    public bool isFirstTry = true;

    public GameObject winner;

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
            //playersNode.Add(player, null);
            //playersStat.Add(player, new PlayerBoardStat { cupQty = 0, keyQty = 0, health = 30 }); 
        }
    }

    private void Start()
    {
       
    }

    public string carNode;

    public string wizardNode;

    public class PlayerNodeData
    {
        public string name;
        public float normalizeTime;
        public SplineContainer splineContainer;
    }

    public Dictionary<GameObject, PlayerNodeData> playersNode = new Dictionary<GameObject, PlayerNodeData>();

    public void UpdatePlayerNode(GameObject player, BoardNode node)
    {
        playersNode[player] = new PlayerNodeData
        {
            name = node.name,
            normalizeTime = node.normalizeTime,
            splineContainer = node.splineContainer,
        };
    }

    public void UpdateCarNode(BoardNode node)
    {
        carNode = node.name;
    }

    public void UpdateWizardNode(BoardNode node)
    {
        wizardNode = node.name;
    }

    public void UpdatePlayerHealth(GameObject player, int qty)
    {
        playersStat[player].health += qty;
        playersStat[player].health = Mathf.Min(playersStat[player].health, 30);
        if (playersStat[player].health <= 0)
        {
            playersStat[player].health = 30;
            playersStat[player].keyQty = 0;
            MatchAwardSystem.instance.keysCount[player] = playersStat[player].keyQty;
        }
    }
    public void UpdatePlayerCup(GameObject player, int qty)
    {
        Debug.Log($"Update cup for {player.name} at {Time.time}");
        playersStat[player].cupQty += qty;
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

    public void UpdatePlayerKey(GameObject player, int qty)
    {
        playersStat[player].keyQty += qty;
        MatchAwardSystem.instance.keysCount[player] = playersStat[player].keyQty;
    }

    public string GetMinigame()
    {
        string pendingMinigame = minigames[currentMinigameIndex];
        currentMinigameIndex = (currentMinigameIndex + 1) % minigames.Count;
        return pendingMinigame;
    }
}
