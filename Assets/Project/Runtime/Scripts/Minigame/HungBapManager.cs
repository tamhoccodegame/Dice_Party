using System.Linq;
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

    protected override void TriggerAfterCutscene()
    {
        base.TriggerAfterCutscene();
    }

    public override void SpawnRewardAvatar()
    {
        FindFirstObjectByType<Light>().shadows = LightShadows.None;

        playerScores = playerScores
                       .OrderByDescending(c => c.Value)
                       .ToDictionary(c => c.Key, c => c.Value);

        int keyAdd = 8;

        for (int i = 0; i < playerScores.Count; i++)
        {
            WizardPartyData.instance.UpdatePlayerKey(playerScores.ElementAt(i).Key, keyAdd);
            gameOverSlots[i].keyQtyText.text = keyAdd.ToString();
            keyAdd -= 2;
            gameOverSlots[i].gameObject.SetActive(true);
            var inputGo = playerObjects[playerScores.ElementAt(i).Key];
            inputGo.GetComponent<CyclingIKController>().enabled = false;

            if (i > 1) inputGo.GetComponent<Animator>().Play($"Lose{Random.Range(1, 4)}");
            else inputGo.GetComponent<Animator>().Play($"Win{Random.Range(1, 6)}");

            inputGo.GetComponent<PlayerController>().enabled = false;
            inputGo.GetComponent<CharacterController>().enabled = false;
            inputGo.transform.Find("CUP").gameObject.SetActive(false);
            inputGo.transform.position = rankPositions[i].position;
            inputGo.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }

    public override void UpdateHUD()
    {
        base.UpdateHUD();
    }
}
