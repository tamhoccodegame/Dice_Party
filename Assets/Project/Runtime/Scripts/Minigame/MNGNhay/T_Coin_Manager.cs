using Dreamteck.Splines;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class T_Coin_Manager : WizardMiniGameManager
{
    public static T_Coin_Manager Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        foreach(var player in PlayerManager.instance.players)
        {
            int lives = WizardPartyData.instance.playerLives[player];
            playerInitLives.Add(player, lives);
        }
        UpdateHUD();
        MusicManager.instance.PlayMusic(music);
    }

    protected override void TriggerAfterTutorial()
    {
        SplineFollower[] cams = FindObjectsByType<SplineFollower>(FindObjectsSortMode.None);

        foreach(var cam in cams)
        {
            cam.follow = true;
        }
    }

    public override void SpawnRewardAvatar()
    {
        base.SpawnRewardAvatar();
    }

    public override void ShowGameOverPanel()
    {
        MusicManager.instance.PlayMusic(winMusic);
        base.ShowGameOverPanel();
    }

    public override bool CheckGameOver()
    {
        var playerLives = WizardPartyData.instance.playerLives;
        return playersCompleteGame.Count == PlayerManager.instance.players.Count || playerLives.All(p => p.Value <= 0);
    }
}