using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class PlayerSetupPosition : MonoBehaviour
{
    public static PlayerSetupPosition instance;

    public CharSetup charSetup;
    public GameObject playerPrefab;
    public Transform[] spawnPosition;

    public CinemachineCamera[] cinemachineCameras;

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

        Debug.Log("Player count: " + PlayerManager.instance.players.Count);


        int posIndex = 0;
        foreach (var player in PlayerManager.instance.players)
        {
            //Board Map chỉ có 1 spawnPosition nên luôn reset = 0
            if (isBoardScene) posIndex = 0;

            if (cinemachineCameras.Count() > 0)
            {
                cinemachineCameras[posIndex].Follow = player.transform;
                Debug.Log($"Assigned cine {posIndex} to Player {posIndex}");
                posIndex++;
            }

            NewBoardGameController controller = player.GetComponent<NewBoardGameController>();

            //spawnIndex++;
            //Custom customData = PlayerManager.instance.GetComponentInChildren<CustomData>().GetCustom(playerInput);
            //PlayerSetup playerSetup = player.GetComponent<PlayerSetup>();
            //playerSetup.UpdateCustom(customData.hairIndex, customData.colorIndex, customData.bodyPartIndex);

            
            //controller.SetInput(playerInput);

            //if (controller is NewBoardGameController newController)
            //    newController.DisableRagdoll();

            if (isBoardScene)
            {
               TurnManager.instance.playerControllers.Add(player, controller);
            }
            else
            {
                player.transform.localScale = new Vector3(2f, 2f, 2f);
                WizardMiniGameManager.instance.playerObjects.Add(player);
                player.GetComponent<MNGPlayerController>().enabled = true;

                Debug.Log("Minigame players: "
                + WizardMiniGameManager.instance.playerObjects.Count);
            }

            player.transform.localScale = charSetup.scale;
            player.GetComponent<NewBoardGameController>().enabled = charSetup.BoardGameController;
            player.GetComponent<MNGPlayerController>().enabled = charSetup.MNGPlayerController;
            player.GetComponent<CharacterController>().enabled = charSetup.CharacterController;
            player.TryGetComponent<LobbyCharacterAnimation>(out var lobbyCharacterAnimation);
            if (lobbyCharacterAnimation != null) Destroy(lobbyCharacterAnimation);
            Debug.Log(charSetup.CharacterController);
            player.GetComponent<ItemController>().enabled = charSetup.ItemController;
            //player.GetComponent<PickUpItem>().enabled = charSetup.PickUpItem;
            player.GetComponent<SplineAnimate>().enabled = charSetup.SplineAnimate;
            player.GetComponent<Rigidbody>().isKinematic = !charSetup.Rigidbody;
            player.GetComponent<Rigidbody>().useGravity = charSetup.Rigidbody;

            foreach (var col in player.GetComponents<Collider>())
            {
                if(col is not CharacterController)
                col.enabled = charSetup.Colliders;
            }

            player.transform.position = spawnPosition[posIndex].position;

            if(!WizardPartyData.instance.playersStat.ContainsKey(player))
            WizardPartyData.instance.playersStat.Add(player, new PlayerBoardStat
            {
                cupQty = 0,
                health = 0,
                keyQty = 0,
            });
            
        }
    }

    public void PlayerWalk()
    {
        if (WizardMiniGameManager.instance.playerObjects.Count == 0)
        {
            Debug.Log("Can't find player");
            return;
        }
        List<GameObject> players = WizardMiniGameManager.instance.playerObjects.ToList();
        var controllers = players.Select(p => p.GetComponent<MNGPlayerController>()).ToList();
        foreach (var controller in controllers)
        {
            controller.MoveForward();
        }
    }

    public void PlayerStop()
    {
        if (WizardMiniGameManager.instance.playerObjects.Count == 0) return;
        List<GameObject> players = WizardMiniGameManager.instance.playerObjects.ToList();
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
        //PlayerManager.instance.AddPlayer(playerInput);
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
        WizardMiniGameManager.instance.playerObjects.Add(player.gameObject);
    }



    public Dictionary<int, int> GetSpawnedCharacters()
    {
        Dictionary<int, int> dictCopy = spawnedCharacters.ToDictionary(pair => pair.Key, pair => pair.Value);
        return dictCopy;

    }
}
