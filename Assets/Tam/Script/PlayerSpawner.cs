using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    private SystemManager networkManager;

    public GameObject playerPrefab;
    public Transform[] spawnPosition;

    public Dictionary<int, int> spawnedCharacters => default;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        networkManager = FindFirstObjectByType<SystemManager>();
        instance = this;
    }

    public void SpawnPlayer()
    {
        BoardGameData boardGameData = BoardGameData.instance;
        bool isBoardScene = SceneManager.GetActiveScene().name == "TuanSceneMap";


        if (boardGameData != null && boardGameData.playersCurrentNode.Count > 0 && isBoardScene)
        {
            TurnManager.instance.isFirstTry = false;
           
        }
        else if (isBoardScene)
        {
           
        }
        else
        {
            //List<PlayerRef> playerList = networkManager.GetAllPlayers();
            //for (int i = 0; i < playerList.Count; i++)
            //{
            //    var go = Instantiate(playerPrefab, spawnPosition[i].position, spawnPosition[i].rotation, playerList[i]);
            //    spawnedCharacters.Add(playerList[i], go);
            //}
        }

    }

    public Dictionary<int, int> GetSpawnedCharacters()
    {
        Dictionary<int, int> dictCopy = spawnedCharacters.ToDictionary(pair => pair.Key, pair => pair.Value);
        return dictCopy;

    }
}
