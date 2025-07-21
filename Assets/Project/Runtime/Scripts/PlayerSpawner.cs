using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    private PlayerManager playerManager;

    public GameObject boardCarPrefab;
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
        BoardGameData boardGameData = BoardGameData.instance;
        bool isBoardScene = SceneManager.GetActiveScene().name == "Map1";
        if (isBoardScene)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        //foreach(var playerInput in playerManager.players)
        //{
        //    var player = Instantiate(playerPrefab, spawnPosition[0].position, Quaternion.identity);
        //    MNGVongXoayController p = player.GetComponent<MNGVongXoayController>();    
        //    p.SetInput(playerInput);
        //}

        BoardGameData boardGameData = BoardGameData.instance;
        bool isBoardScene = SceneManager.GetActiveScene().name == "Map1";

        if (isBoardScene)
        {
            BoardCar car = Instantiate(boardCarPrefab, spawnPosition[0].position, Quaternion.identity)
                              .GetComponent<BoardCar>();
            int index = 0;
            foreach(var playerInput in playerManager.players)
            {
                var player = Instantiate(playerPrefab, car.playerSitPositions[index].position, Quaternion.identity);
                player.transform.SetParent(car.playerSitPositions[index]);
                player.transform.localPosition = Vector3.zero;
                player.transform.localRotation = Quaternion.Euler(0,0,0);

                Custom customData = PlayerManager.instance.GetComponentInChildren<CustomData>().GetCustom(playerInput);
                PlayerSetup playerSetup = player.GetComponent<PlayerSetup>();

                playerSetup.UpdateCustom(customData.hairIndex, customData.colorIndex, customData.bodyPartIndex);

                car.animators.Add(player.GetComponent<Animator>());
                if (index <= 1) player.GetComponent<Animator>().Play("Sit");
                index++;
            }

        }
        else
        {
            for(int i = 0; i < playerManager.players.Count; i++) 
            {
                MNGChayTruongController player = Instantiate(playerPrefab, spawnPosition[i].position, Quaternion.identity).GetComponent<MNGChayTruongController>();
                player.SetInput(playerManager.players[i]);
            }
        }
        

    }

    public Dictionary<int, int> GetSpawnedCharacters()
    {
        Dictionary<int, int> dictCopy = spawnedCharacters.ToDictionary(pair => pair.Key, pair => pair.Value);
        return dictCopy;

    }
}
