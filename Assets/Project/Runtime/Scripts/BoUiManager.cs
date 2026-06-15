using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoUiManager : WizardMiniGameManager
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

    public override void SpawnRewardAvatar(bool isAscending)
    {
        base.SpawnRewardAvatar(false);
    }

    public override void UpdateHUD()
    {
        base.UpdateHUD();
    }

    protected override void TriggerAfterCutscene()
    {
        base.TriggerAfterCutscene();
    }
}
