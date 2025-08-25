using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class TYuanMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float rotationSpeed = 360f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float smoothTime = 0.05f;
    public GameObject splashEffectPrefab;

    private Vector3 spawnPoint;
    private Animator animator;
    private string currentAnim = "";
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 currentMove;
    private Vector3 velocitySmooth;
    private bool canTakeDamage = false;

    public AudioSource audioSource;
    public AudioClip audioClip;

    public int maxHealth = 3;
    private int currentHealth;

    private bool isDead = false;
    private bool isInvincible = false;
    public float invincibleTime = 2f;

    private Renderer[] renderers;
    void Start()
    {

        spawnPoint = transform.position + Vector3.up * 1f;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        renderers = GetComponentsInChildren<Renderer>();
        StartCoroutine(EnableDamageAfterDelay());
    }

    void Update()
    {
        if (!controller.enabled || isDead) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(x, 0f, z).normalized;
        currentMove = Vector3.SmoothDamp(currentMove, inputDir, ref velocitySmooth, smoothTime);

        if (inputDir == Vector3.zero && currentMove.magnitude < 0.01f)
        {
            PlayAnim("Idle");
            currentMove = Vector3.zero;
            velocitySmooth = Vector3.zero;
        }
        else
        {
            PlayAnim("Run");
        }
        controller.Move(currentMove * moveSpeed * Time.deltaTime);
        if (currentMove != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(currentMove, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
/*        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            animator.CrossFade("Jump", 0.2f);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }*/

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    void PlayAnim(string animName)
    {
        if (currentAnim == animName) return;
        animator.CrossFade(animName, 0.2f);
        currentAnim = animName;
    }
    void OnTriggerEnter(Collider other)
    {
        //if (!canTakeDamage || isInvincible) return;

        if (other.CompareTag("Water"))
        {
            SpawnSplashEffect();
            audioSource.PlayOneShot(audioClip, 1f);
            TakeDamage(1);

            if (currentHealth > 0)
            {
                StartCoroutine(Respawn());
            }
        }
    }
    void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityFlash());
    }
    IEnumerator Respawn()
    {
        Transform randomCrate = GetRandomActiveCratePosition();
        if (randomCrate != null)
        {
            controller.enabled = false;
            yield return null;
            transform.position = randomCrate.position + Vector3.up * 1f;
            controller.enabled = true;
        }
    }
    IEnumerator InvincibilityFlash()
    {
        isInvincible = true;

        float elapsed = 0f;
        float flashInterval = 0.4f;

        while (elapsed < invincibleTime)
        {
            foreach (Renderer r in renderers)
            {
                r.enabled = !r.enabled;
            }

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }


        foreach (Renderer r in renderers)
            r.enabled = true;

        isInvincible = false;
    }
    void Die()
    {
        isDead = true;
        controller.enabled = false;
        PlayAnim("Die");
    }
    IEnumerator EnableDamageAfterDelay()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(0.5f);
        canTakeDamage = true;
    }
    Transform GetRandomActiveCratePosition()
    {
        GameObject[] allCrates = GameObject.FindGameObjectsWithTag("Crate");
        List<Transform> activeCrates = new List<Transform>();

        foreach (GameObject crate in allCrates)
        {
            if (crate.activeInHierarchy)
            {
                activeCrates.Add(crate.transform);
            }
        }

        if (activeCrates.Count == 0)
        {
            Debug.LogWarning("No active crates to respawn on!");
            return null;
        }

        int randIndex = Random.Range(0, activeCrates.Count);
        return activeCrates[randIndex];
    }
    void SpawnSplashEffect()
    {
        if (splashEffectPrefab != null)
        {
            Vector3 spawnPos = groundCheck.position + Vector3.up * 2f;
            GameObject splash = Instantiate(splashEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(splash, 2f);
        }
    }
}
