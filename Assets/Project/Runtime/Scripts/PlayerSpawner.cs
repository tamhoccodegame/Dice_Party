using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;

    public GameObject boardCarPrefab;
    public GameObject playerPrefab;
    public Transform[] spawnPosition;

    public Dictionary<int, int> spawnedCharacters => default;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    public void TrySpawnPlayer()
    {
        if (WizardMiniGameManager.instance != null && WizardMiniGameManager.instance.isDevMode)
        {
            for(int i = 0; i < 4; i++)
            DevModeSpawnPlayer(i);
        }
        else SpawnPlayer();

    }

    void SpawnPlayer()
    {
        BoardGameData boardGameData = BoardGameData.instance;
        bool isBoardScene = SceneManager.GetActiveScene().name == "BoardMap";

        int spawnIndex = 0;
        foreach (var playerInput in PlayerManager.instance.players)
        {
            if (isBoardScene) spawnIndex = 0;

            var player = Instantiate(playerPrefab, spawnPosition[spawnIndex].position, spawnPosition[spawnIndex].rotation);
            spawnIndex++;
            Custom customData = PlayerManager.instance.GetComponentInChildren<CustomData>().GetCustom(playerInput);
            PlayerSetup playerSetup = player.GetComponent<PlayerSetup>();
            playerSetup.UpdateCustom(customData.hairIndex, customData.colorIndex, customData.bodyPartIndex);

            PlayerController controller = player.GetComponent<PlayerController>();
            controller.SetInput(playerInput);

            if (controller is NewBoardGameController newController)
                newController.DisableRagdoll();

            if (isBoardScene)
                TurnManager.instance.playerControllers.Add(playerInput, controller as NewBoardGameController);
            else
                WizardMiniGameManager.instance.playerObjects.Add(playerInput, player.gameObject);
        }
    }

    public void PlayerWalk()
    {
        if (WizardMiniGameManager.instance.playerObjects.Count == 0)
        {
            Debug.Log("Can't find player");
            return;
        }
        List<GameObject> players = WizardMiniGameManager.instance.playerObjects.Values.ToList();
        var controllers = players.Select(p => p.GetComponent<MNGPlayerController>()).ToList();
        foreach (var controller in controllers)
        {
            controller.MoveForward();
        }
    }

    public void PlayerStop()
    {
        if (WizardMiniGameManager.instance.playerObjects.Count == 0) return;
        List<GameObject> players = WizardMiniGameManager.instance.playerObjects.Values.ToList();
        var controllers = players.Select(p => p.GetComponent<MNGPlayerController>()).ToList();
        foreach (var controller in controllers)
        {
            controller.StopMove();
        }
    }

    void DevModeSpawnPlayer(int spawnIndex)
    {
        var player = Instantiate(playerPrefab, spawnPosition[spawnIndex].position, spawnPosition[spawnIndex].rotation).GetComponent<PlayerController>();

        var playerInput = player.gameObject.AddComponent<PlayerInput>();
        PlayerManager.instance.AddPlayer(playerInput);
        // Load Input Action Asset
        var asset = Resources.Load<InputActionAsset>("InputAction/DefaultInputActions");
        playerInput.actions = asset;

        // Enable toàn bộ actions
        playerInput.actions.Enable();

        // Chọn map chính
        playerInput.defaultActionMap = "Player";
        playerInput.SwitchCurrentActionMap("Player");

        // Mock keyboard
        playerInput.neverAutoSwitchControlSchemes = true;

        player.SetInput(playerInput);
        WizardMiniGameManager.instance.playerObjects.Add(playerInput, player.gameObject);
    }



    public Dictionary<int, int> GetSpawnedCharacters()
    {
        Dictionary<int, int> dictCopy = spawnedCharacters.ToDictionary(pair => pair.Key, pair => pair.Value);
        return dictCopy;

    }
}
