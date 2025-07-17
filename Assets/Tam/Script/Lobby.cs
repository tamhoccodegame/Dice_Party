using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public Transform[] avatarStandingPosition;
    public GameObject playerPrefab;

    public Transform playerSlotContainer;

    public GameObject[] hostOwnObjects;
    public Button hostStartButton;

    private Dictionary<PlayerInput, GameObject> spawnedAvatars = new Dictionary<PlayerInput, GameObject>();
    private Dictionary<PlayerInput, bool> readyStatus = new Dictionary<PlayerInput, bool>();

    private int playerCount = 0;

    private void Awake()
    {
        EnsureReadyStatusInit();

        foreach (var go in hostOwnObjects)
        {
            go.SetActive(true);
        }

    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public async void StartGame()
    {
        //ApplyCustom();

        await Task.Delay(1000);

        SceneManager.LoadScene("TuanSceneMap");
    }

    #region UpdatePlayerListUI
    private void NetworkManager_onPlayerListChange()
    {
        //ApplyCustom();

        EnsureReadyStatusInit();
    }
    public void OnClickSetReady(bool ready)
    {
        //SetRea(player, ready);
    }

    void EnsureReadyStatusInit()
    {

    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerManager.instance.AddPlayer(playerInput);
        playerInput.transform.SetParent(playerSlotContainer);

        playerInput.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = $"Player {playerCount + 1}";

        //Spawn Avatar (model 3D)
        if (!spawnedAvatars.ContainsKey(playerInput))
        {
            var model = Instantiate(playerPrefab, 
                                    avatarStandingPosition[playerCount].position, 
                                    Quaternion.Euler(0, 180, 0));
            spawnedAvatars.Add(playerInput, model);
            playerCount++;
            playerInput.GetComponent<PlayerSlotUI>().InitSelector(model.GetComponent<PlayerCustom>());
        }

        // Cập nhật UI NOTE: Sửa lại cho nó liên quan giữa char và slot ui. hiện tại nó chưa liên quan?
    }
        #endregion

        #region CustomCharacter
        //public void NextHair()
        //{
        //    var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        //    foreach (var p in playerCustoms)
        //        if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().NextHair();
        //}

        //public void PrevHair()
        //{
        //    var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        //    foreach (var p in playerCustoms)
        //        if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().PrevHair();
        //}


        //public void NextColor()
        //{
        //    var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        //    foreach (var p in playerCustoms)
        //        if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().NextColor();

        //}

        //public void PrevColor()
        //{
        //    var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        //    foreach (var p in playerCustoms)
        //        if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().PrevColor();

        //}

        //public void NextBodypart()
        //{
        //    var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        //    foreach (var p in playerCustoms)
        //        if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().NextBodypart();

        //}

        //public void PrevBodypart()
        //{
        //    var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        //    foreach (var p in playerCustoms)
        //        if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().PrevBodypart();
        //}

        //public void ApplyCustom()
        //{
        //    var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        //    foreach (var p in playerCustoms)
        //        if (p.HasInputAuthority) p.RequestApplyCustom(p.currentHairIndex, p.currentColorIndex, p.currentBodypartIndex);
        //}

        //public void OnClickSetName(TextMeshProUGUI text)
        //{
        //    PlayerRef player = Runner.LocalPlayer;
        //    NetworkString<_16> name = (NetworkString<_16>)text.text;
        //    RPC_RequestSetName(player, name);
        //}

        //public void SetName(PlayerRef player, NetworkString<_16> name)
        //{
        //    BoardGameData.instance.UpdateName(player, (string)name);
        //    if (Object.HasStateAuthority) UpdatePlayerList();
        //}

        //IEnumerator UpdateDelayAfterSetName()
        //{
        //    yield return new WaitForSecondsRealtime(0.3f);
        //    UpdatePlayerList();
        //}

        #endregion
    }

