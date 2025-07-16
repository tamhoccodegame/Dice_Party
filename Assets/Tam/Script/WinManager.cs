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
