using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NhayLopManager : WizardMiniGameManager
{
    protected override void Awake()
    {
        instance = this;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        foreach (var player in PlayerManager.instance.players)
        {
            int lives = WizardPartyData.instance.playerLives[player];
            playerInitLives.Add(player, lives);
        }
        UpdateHUD();
        MusicManager.instance.PlayMusic(music);
    }

    private void Update()
    {
        if(CheckGameOver())
        {
            ShowGameOverPanel();
        }
    }

    public override bool CheckGameOver()
    {
        return time <= 0;
    }

    public override void ShowGameOverPanel()
    {
        base.ShowGameOverPanel();
    }

    public override void SpawnRewardAvatar()
    {
        base.SpawnRewardAvatar();
    }

    public override void UpdateHUD()
    {
        base.UpdateHUD();
    }
}
