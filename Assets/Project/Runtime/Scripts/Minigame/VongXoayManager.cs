using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class VongXoayManager : WizardMiniGameManager
{
    public static VongXoayManager instance;

    public float time;
    public TextMeshProUGUI timeText;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        InvokeRepeating(nameof(CountDown), 0f, 1f);
        foreach (var player in PlayerManager.instance.players)
        {
            int lives = WizardPartyData.instance.playersKey[player];
            playerInitLives.Add(player, lives);
        }
        UpdateHUD();
        MusicManager.instance.PlayMusic(music);
    }

    void CountDown()
    {
        if(isGameOver || !isGameStarted) return;
        time -= 1;
        time = Mathf.Max(time, 0f);
        timeText.text = time.ToString();
        if (CheckGameOver())
        {
            isGameOver = true;
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
