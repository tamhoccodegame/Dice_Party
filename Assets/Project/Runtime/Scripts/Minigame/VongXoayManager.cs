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
        base.Awake();
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        InvokeRepeating(nameof(CountDown), 0f, 1f);
        foreach (var player in PlayerManager.instance.players)
        {
            int lives = WizardPartyData.instance.playerLives[player];
            playerInitLives.Add(player, lives);
        }
        UpdateHUD();
        MusicManager.instance.PlayMusic(music);
    }

    void CountDown()
    {
        if(isGameOver || !isGameStarted) return;
        time -= 1;
        time = Mathf.Max(time, 0f);
        timeText.text = time.ToString();
        if (CheckGameOver())
        {
            isGameOver = true;
            ShowGameOverPanel();
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
        List<PlayerInput> inputs = PlayerManager.instance.players;

        for (int i = 0; i < inputs.Count; i++)
        {
            var inputGo = playerObjects[inputs[i]];
            inputGo.GetComponent<PlayerController>().enabled = false;
            inputGo.GetComponent<CharacterController>().enabled = false;
            inputGo.transform.position = rankPositions[i].position;
            inputGo.transform.rotation = Quaternion.Euler(0, -90, 0);

            int currentLives = WizardPartyData.instance.playerLives[inputs[i]];
            gameOverSlots[i].gameObject.SetActive(true);
            gameOverSlots[i].keyQtyText.text = currentLives.ToString();
            if (playerInitLives[inputs[i]] > currentLives)
            {
                gameOverSlots[i].rankText.text = "-" + Mathf.Max(0, (playerInitLives[inputs[i]] - currentLives)).ToString();
                inputGo.GetComponent<Animator>().Play($"Lose{i + 1}");
                if (currentLives <= 0)
                {
                    PlayerManager.instance.RemovePlayer(inputs[i]);
                }
            }
            else if (currentLives > 0)
            {
                inputGo.GetComponent<Animator>().Play($"Win{i + 1}");
                gameOverSlots[i].rankText.text = "";
            }
        }
    }

    public override void UpdateHUD()
    {
        List<PlayerInput> inputs = PlayerManager.instance.players;

        for (int i = 0; i < inputs.Count; i++)
        {
            int currentPlayerLive = WizardPartyData.instance.playerLives[inputs[i]];
            //playerTextUI[i].text = currentPlayerLive.ToString();
        }

        if (CheckGameOver())
        {
            ShowGameOverPanel();
        }
    }
}
