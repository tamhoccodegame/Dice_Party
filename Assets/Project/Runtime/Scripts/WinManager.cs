using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class WinManager : MonoBehaviour
{
    public static WinManager instance;

    public GameObject playerPrefab;
    public Transform[] spawnPositions;

    public bool canMove { get; set; } = false;

    public PlayableDirector cutscene;

    public List<PlayerInput> playerInputs = new List<PlayerInput>();
    private HashSet<PlayerInput> confirmedPlayers = new HashSet<PlayerInput>();


    public void Awake()
    {
        SpawnPlayer();
        PlayCutscene();
    }

    private void Start()
    {
        playerInputs = PlayerManager.instance.players;
    }

    void ReturnToMainMenu()
    {
        MusicManager.instance.PlayMainTheme();
        LevelLoader.instance.LoadScene("UI_StartScene");
    }

    private void Update()
    {
        foreach (var playerInput in playerInputs)
        {
            if (playerInput.actions["Confirm"].WasPressedThisFrame())
            {
                confirmedPlayers.Add(playerInput);
            }
        }

        if (confirmedPlayers.Count == playerInputs.Count)
        {
            ReturnToMainMenu();
        }
    }

    public void SpawnPlayer()
    {
        Dictionary<PlayerInput, MatchAwardSystem.MatchTitle> allMatchTitle;
        allMatchTitle = MatchAwardSystem.instance.GetAllMatchTitles();

        int index = 1;
        foreach(var playerInput in PlayerManager.instance.players)
        {
            GameObject spawnedPlayer = spawnedPlayer = Instantiate(playerPrefab);
;
            if (playerInput == WizardPartyData.instance.winner)
            {
                spawnedPlayer.transform.position = spawnPositions[0].position;
                spawnedPlayer.transform.rotation = spawnPositions[0].rotation;
            }
            else
            {
                spawnedPlayer.transform.position = spawnPositions[index].position;
                spawnedPlayer.transform.rotation = spawnPositions[index].rotation;
                index++;
            }

            Custom customData = PlayerManager.instance.GetComponentInChildren<CustomData>().GetCustom(playerInput);
            PlayerSetup playerSetup = spawnedPlayer.GetComponent<PlayerSetup>();
            playerSetup.UpdateCustom(customData.hairIndex, customData.colorIndex, customData.bodyPartIndex);
            spawnedPlayer.GetComponent<WinController>().SetInput(playerInput);

            MatchAwardSystem.MatchTitle matchTitle = MatchAwardSystem.MatchTitle.None;
            if (allMatchTitle.ContainsKey(playerInput))
            {
                matchTitle = allMatchTitle[playerInput];
            }

            if(matchTitle != MatchAwardSystem.MatchTitle.None)
            spawnedPlayer.GetComponent<WinController>().SetAwardText(matchTitle);
        }
    }


    void PlayCutscene()
    {
        cutscene.Play();
        cutscene.stopped += Cutscene_stopped;
    }

    private void Cutscene_stopped(PlayableDirector obj)
    {
        canMove = true;
    }
}
