using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoanTauManager : WizardMiniGameManager
{
    public static DoanTauManager instance;

    public List<Sprite> playerAvatars = new List<Sprite>();

    public List<GachaGun> gachaGuns;

    public float time;

    protected override void Awake()
    {
        instance = this;

        var players = PlayerManager.instance.players;
        for(int i = 0; i < players.Count; i++)
        {
            Sprite playerAvatar = AvatarLoader.instance.GetAvatarSprite(i);
            if(playerAvatar != null)
            {
                playerAvatars.Add(playerAvatar);
            }
        }

        foreach(var gun in gachaGuns)
        {
            gun.Init(playerAvatars);
        }

        base.Awake();
    }

    IEnumerator FireGunRepeat()
    {
        while(time > 0)
        {
            foreach (var gun in gachaGuns)
            {
                gun.SpinGacha();
                while (!gun.readyToFire) yield return null;
            }

            bool isAllReadyToSpinAgain = true;
            while (!isAllReadyToSpinAgain)
            {
                foreach(var gun in gachaGuns)
                {
                    if (!gun.readyToChooseTarget)
                    {
                        isAllReadyToSpinAgain = false;
                        yield return new WaitForSeconds(0.1f);
                        break;
                    }
                }
            }
        }
        
    }

    public void ReadyToFire()
    {
        bool isAllReady = gachaGuns.All(g => g.readyToFire);

        if (isAllReady)
        {
            foreach(var gun in gachaGuns)
            {
                gun.Fire();
            }
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public override bool CheckGameOver()
    {
        return base.CheckGameOver();
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

    protected override void TriggerAfterTutorial()
    {
        base.TriggerAfterTutorial();
    }
}
