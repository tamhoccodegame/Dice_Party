using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Coin_Manager : MonoBehaviour
{
    public PlayableDirector introCutscene;

    public Dictionary<int, int> playersCoin = new();

    public Dictionary<int, int> playersObject = new();

    [Header("Avatar Standing Position")]
    public Transform[] rankPositions;

    public TextMeshProUGUI[] playerScoreTextUI;

    [Header("Tutorial Panel")]
    public GameObject tutorialPanel;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public GameOverSlotUI[] gameOverSlots;
    public TextMeshProUGUI whoWinsText;
    public GameObject gameOverVolume;

    public Image blackScreen;
    public float fadeDuration = 1f;

    public bool isGameOver { get; set; } = false;
    public bool isGameStarted { get; set; } = false;

    public static Coin_Manager Instance { get; private set; }

    public int TotalCoins { get; private set; } = 0;
    public GameObject coinPrefab;
    public GameObject pickupVFX;

    [Header("Drop Settings")]
    public int coinsToDropOnHit = 3;
    public float coinSpawnHeight = 0.5f;
    public float coinLifetime = 5f;
    public float spawnForce = 2.5f;

    public void Awake()
    {
        Instance = this;
        MusicManager.instance.PlayMusic(MusicManager.MusicType.MNG);
        tutorialPanel.SetActive(true);

            HideTutorial();

            UpdateScoreUI();
    }
   
    public void UpdateCoin()
    {
        //if (playersCoin.ContainsKey(player))
        //{
        //    int coin = playersCoin[player];
        //    coin += ammount;
        //    coin = Mathf.Clamp(coin, 0, 100);

        //    playersCoin.Set(player, coin);

        //    // Bước 1: Convert sang List và sort theo coin giảm dần
        //    var sorted = new List<KeyValuePair<PlayerRef, int>>(playersCoin);
        //    sorted.Sort((a, b) => b.Value.CompareTo(a.Value)); // giảm dần

        //    // Bước 2: Clear playersObject trước
        //    playersObject.Clear();

        //    // Bước 3: Gán lại theo thứ tự
        //    for (int i = 0; i < sorted.Count; i++)
        //    {
        //        PlayerRef p = sorted[i].Key;
        //        NetworkId objectId = PlayerSpawner.instance.spawnedCharacters[p];

        //        playersObject.Add(player, objectId); // hoặc map vị trí i nếu cần
        //    }
        //}
        //else
        //{
        //    playersCoin.Add(player, 1);
        //}
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
       
    }

    public void DropCoins(Vector3 origin)
    {
        int dropCount = Mathf.Min(TotalCoins, coinsToDropOnHit);
        if (dropCount <= 0)
        {
            Debug.Log("[⚠️ DROP] Not enough coins to drop.");
            return;
        }


        for (int i = 0; i < dropCount; i++)
        {
            // 👉 spawn tại player, thêm chút chiều cao để không dính sàn
            Vector3 spawnPos = origin + Vector3.up * coinSpawnHeight;
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

            Coins coinScript = coin.GetComponent<Coins>();
            if (coinScript != null)
            {
                coinScript.SetLifetime(coinLifetime);
                coinScript.value = 1;
                coinScript.pickupVFX = pickupVFX;
            }

            Rigidbody rb = coin.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 👉 Văng ra các hướng ngẫu nhiên, có hướng lên nhẹ để nảy
                Vector3 randomDir = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.4f, 1.2f),
                    Random.Range(-1f, 1f)
                ).normalized;

                rb.AddForce(randomDir * spawnForce, ForceMode.Impulse);

                // 👉 Add torque để coin xoay xoay mượt hơn
                Vector3 torque = new Vector3(
                    Random.Range(-200, 200),
                    Random.Range(-200, 200),
                    Random.Range(-200, 200)
                );
                rb.AddTorque(torque);
            }
        }

        Debug.Log($"[💥 COINS DROPPED] {dropCount} coins dropped at {origin}");
        UpdateScoreUI();
    }

    private IEnumerator FadeBlackScreen(float from, float to)
    {
        float elapsed = 0f;
        Color color = blackScreen.color;
        color.a = from;

        blackScreen.color = color;

        Color newColor = blackScreen.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Dùng unscaled để không bị ảnh hưởng bởi Time.timeScale
            newColor.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            blackScreen.color = newColor;
            yield return null;
        }

        blackScreen.color = newColor;
    }


    void HideTutorial()
    {
        StartCoroutine(HideTutorialCouroutine());
    }

    IEnumerator HideTutorialCouroutine()
    {
        yield return new WaitForSecondsRealtime(10f);

        yield return StartCoroutine(FadeBlackScreen(0, 1));
        tutorialPanel.SetActive(false);

        yield return new WaitForSecondsRealtime(5f);
        GetComponent<PlayerSpawner>().SpawnPlayer();

        if(introCutscene != null)
        {
            introCutscene.Play();
            introCutscene.stopped += StartGame;
            yield return new WaitForSecondsRealtime(1f);
            yield return StartCoroutine(FadeBlackScreen(1, 0));
        }
        else
        {
            yield return new WaitForSecondsRealtime(1f);
            yield return StartCoroutine(FadeBlackScreen(1, 0));
            yield return new WaitForSecondsRealtime(1f);
            isGameStarted = true;
            foreach (var s in FindObjectsByType<SplineFollower>(FindObjectsSortMode.None))
            {
                s.follow = true;
            }
            FindFirstObjectByType<TrapActivationManager>().SetPlayer();
        }
           
    }

    private void StartGame(PlayableDirector obj)
    {
        Destroy(obj.gameObject);
        //FindFirstObjectByType<GlobalVolume>().StartFadeOut();
            isGameStarted = true;
    }

    IEnumerator ReturnToBoard()
    {
        //Volume active
        gameOverVolume.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
            SpawnRewardAvatar();
        yield return null;
        gameOverPanel.SetActive(true);
        gameOverVolume.SetActive(false);
        yield return new WaitForSecondsRealtime(6f);
        yield return StartCoroutine(FadeBlackScreen(0, 1));
        yield return new WaitForSecondsRealtime(3f);

            SceneManager.LoadScene("TuanSceneMap");
    }

    bool CheckGameOver()
    {
        var players = FindObjectsByType<MNGChayTruongController>(FindObjectsSortMode.None);
        return players.All(p => p.isGoal);
    }

    public void UpdateGameState()
    {
        if (isGameOver) return;

        if (CheckGameOver())
        {
            isGameOver = true;
            ShowGameOverPanel();
        }
    }

    void ShowGameOverPanel()
    {
        StartCoroutine(ReturnToBoard());
    }

    public void SpawnRewardAvatar()
    {
        //whoWinsText.text = BoardGameData.instance.GetName(playersCoin.ElementAt(0).Key) + " Wins";
        for (int i = 0; i < playersCoin.Count; i++)
        {
            #region Player
            //NetworkObject iRankObject = Runner.FindObject(playersObject.ElementAt(i).Value);
            //NetworkCharacterController iCc = iRankObject.GetComponent<NetworkCharacterController>();
            //iCc.gravity = 0;
            //iCc.jumpImpulse = 0;

            //if (HasStateAuthority)
            //{
            //    iCc.Teleport(rankPositions[i].position, Quaternion.Euler(0, -90, 0));
            //}

            //Animator iAnimator = iRankObject.GetComponent<Animator>();

            //if (i == 0) iAnimator.Play("Win");
            //else iAnimator.Play("Lose");
            #endregion

            #region UISlot
            //gameOverSlots[i].gameObject.SetActive(true);
            //gameOverSlots[i].keyQtyText.text = "10";
            //gameOverSlots[i].rankText.text = $"{i + 1}";

            //string playerName = BoardGameData.instance.GetName(playersObject.ElementAt(i).Key);
            //gameOverSlots[i].nameText.text = playerName;
            #endregion

            #region Reward
            //BoardGameData data = BoardGameData.instance;
            //if (data != null)
            //{
            //    int rewardKeyQty = i == 0 ? 8 : 4;
            //    data.UpdateKey(iRankObject.InputAuthority, rewardKeyQty);

            //    //BoardItem boardItem = new ElectricGun();
            //    //data.UpdateItem(iRankObject.InputAuthority, boardItem);
            //}
            #endregion
        }
    }


}