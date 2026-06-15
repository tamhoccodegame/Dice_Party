using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuanNhauManager : WizardMiniGameManager
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
        return (playersCompleteGame.Count == WizardMiniGameManager.instance.playerObjects.Count);
    }

    public override void ShowGameOverPanel()
    {
        base.ShowGameOverPanel();
    }

    public override void SpawnRewardAvatar(bool isAscending)
    {
        base.SpawnRewardAvatar(false);
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
