using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    private PlayerManager playerManager;

    public GameObject playerPrefab;
    public Transform[] spawnPosition;

    public Dictionary<int, int> spawnedCharacters => default;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerManager = PlayerManager.instance;
        instance = this;
    }

    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        foreach(var playerInput in playerManager.players)
        {
            MNGVongXoayController player = Instantiate(playerPrefab, spawnPosition[0].position, Quaternion.identity)
                .GetComponent<MNGVongXoayController>();

            PlayerInput p = player.GetComponent<PlayerInput>();
            p = playerInput;
        }

        //BoardGameData boardGameData = BoardGameData.instance;
        //bool isBoardScene = SceneManager.GetActiveScene().name == "TuanSceneMap";


        //if (boardGameData != null && boardGameData.playersCurrentNode.Count > 0 && isBoardScene)
        //{
        //    TurnManager.instance.isFirstTry = false;
           
        //}
        //else if (isBoardScene)
        //{
           
        //}
        //else
        //{
            
        //}

    }

    public Dictionary<int, int> GetSpawnedCharacters()
    {
        Dictionary<int, int> dictCopy = spawnedCharacters.ToDictionary(pair => pair.Key, pair => pair.Value);
        return dictCopy;

    }
}
