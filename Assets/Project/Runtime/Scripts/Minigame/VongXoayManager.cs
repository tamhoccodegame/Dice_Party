using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class VongXoayManager : WizardMiniGameManager
{
    public static VongXoayManager instance;

    public float time;
    public TextMeshProUGUI timeText;

    protected override void Awake()
    {
        instance = this;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        //InvokeRepeating(nameof(CountDown), 0f, 1f);
    }

    void CountDown()
    {
        time -= 1;
        time = Mathf.Max(time, 0f);
        timeText.text = time.ToString();
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
        List<PlayerInput> inputs = PlayerManager.instance.players;

        for (int i = 0; i < inputs.Count; i++)
        {
            var inputGo = playerObjects[inputs[i]];
            inputGo.GetComponent<MNGChayTruongController>().enabled = false;
            inputGo.GetComponent<CharacterController>().enabled = false;
            inputGo.transform.position = rankPositions[i].position;
            inputGo.transform.rotation = Quaternion.Euler(0, -90, 0);

            inputGo.GetComponent<Animator>().Play("Win");


            int currentLives = WizardPartyData.instance.playerLives[inputs[i]];
            gameOverSlots[i].gameObject.SetActive(true);
            gameOverSlots[i].keyQtyText.text = currentLives.ToString();
            if (playerInitLives[inputs[i]] > currentLives)
                gameOverSlots[i].rankText.text = "-" + Mathf.Max(0, (playerInitLives[inputs[i]] - currentLives)).ToString();
            else
            {
                gameOverSlots[i].rankText.text = "";
            }
        }
    }

    public override void UpdateHUD()
    {
        int index = 0;
        foreach (var kvp in playerInitLives)
        {
            if (index < playerTextUI.Length)
            {
                playerTextUI[index].text = kvp.Value.ToString();
                index++;
            }
        }
    }
}
