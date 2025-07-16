using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<BoardItem> items = new ();

    private BoardItem selectedItem;

    public void AddItem(BoardItem item)
    {
        items.Add(item);
    }

    public void SetSelectedItem(BoardItem item)
    {
        selectedItem = item;
    }

    public BoardItem GetSelectedItem()
    {
        return selectedItem;
    }

    public List<BoardItem> GetItemList()
    {
        return items;
    }
}

public class BoardGameData : MonoBehaviour
{
    public static BoardGameData instance;

    public Dictionary<int, string> playersCurrentNode = new Dictionary<int, string>();
    public Dictionary<int, string> playersName = new Dictionary<int, string>();

    public Dictionary<int, BoardGameStat> playersBoardStat = new Dictionary<int, BoardGameStat>();
    public Dictionary<int, Inventory> playersInventory = new Dictionary<int       , Inventory>();

    //public PlayerRef winner;

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
    }

    private void Start()
    {
        
    }

    //public void UpdateNode(PlayerRef player, string nodeName)
    //{
    //    if (!playersCurrentNode.ContainsKey(player))
    //    {
    //        playersCurrentNode.Add(player, nodeName);
    //    }
    //    else
    //    {
    //        playersCurrentNode[player] = nodeName;
    //    }

    //    foreach (var kvp in playersCurrentNode)
    //    {
    //        Debug.Log($"{kvp.Key} {kvp.Value}");
    //    }
    //}

    //public string GetNode(PlayerRef player)
    //{
    //    if(playersCurrentNode.ContainsKey(player))
    //    return playersCurrentNode[player];

    //    return null;
    //}

    //public void UpdateName(PlayerRef player, string name)
    //{
    //    if (name == "" || name == null) return;
    //    if (!playersName.ContainsKey(player))
    //    {
    //        playersName.Add(player, name);
    //    }
    //    else
    //    {
    //        playersName[player] = name;
    //    }
    //}

    //public string GetName(PlayerRef player)
    //{
    //    if(playersName.ContainsKey(player))
    //        return playersName[player];
    //    return null;
    //}

    //public void EnsurePlayerStatAndInventory(List<PlayerRef> players)
    //{
    //    foreach(var player in players)
    //    {
    //        if (!playersBoardStat.ContainsKey(player))
    //        {
    //            playersBoardStat[player] = new BoardGameStat();
    //            playersInventory[player] = new Inventory();
    //        }
    //    }
    //}

    //public void UpdateItem(PlayerRef player, BoardItem item)
    //{
    //    if (playersBoardStat.ContainsKey(player))
    //    {
    //        Inventory inventory = playersInventory[player];
    //        inventory.AddItem(item);
    //        inventory.SetSelectedItem(item);
    //    }
    //}

    //public void UpdateKey(PlayerRef player, int ammount)
    //{
    //    if (playersBoardStat.ContainsKey(player))
    //    {
    //        BoardGameStat stat = playersBoardStat[player];

    //        stat.keyQty += ammount;

    //        playersBoardStat[player] = stat;
    //    }
    //}
    //public void UpdateCup(PlayerRef player, int ammount)
    //{
    //    if (playersBoardStat.ContainsKey(player))
    //    {
    //        BoardGameStat inventory = playersBoardStat[player];

    //        inventory.cupQty += ammount;

    //        playersBoardStat[player] = inventory;
    //    }
    //}
    //public void UpdateHealth(PlayerRef player, int ammount)
    //{
    //    if (playersBoardStat.ContainsKey(player))
    //    {
    //        BoardGameStat stat = playersBoardStat[player];

    //        stat.health += ammount;

    //        playersBoardStat[player] = stat;
    //    }
    //}

    //public int GetKey(PlayerRef player)
    //{
    //    BoardGameStat boardGameStat = playersBoardStat[player];

    //    if(boardGameStat != null)
    //    {
    //        return boardGameStat.keyQty;
    //    }
    //    return 0;
    //}
    //public int GetCup(PlayerRef player)
    //{
    //    BoardGameStat boardGameStat = playersBoardStat[player];

    //    if (boardGameStat != null)
    //    {
    //        return boardGameStat.cupQty;
    //    }
    //    return 0;
    //}
    //public int GetHealth(PlayerRef player)
    //{
    //    BoardGameStat boardGameStat = playersBoardStat[player];

    //    if (boardGameStat != null)
    //    {
    //        return boardGameStat.health;
    //    }
    //    return 0;
    //}
}
