using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class WinManager : MonoBehaviour
{
    public static WinManager instance;

    public GameObject playerPrefab;
    public Transform[] spawnPositions;

    public bool canMove { get; set; } = false;

    public PlayableDirector cutscene;

    public void Awake()
    {
        int index = 1;
        foreach (var player in NetworkManager.instance.GetAllPlayers())
        {
            BoardGameData data = BoardGameData.instance;

            if (player == data.winner)
            {
                //Runner.Spawn(playerPrefab, spawnPositions[0].position, Quaternion.Euler(0, -180, 0), player);
            }
            else
            {
                //Runner.Spawn(playerPrefab, spawnPositions[index].position, Quaternion.Euler(0, -180, 0), player);
                index++;
            }
        }

        PlayCutscene();
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
