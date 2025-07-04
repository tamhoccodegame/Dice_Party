using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class BoardGameData : MonoBehaviour
{
    public static BoardGameData instance;

    public Dictionary<PlayerRef, string> playersCurrentNode = new Dictionary<PlayerRef, string>();
    public Dictionary<PlayerRef, string> playersName = new Dictionary<PlayerRef, string>();

    public Dictionary<PlayerRef, BoardGameStat> playersBoardStat = new Dictionary<PlayerRef, BoardGameStat>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        
    }

    public void UpdateNode(PlayerRef player, string nodeName)
    {
        if (!playersCurrentNode.ContainsKey(player))
        {
            playersCurrentNode.Add(player, nodeName);
        }
        else
        {
            playersCurrentNode[player] = nodeName;
        }

        foreach (var kvp in playersCurrentNode)
        {
            Debug.Log($"{kvp.Key} {kvp.Value}");
        }
    }

    public string GetNode(PlayerRef player)
    {
        if(playersCurrentNode.ContainsKey(player))
        return playersCurrentNode[player];

        return null;
    }

    public void SetName(PlayerRef player, string name)
    {
        if (!playersName.ContainsKey(player))
        {
            playersName.Add(player, name);
        }
    }

    public string GetName(PlayerRef player)
    {
        if(playersName.ContainsKey(player))
            return playersName[player];
        return null;
    }

    public void EnsurePlayerStat(List<PlayerRef> players)
    {
        foreach(var player in players)
        {
            if (!playersBoardStat.ContainsKey(player))
            {
                playersBoardStat[player] = new BoardGameStat();
            }
        }
    }

    public void UpdateItem(PlayerRef player, BoardItem item)
    {
        if (playersBoardStat.ContainsKey(player))
        {
            BoardGameStat stat = playersBoardStat[player];

            stat.AddItem(item);

            playersBoardStat[player] = stat;
        }
    }

    public void UpdateKey(PlayerRef player, int ammount)
    {
        if (playersBoardStat.ContainsKey(player))
        {
            BoardGameStat stat = playersBoardStat[player];

            stat.keyQty += ammount;

            playersBoardStat[player] = stat;
        }
    }
    public void UpdateCup(PlayerRef player, int ammount)
    {
        if (playersBoardStat.ContainsKey(player))
        {
            BoardGameStat inventory = playersBoardStat[player];

            inventory.cupQty += ammount;

            playersBoardStat[player] = inventory;
        }
    }
    public void UpdateHealth(PlayerRef player, int ammount)
    {
        if (playersBoardStat.ContainsKey(player))
        {
            BoardGameStat stat = playersBoardStat[player];

            stat.health += ammount;

            playersBoardStat[player] = stat;
        }
    }

    public int GetKey(PlayerRef player)
    {
        BoardGameStat boardGameStat = playersBoardStat[player];

        if(boardGameStat != null)
        {
            return boardGameStat.keyQty;
        }
        return 0;
    }
    public int GetCup(PlayerRef player)
    {
        BoardGameStat boardGameStat = playersBoardStat[player];

        if (boardGameStat != null)
        {
            return boardGameStat.cupQty;
        }
        return 0;
    }
    public int GetHealth(PlayerRef player)
    {
        BoardGameStat boardGameStat = playersBoardStat[player];

        if (boardGameStat != null)
        {
            return boardGameStat.health;
        }
        return 0;
    }
}
