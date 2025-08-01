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
    
    }

    public void SpawnPlayer()
    {
        BoardGameData boardGameData = BoardGameData.instance;
        bool isBoardScene = SceneManager.GetActiveScene().name == "TuanSceneMap";

        if (isBoardScene)
        {
            foreach (var playerInput in playerManager.players)
            {
                var player = Instantiate(playerPrefab, spawnPosition[0].position, Quaternion.identity);
                Custom customData = PlayerManager.instance.GetComponentInChildren<CustomData>().GetCustom(playerInput);
                PlayerSetup playerSetup = player.GetComponent<PlayerSetup>();
                playerSetup.UpdateCustom(customData.hairIndex, customData.colorIndex, customData.bodyPartIndex);

                NewBoardGameController controller = player.GetComponent<NewBoardGameController>();
                controller.SetInput(playerInput);
                controller.DisableRagdoll();
                controller.enabled = false;
                TurnManager.instance.playerControllers.Add(playerInput, controller);
            }

        }
        else //Minigame Spawn
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                PlayerInput playerInput = PlayerManager.instance.players[i];
                var player = Instantiate(playerPrefab, spawnPosition[i].position, playerPrefab.transform.rotation).GetComponent<PlayerController>();
                Custom customData = PlayerManager.instance.GetComponentInChildren<CustomData>().GetCustom(playerInput);
                PlayerSetup playerSetup = player.GetComponent<PlayerSetup>();
                playerSetup.UpdateCustom(customData.hairIndex, customData.colorIndex, customData.bodyPartIndex);
                player.SetInput(playerManager.players[i]);
                WizardMiniGameManager.instance.playerObjects.Add(playerInput, player.gameObject);
            }
        }
    }

    public Dictionary<int, int> GetSpawnedCharacters()
    {
        Dictionary<int, int> dictCopy = spawnedCharacters.ToDictionary(pair => pair.Key, pair => pair.Value);
        return dictCopy;

    }
}
