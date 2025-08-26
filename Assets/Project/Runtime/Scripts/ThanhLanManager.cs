using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThanhLanManager : WizardMiniGameManager
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

    public override void SpawnRewardAvatar()
    {
        base.SpawnRewardAvatar();
    }

    protected override void TriggerAfterCutscene()
    {
        base.TriggerAfterCutscene();
    }

    public override void UpdateHUD()
    {
        base.UpdateHUD();
    }
}
