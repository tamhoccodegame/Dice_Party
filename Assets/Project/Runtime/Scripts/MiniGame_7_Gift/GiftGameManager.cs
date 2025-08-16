using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftGameManager : MonoBehaviour
{
    [Header("Gift Setup")]
    public GameObject giftPrefab;
    public House_Area[] houseAreas;
    public int giftsPerArea = 4;

    [Header("Gameplay")]
    public float gameDuration = 60f;
    private float timer;

    void Start()
    {
        SpawnGifts();
        timer = gameDuration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) EndGame();
    }

    void SpawnGifts()
    {
        foreach (var house in houseAreas)
        {
            for (int i = 0; i < giftsPerArea; i++)
            {
                if (!house.CanAddGift()) break;
                GameObject giftObj = Instantiate(giftPrefab);
                GiftBox gift = giftObj.GetComponent<GiftBox>();
                gift.ownerID = house.ownerID;
                // đặt vào slot trống gần center (mặc định) – không quan trọng, vì pick/drop theo khoảng cách rồi
                house.AddGift(gift);
            }
        }
    }

    void EndGame()
    {
        Debug.Log("Game Over!");
        Time.timeScale = 0f;
    }
}
