using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GachaGun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    public Dictionary<PlayerInput, Sprite> playerAvatars = new Dictionary<PlayerInput, Sprite>();
    public SpriteRenderer avatarRenderer;

    public bool readyToFire = false;
    public bool readyToChooseTarget = false;

    public float spinTime = 3f;
    public Transform currentTarget;

    private Coroutine fakeGachaRoutine;
    public GameObject lockSign;
    public ParticleSystem fireEffect;

    public void Init(Dictionary<PlayerInput, Sprite> _playerAvatars)
    {
        playerAvatars = _playerAvatars;
        FitSpriteToOriginalSize(playerAvatars.ElementAt(0).Value);
    }

    private void Start()
    {
        StartFakeGacha(); // Gacha giả khởi động luôn
    }

    public void StartFakeGacha()
    {
        fakeGachaRoutine = StartCoroutine(FakeGachaCoroutine());
    }

    void StopFakeGacha()
    {
        if (fakeGachaRoutine != null)
        {
            StopCoroutine(fakeGachaRoutine);
            fakeGachaRoutine = null;
        }
    }

    IEnumerator FakeGachaCoroutine()
    {
        yield return null;
        //while (true)
        //{
        //    FitSpriteToOriginalSize(playerAvatars.ElementAt(Random.Range(0, playerAvatars.Count)).Value);
        //    yield return new WaitForSeconds(0.2f);
        //}
    }

    public void SpinGacha()
    {
        StartCoroutine(ReadyFireCoroutine());
    }

    IEnumerator ReadyFireCoroutine()
    {
        float elapsedTime = 0;
        while (elapsedTime < spinTime)
        {
            FitSpriteToOriginalSize(playerAvatars.ElementAt(Random.Range(0, playerAvatars.Count)).Value);
            elapsedTime += 0.2f;
            yield return new WaitForSeconds(0.2f);
        }

        // Chọn avatar thật sự
        var randomIndex = Random.Range(0, playerAvatars.Count);
        PlayerInput playerInput = playerAvatars.ElementAt(randomIndex).Key;
        Sprite newSprite = playerAvatars.ElementAt(randomIndex).Value;

        currentTarget = DoanTauManager.instance.playerObjects[playerInput].transform;
        FitSpriteToOriginalSize(newSprite);

        readyToFire = true;
        lockSign.SetActive(true);
    }

    void FitSpriteToOriginalSize(Sprite newSprite)
    {
        avatarRenderer.sprite = newSprite;

        Vector2 originalSize = avatarRenderer.sprite.bounds.size;
        Vector2 targetSize = new Vector2(1.3f, 1.3f); // scale chuẩn

        Vector3 newScale = new Vector3(
            targetSize.x / originalSize.x,
            targetSize.y / originalSize.y,
            1f
        );

        avatarRenderer.transform.localScale = newScale;
    }

    public void Fire()
    {
        fireEffect.Play();
        readyToChooseTarget = false;
        readyToFire = false;

        DoanTauBullet bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity).GetComponent<DoanTauBullet>();
        bullet.Init(currentTarget, this);
    }
}
