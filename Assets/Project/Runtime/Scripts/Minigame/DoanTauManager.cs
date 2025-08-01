using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoanTauManager : WizardMiniGameManager
{
    public static DoanTauManager instance;

    public Dictionary<PlayerInput, Sprite> playerAvatars = new Dictionary<PlayerInput, Sprite>();

    public List<GachaGun> gachaGuns;

    protected override void Awake()
    {
        instance = this;

        var players = PlayerManager.instance.players;
        for(int i = 0; i < players.Count; i++)
        {
            Sprite playerAvatar = AvatarLoader.instance.GetAvatarSprite(i);
            if(playerAvatar != null)
            {
                playerAvatars.Add(players[i], playerAvatar);
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
        while (playerObjects.Count <= 0) yield return null;

        while (!isGameStarted || isGameOver) yield return null;

        while (time > 0)
        {
            foreach(var gun in gachaGuns) gun.lockSign.SetActive(false);

            foreach (var gun in gachaGuns)
            {
                gun.SpinGacha();
                yield return new WaitForSeconds(0.1f);
                while (!gun.readyToFire) yield return null;
            }

            yield return new WaitForSeconds(1.5f);

            foreach(var gun in gachaGuns)
            {
                gun.Fire();
                yield return null;
            }

            bool isAllReadyToSpinAgain = false;
            Debug.Log($"Before While");

            while (!isAllReadyToSpinAgain)
            {
                foreach(var gun in gachaGuns)
                {
                    if (!gun.readyToChooseTarget)
                    {
                        yield return new WaitForSeconds(0.1f);
                        Debug.Log($"{gun.name} is not ready");
                        break;
                    }
                    Debug.Log($"All Gun Ready");
                    isAllReadyToSpinAgain = true;
                    yield return null;
                }
                yield return null;
            }

        }
        
    }

    public void StartAllFakeGacha()
    {
        foreach (var gun in gachaGuns)
        {
            gun.StartFakeGacha();
        }
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(FireGunRepeat());
    }

    public override bool CheckGameOver()
    {
        return playersCompleteGame.Count == PlayerManager.instance.players.Count || time <= 0;
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
