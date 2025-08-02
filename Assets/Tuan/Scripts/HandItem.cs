using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HandItem : MonoBehaviour
{
    [SerializeField] public float rotateSpeed;
    public GameObject handPrefab;
    public Transform handHoldPoint;
    public GameObject player;
    public CharacterController controller;

    private GameObject currentHand;
    private Animator animator;
    private bool isUsingHand = false;

    private GameObject chargeEffect;
    private GameObject impactEffect;


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F) && !isUsingHand)
        {
            SpawnHand();
        }
        if (isUsingHand)
        {
            RotatePlayerToMouse();
           
        }
        if (isUsingHand && Input.GetMouseButtonDown(0))
        {
            Smash();
        }
    }

    void SpawnHand()
    {
        currentHand = Instantiate(handPrefab, handHoldPoint.position, handHoldPoint.rotation, handHoldPoint);
        currentHand.transform.localRotation = Quaternion.Euler(180, 180, 0);

        animator = currentHand.GetComponentInChildren<Animator>();

        ParticleSystem[] effects = currentHand.GetComponentsInChildren<ParticleSystem>();
        foreach (var fx in effects)
        {
            if (fx.gameObject.name.Contains("Charge"))
                chargeEffect = fx.gameObject;
            else if (fx.gameObject.name.Contains("Impact"))
                impactEffect = fx.gameObject;
        }

        ResetEffects();
        if (controller != null)
            controller.enabled = false;
        isUsingHand = true;
    }
    void ResetEffects()
    {
        if (chargeEffect != null)
            chargeEffect.SetActive(false);
        if (impactEffect != null)
            impactEffect.SetActive(false);
    }
    void Smash()
    {
        if (animator != null)
            animator.CrossFade("HandAni", 0.2f);
        StartCoroutine(ChargeEffect());
        StartCoroutine(OnHandImpact());
    }
    IEnumerator ChargeEffect()
    {
        if (chargeEffect != null)
        {
            yield return new WaitForSeconds(0.5f);
            chargeEffect.SetActive(true);
        }
    }
    IEnumerator OnHandImpact()
    {
        if (impactEffect != null)
        {
            yield return new WaitForSeconds(1.2f);
            impactEffect.SetActive(true);
        }

    }
    void RotatePlayerToMouse()
    {
        float rotateInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            rotateInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            rotateInput = 1f;

        if (rotateInput != 0f)
        {
            player.transform.Rotate(Vector3.up, rotateSpeed * rotateInput * Time.deltaTime);
        }
    }
}
