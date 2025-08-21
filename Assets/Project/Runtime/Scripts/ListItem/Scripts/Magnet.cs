using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Magnet : BoardItem, IRotatableItem
{
    [SerializeField] float rotateSpeed = 90f;
    public GameObject handPrefab;
    public Transform handHoldPoint;

    private Transform playerModel;
    private GameObject currentHand;
    private Animator animator;
    private bool isUsing = false;
    private bool isAttack = false;

    private GameObject effect;
    private NewBoardGameController controller;
    public override void Use(NewBoardGameController controller)
    {
        this.controller = controller;
        handHoldPoint = controller.handSpawnPoint;
        playerModel = controller.GetComponent<Animator>().transform;
        if (handHoldPoint == null)
        {
            Debug.LogError("GunSpawnPoint not assigned in controller!");
            return;
        }
        controller.StartCoroutine(SpawnMagnet());
    }
    private IEnumerator SpawnMagnet()
    {
        currentHand = Instantiate(handPrefab, handHoldPoint.position + new Vector3(0, -0.5f, -1.2f), handHoldPoint.rotation, handHoldPoint);
        currentHand.transform.localRotation = Quaternion.Euler(90, 180, -90);
        isUsing = true;
        animator = currentHand.GetComponentInChildren<Animator>();

        ParticleSystem[] effects = currentHand.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var fx in effects)
        {
            if (fx.name.Contains("MagnetEffect"))
                effect = fx.gameObject;

        }

        if (effect) effect.SetActive(false);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        while (isUsing)
        {
            if (Input.GetMouseButton(0))
            {
                yield return MagnetHandle();
            }

            yield return null;
        }

        isUsing = false;
        Destroy(currentHand);
    }
    public IEnumerator MagnetHandle()
    {
        isAttack = true;
        controller.StartCoroutine(MagnetEffect());
        yield return new WaitForSeconds(5f);
        isAttack = false;
        isUsing = false;

    }
    private IEnumerator MagnetEffect()
    {
        yield return new WaitForSeconds(0.5f);
        if (effect) effect.SetActive(true);
    }
    public void Rotate(float direction)
    {
        if (isAttack)
            return;
        if (playerModel != null)
        {
            playerModel.Rotate(Vector3.up * direction * rotateSpeed * Time.deltaTime);
        }
    }
}
