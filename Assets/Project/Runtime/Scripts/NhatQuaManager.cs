using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NhatQuaManager : WizardMiniGameManager
{
    public House_Area[] houses;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        if (PlayerManager.instance == null) return;
        var players = PlayerManager.instance.players;
        for(int i = 0; i < players.Count; i++)
        {
            houses[i].gameObject.SetActive(true);
            houses[i].houseOwner = players[i];

            Sprite avatar = AvatarLoader.instance.GetAvatarSprite(i);

            houses[i].houseOwnerAvatar.sprite = avatar;
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

    protected override void TriggerAfterCutscene()
    {
        base.TriggerAfterCutscene();
    }

    public override void UpdateHUD()
    {
        base.UpdateHUD();
    }
}
