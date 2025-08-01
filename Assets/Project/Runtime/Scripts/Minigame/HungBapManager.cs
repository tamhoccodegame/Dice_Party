using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungBapManager : WizardMiniGameManager
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override bool CheckGameOver()
    {
        return time <= 0;
    }

    public override void ShowGameOverPanel()
    {
        base.ShowGameOverPanel();
    }

    protected override void TriggerAfterTutorial()
    {
        base.TriggerAfterTutorial();
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
