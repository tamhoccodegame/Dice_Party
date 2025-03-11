using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<PlayerController> playerController;
    public int currentPlayerIndex;
    public Camera cam;
    public Vector3 camOffset;
    public Transform currentFollowTarget;
    private Coroutine changeCamCoroutine;

    public TextMeshProUGUI turnNotifyText;

    public enum GameState
    {
        BoardGame,
        MiniGame
    }
    public GameState currentState;  

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cam = Camera.main;
        playerController = FindObjectsByType<PlayerController>(FindObjectsSortMode.None).ToList();
        StartFirstTurn();
    }

    private void Update()
    {
        if(changeCamCoroutine == null)
        {
            Vector3 lookAtPosition = playerController[currentPlayerIndex].transform.position;
            cam.transform.position = lookAtPosition + camOffset;
        }
    }

    void StartFirstTurn()
    {
        currentPlayerIndex = 0;
        turnNotifyText.text = $"{playerController[currentPlayerIndex].name}'s Turn";
        turnNotifyText.gameObject.SetActive(true);
        playerController[currentPlayerIndex].StartTurn();
    }

    public void NextTurn()
    {
        changeCamCoroutine = StartCoroutine(ChangeFollowTarget());
    }

    IEnumerator ChangeFollowTarget()
    {
        cam.GetComponent<CinemachineBrain>().enabled = false;

        Vector3 oldTarget = playerController[currentPlayerIndex].transform.position + camOffset;

        currentPlayerIndex++;
        if (currentPlayerIndex >= playerController.Count) currentPlayerIndex = 0;
        turnNotifyText.text = $"{playerController[currentPlayerIndex].name}'s Turn";
        turnNotifyText.gameObject.SetActive(true);

        Vector3 newTarget = playerController[currentPlayerIndex].transform.position + camOffset;

        yield return null;

        float elapsedTime = 0f;
        float duration = Vector3.Distance(oldTarget, newTarget) / 10f;

        while (elapsedTime < duration)
        {
            cam.transform.position = Vector3.Lerp(oldTarget, newTarget, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = newTarget;
        cam.GetComponent<CinemachineBrain>().enabled = true;

       
        playerController[currentPlayerIndex].StartTurn();
        changeCamCoroutine = null;
    }
}
