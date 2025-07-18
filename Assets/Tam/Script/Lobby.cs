using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    public static Lobby instance;

    public Transform[] avatarStandingPosition;
    public GameObject playerPrefab;

    public Transform playerSlotContainer;
    public Transform[] playerSlots;

    private Dictionary<PlayerInput, GameObject> spawnedAvatars = new Dictionary<PlayerInput, GameObject>();
    private Dictionary<PlayerInput, bool> readyStatus = new Dictionary<PlayerInput, bool>();

    private int playerCount = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("TestSpawnPlayerAfterLobby");
        //SceneManager.LoadScene("TuanSceneMap");

    }

    public void SetReady(PlayerInput playerInput, bool ready)
    {
        readyStatus[playerInput] = ready;
        Debug.Log($"Dictionary Count: {readyStatus.Count} {playerInput} status: {ready}");

        if (CheckAllReady())
        {
            StartCoroutine(StartGame());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    bool CheckAllReady()
    {
        return readyStatus.All(r => r.Value == true);
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerManager.instance.AddPlayer(playerInput);
        playerInput.transform.SetParent(playerSlotContainer);

        playerInput.uiInputModule = playerSlots[playerCount].GetComponent<PlayerSlotUI>()
                                    .inputSystemUIInputModule;

        Debug.Log(playerSlots[playerCount].GetComponent<PlayerSlotUI>().name);  

        //playerInput.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = $"Player {playerCount + 1}";

        //Spawn Avatar (model 3D)
        if (!spawnedAvatars.ContainsKey(playerInput))
        {
            var model = Instantiate(playerPrefab,
                                    avatarStandingPosition[playerCount].position,
                                    Quaternion.Euler(0, 180, 0));
            spawnedAvatars.Add(playerInput, model);
            playerSlots[playerCount].gameObject.SetActive(true);
            playerSlots[playerCount].GetComponent<PlayerSlotUI>().InitSelector(model.GetComponent<PlayerCustom>());
            playerCount++;
            model.GetComponent<PlayerCustom>().Init(playerInput);
        }

        readyStatus.Add(playerInput, false);
        StopAllCoroutines();
    }
}

