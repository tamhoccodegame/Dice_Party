using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines.Editor;
using Unity.Cinemachine;
using UnityEngine;

public class TNTController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    public string explosionObjectName = "ExplosionFX";
    public float destroyDelay = 0.7f;
    public float triggerDelay = 2f;

    public CinemachineCamera tntCam;
    public CinemachineCamera playerCam;

    private Rigidbody rb;
    private bool hasExploded = false;
    private Transform explosionFX;
    private MeshRenderer mesh;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponentInChildren<MeshRenderer>();

        tntCam = GameObject.Find("TNTCamera")?.GetComponent<CinemachineCamera>();
        playerCam = GameObject.Find("PlayerCamera")?.GetComponent<CinemachineCamera>();

        explosionFX = transform.Find(explosionObjectName);
        if (explosionFX == null)
        {
            explosionFX = FindDeepChild(transform, explosionObjectName);
        }
        if (explosionFX != null)
        {
            explosionFX.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (hasExploded) return;
        if (Input.GetMouseButtonDown(0))
        {
                Explode();

            if (playerCam != null && tntCam != null)
            {
                playerCam.Priority = 20;
                tntCam.Priority = 10;
            }
        }

    }
    void FixedUpdate()
    {
        if (hasExploded) return;

        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        Vector3 move = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (rb != null)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.3f, rb.velocity.z);
            }
        }
    }
    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Transform fx = transform.Find(explosionObjectName);
        if (fx != null)
        {
            fx.SetParent(null);
            fx.gameObject.SetActive(true);
            Destroy(fx.gameObject, destroyDelay);
        }


        gameObject.SetActive(false);
        Destroy(gameObject, destroyDelay);
    }
    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;

            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
