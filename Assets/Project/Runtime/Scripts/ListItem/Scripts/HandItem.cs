//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.VFX;
//public class HandItem : BoardItem, IRotatableItem
//{
//    [SerializeField] public float rotateSpeed;
//    public GameObject handPrefab;
//    public Transform handHoldPoint;

//    private GameObject currentHand;
//    private Transform playerModel;
//    private Animator animator;
//    private bool isUsingHand = false;
//    private bool isAttack = false;

//    private GameObject chargeEffect;
//    private GameObject impactEffect;
//    private NewBoardGameController controller;

//    public override void Use(NewBoardGameController controller)
//    {
//        this.controller = controller;
//        handHoldPoint = controller.handSpawnPoint;
//        playerModel = controller.GetComponent<Animator>().transform;
//        if (handHoldPoint == null)
//        {
//            Debug.LogError("GunSpawnPoint not assigned in controller!");
//            return;
//        }
//        controller.StartCoroutine(SpawnHand());
//    }

//    private IEnumerator SpawnHand()
//    {
//        currentHand = Instantiate(handPrefab, handHoldPoint.position + new Vector3(0,-1,0), handHoldPoint.rotation, handHoldPoint);
//        currentHand.transform.localRotation = Quaternion.Euler(180, 90, 180);
//        isUsingHand = true;
//        animator = currentHand.GetComponentInChildren<Animator>();

//        ParticleSystem[] effects = currentHand.GetComponentsInChildren<ParticleSystem>(true);
//        foreach (var fx in effects)
//        {
//            if (fx.name.Contains("Charge"))
//                chargeEffect = fx.gameObject;
//            else if (fx.name.Contains("Impact"))
//                impactEffect = fx.gameObject;
//        }

//        if (chargeEffect) chargeEffect.SetActive(false);
//        if (impactEffect) impactEffect.SetActive(false);
//        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
//        while (isUsingHand)
//        {
//            if (Input.GetMouseButton(0))
//            {
//                yield return Smash();
//            }

//            yield return null;
//        }

//        isUsingHand = false;
//        Destroy(currentHand);
//    }

//    public IEnumerator Smash()
//    {
//        isAttack = true;
//        animator.CrossFade("HandAni", 0.2f);
//        controller.StartCoroutine(ChargeEffect());
//        controller.StartCoroutine(OnHandImpact());
//        yield return new WaitForSeconds(5f);
//        isAttack = false;
//        isUsingHand= false;

//    }

//    private IEnumerator ChargeEffect()
//    {
//        yield return new WaitForSeconds(0.5f);
//        if (chargeEffect) chargeEffect.SetActive(true);
//    }
//    private IEnumerator OnHandImpact()
//    {
//        yield return new WaitForSeconds(1.2f);
//        if (impactEffect) impactEffect.SetActive(true);
//    }
//    public void Rotate(float direction)
//    {
//        if (isAttack)
//            return;
//        if (playerModel != null)
//        {
//            playerModel.Rotate(Vector3.up * direction * rotateSpeed * Time.deltaTime);
//        }
//    }
//}
