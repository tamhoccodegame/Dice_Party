using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class T_Coin_Manager : MiniGameManager
{
    public static T_Coin_Manager Instance { get; private set; }
    public Dictionary<PlayerInput, GameObject> playersGoal = new Dictionary<PlayerInput, GameObject>();
    private Dictionary<PlayerInput, int> playerInitLives = new Dictionary<PlayerInput, int>();

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        foreach(var player in PlayerManager.instance.players)
        {
            int lives = WizardPartyData.instance.playerLives[player];
            playerInitLives.Add(player, lives);
        }
        UpdateHUD();
    }

    protected override void TriggerAfterTutorial()
    {
        SplineFollower[] cams = FindObjectsByType<SplineFollower>(FindObjectsSortMode.None);

        foreach(var cam in cams)
        {
            cam.follow = true;
        }
    }

    public void UpdateGoal(PlayerInput playerInput, GameObject playerGo)
    {
        if(!playersGoal.ContainsKey(playerInput))
        {
            playersGoal.Add(playerInput, playerGo);
        }
        if (CheckGameOver())
        {
            ShowGameOverPanel();
        }
    }

    public override void SpawnRewardAvatar()
    {
        List<PlayerInput> inputs = PlayerManager.instance.players;

        for(int i = 0; i < inputs.Count; i++)
        {
            var inputGo = playersGoal[inputs[i]];
            inputGo.GetComponent<MNGChayTruongController>().enabled = false;
            inputGo.GetComponent<CharacterController>().enabled = false;
            inputGo.transform.position = rankPositions[i].position;
            inputGo.transform.rotation = Quaternion.Euler(0, -90, 0);



            int currentLives = WizardPartyData.instance.playerLives[inputs[i]];
            gameOverSlots[i].gameObject.SetActive(true);
            gameOverSlots[i].keyQtyText.text = currentLives.ToString();
            if (playerInitLives[inputs[i]] > currentLives)
            {
                gameOverSlots[i].rankText.text = "-" + Mathf.Max(0, (playerInitLives[inputs[i]] - currentLives)).ToString();
                inputGo.GetComponent<Animator>().Play("Lose");
            }
            else
            {
                inputGo.GetComponent<Animator>().Play("Win");
                gameOverSlots[i].rankText.text = "";
            }
        }
    }

    public override void UpdateHUD()
    {
        List<PlayerInput> inputs = PlayerManager.instance.players;

        for (int i = 0; i < inputs.Count; i++)
        {
            playerTextUI[i].text = WizardPartyData.instance.playerLives[inputs[i]].ToString();
        }
    }

    public override void ShowGameOverPanel()
    {
        base.ShowGameOverPanel();
    }

    public override bool CheckGameOver()
    {
        return playersGoal.Count == PlayerManager.instance.players.Count;
    }
}