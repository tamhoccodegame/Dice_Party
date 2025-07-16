using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    private SystemManager systemManager;

    public Transform[] avatarStandingPosition;
    public GameObject playerPrefab;

    public Transform playerSlotTemplate;
    public Transform playerSlotContainer;

    public GameObject[] hostOwnObjects;
    public Button hostStartButton;


    //private Dictionary<PlayerRef, NetworkObject> spawnedAvatars = new Dictionary<PlayerRef, NetworkObject>();
    //[Networked, Capacity(4)] public NetworkDictionary<PlayerRef, NetworkBool> readyStatus => default;

    private void Awake()
    {
        systemManager = FindFirstObjectByType<SystemManager>();
        EnsureReadyStatusInit();
        UpdatePlayerList();

        foreach (var go in hostOwnObjects)
        {
            go.SetActive(true);
        }
        systemManager.onPlayerListChange += NetworkManager_onPlayerListChange;
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
        UpdatePlayerList();
        UpdatePlayerList();
    }
    public void OnClickSetReady(bool ready)
    {
        //SetRea(player, ready);
    }

    void EnsureReadyStatusInit()
    {

    }

    void UpdatePlayerList()
    {
        //players.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));

        foreach (Transform child in playerSlotContainer)
        {
            if (child == playerSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        //foreach (PlayerRef player in players)
        //{
           
        //        //Spawn Avatar (model 3D)
        //        if (!spawnedAvatars.ContainsKey(player))
        //        {
        //            var avatar = Runner.Spawn(playerPrefab,
        //                                      avatarStandingPosition[player.PlayerId - 1].position,
        //                                      Quaternion.Euler(0, 180, 0), player);
        //            spawnedAvatars.Add(player, avatar);
        //        }

        //    var p = Instantiate(playerSlotTemplate, playerSlotContainer);
        //    p.gameObject.SetActive(true);

        //    //string playerName = BoardGameData.instance.GetName(player);
        //    TextMeshProUGUI playerNameText = p.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        //    if (string.IsNullOrEmpty(playerName))
        //        playerNameText.text = player.PlayerId.ToString();
        //    else
        //        playerNameText.text = playerName;

        //    PlayerSlotUI playerSlotUI = p.GetComponent<PlayerSlotUI>();
        //    //bool isReady = readyStatus.Get(player);
        //    var readyPanel = playerSlotUI.readyPanel;
        //    //readyPanel.SetActive(isReady);

        //    PlayerCustom playerCustom = playerPrefab.GetComponent<PlayerCustom>();

        //    foreach (var hair in playerCustom.hairs)
        //    {
        //        playerSlotUI.AddHairName(hair.name);
        //    }

        //    foreach (var bodypart in playerCustom.bodyparts)
        //    {
        //        playerSlotUI.AddBodypartName(bodypart.name);
        //    }

        //    //if (Runner.LocalPlayer != player)
        //    //{
        //    //    playerSlotUI.unreadyButton.SetActive(false);
        //    //    playerSlotUI.unreadyPanel.SetActive(!isReady);
        //    //    playerSlotUI.adjustAppearancePanel.SetActive(false);
        //    //    playerSlotUI.afterJoinPanel.SetActive(false);
        //    //    playerSlotUI.customizePanel.SetActive(false);
        //    //}
        //}
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

