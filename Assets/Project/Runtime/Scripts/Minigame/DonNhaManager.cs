using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonNhaManager : WizardMiniGameManager
{
    public override bool CheckGameOver()
    {
        return time <= 0;
    }

    public override void SpawnRewardAvatar(bool isAscending)
    {
        base.SpawnRewardAvatar(true);
    }

}
