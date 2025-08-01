//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class MinigameLivesManager : WizardMiniGameManager
//{
//    public Dictionary<PlayerInput, int> playerLives = new Dictionary<PlayerInput, int>();

//    protected override void Awake()
//    {
//        base.Awake();
//    }

//    protected override void Start()
//    {
//        base.Start();
//    }

//    public override bool CheckGameOver()
//    {
//        return base.CheckGameOver();
//    }

//    public override void ShowGameOverPanel()
//    {
//        base.ShowGameOverPanel();
//    }

//    public override void SpawnRewardAvatar()
//    {
//        List<PlayerInput> inputs = PlayerManager.instance.players;

//        for (int i = 0; i < inputs.Count; i++)
//        {
//            var inputGo = playerObjects[inputs[i]];
//            inputGo.GetComponent<PlayerController>().enabled = false;
//            inputGo.GetComponent<CharacterController>().enabled = false;
//            inputGo.transform.position = rankPositions[i].position;
//            inputGo.transform.rotation = Quaternion.Euler(0, -90, 0);

//            int currentLives = WizardPartyData.instance.playersKey[inputs[i]];
//            gameOverSlots[i].gameObject.SetActive(true);
//            gameOverSlots[i].keyQtyText.text = currentLives.ToString();
//            if (playerLives[inputs[i]] > currentLives)
//            {
//                gameOverSlots[i].rankText.text = "-" + Mathf.Max(0, (playerLives[inputs[i]] - currentLives)).ToString();
//                inputGo.GetComponent<Animator>().Play($"Lose{i + 1}");
//                if (currentLives <= 0)
//                {
//                    PlayerManager.instance.RemovePlayer(inputs[i]);
//                }
//            }
//            else if (currentLives > 0)
//            {
//                inputGo.GetComponent<Animator>().Play($"Win{i + 1}");
//                gameOverSlots[i].rankText.text = "";
//            }
//        }
//    }

//    protected override void TriggerAfterTutorial()
//    {
//        base.TriggerAfterTutorial();
//    }

//    public override void UpdateHUD()
//    {
//        List<PlayerInput> inputs = PlayerManager.instance.players;

//        for (int i = 0; i < inputs.Count; i++)
//        {
//            int currentPlayerLive = playerLives[inputs[i]];
//            playerHUDs[i].textUI.text = currentPlayerLive.ToString();
//        }

//        if (CheckGameOver())
//        {
//            ShowGameOverPanel();
//        }
//    }
//}
