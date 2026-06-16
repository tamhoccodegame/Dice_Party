using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class DapXeManager : WizardMiniGameManager
{
    public Rect[] playersCamRect;
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
        return playersCompleteGame.Count >= playerObjects.Count;
    }

    public override void ShowGameOverPanel()
    {
        base.ShowGameOverPanel();
    }

    public override void SpawnRewardAvatar(bool isAscending)
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

            //var inputGo = playerObjects[playerScores.ElementAt(i).Key];
            ////if (i > 1) inputGo.GetComponent<Animator>().Play($"Lose{Random.Range(1, 4)}");
            ////else inputGo.GetComponent<Animator>().Play($"Win{Random.Range(1, 6)}");

            //inputGo.GetComponent<PlayerController>().enabled = false;
            //inputGo.GetComponent<CharacterController>().enabled = false;
            //inputGo.transform.position = rankPositions[i].position;
            //inputGo.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }

    protected override void TriggerAfterCutscene()
    {
        base.TriggerAfterCutscene();
        for(int i = 0; i < playerObjects.Count; i++)
        {
            Camera cam = playerObjects[i].GetComponentInChildren<Camera>();
            cam.gameObject.SetActive(true);
            cam.rect = playersCamRect[i];

            //GetComponentInChildren<CinemachineCamera>().
        }
    }

    public override void UpdateHUD()
    {
        base.UpdateHUD();
    }

}
