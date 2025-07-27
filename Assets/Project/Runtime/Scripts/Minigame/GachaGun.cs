using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaGun : MonoBehaviour
{
    public GameObject bulletPrefab;

    public List<Sprite> playerAvatars = new List<Sprite>();
    public Image avatarImage;

    public bool readyToFire = false;
    public bool readyToChooseTarget = false;

    public float spinTime = 3f;


    public void Init(List<Sprite> _playerAvatars)
    {
        playerAvatars = _playerAvatars;
    }

    public void SpinGacha()
    {
        StartCoroutine(GachaCoroutine());
    }

    IEnumerator GachaCoroutine()
    {
        float elapsedTime = 0;

        while(elapsedTime < spinTime)
        {
            avatarImage.sprite = playerAvatars[Random.Range(0, playerAvatars.Count)];
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        avatarImage.sprite = playerAvatars[Random.Range(0, playerAvatars.Count)];

        readyToFire = true;
        DoanTauManager.instance.ReadyToFire();
    }

    public void Fire()
    {
        readyToChooseTarget = false;
        //Spawn Bullet
        //Init Bullet
    }
}
