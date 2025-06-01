using Fusion;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HostLobby : NetworkBehaviour
{
    private NetworkManager networkManager;

    public Transform[] avatarStandingPosition;
    public GameObject[] playerPrefabs;

    public Transform playerSlotTemplate;
    public Transform playerSlotContainer;

    private Dictionary<PlayerRef, NetworkObject> spawnedAvatars = new Dictionary<PlayerRef, NetworkObject>();
    [Networked, Capacity(4)] public NetworkDictionary<PlayerRef, NetworkBool> readyStatus => default;

    private void Awake()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            EnsureReadyStatusInit();
            UpdatePlayerList();
        }
        else
        {
            RPC_RequestUpdatePlayerList();
        }
        networkManager.onPlayerListChange += NetworkManager_onPlayerListChange;
    }

    public async void StartGame()
    {
        RequestApplyCustom();

        await Task.Delay(1000);
        Debug.Log("Runner: " + Runner);

        await NetworkManager.runnerInstance.LoadScene("TuanSceneMap");
    }

    #region UpdatePlayerListUI
    private void NetworkManager_onPlayerListChange()
    {
        if (Object.HasStateAuthority)
        {
            EnsureReadyStatusInit();
            UpdatePlayerList();
        }
        else
        {
            RPC_RequestUpdatePlayerList();
        }
    }
    public void OnClickSetReady(bool ready)
    {
        PlayerRef player = Runner.LocalPlayer;
        RPC_RequestSetReady(player, ready);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestSetReady(PlayerRef player, bool ready)
    {
        readyStatus.Set(player, ready);
        foreach (var kvp in readyStatus)
        {
            Debug.Log($"Player: {kvp.Key} Ready: {kvp.Value}");
        }

        if (Object.HasStateAuthority)
            UpdatePlayerList();
    }

    void EnsureReadyStatusInit()
    {
        if (!Object.HasStateAuthority) return;

        List<PlayerRef> players = networkManager.GetAllPlayers();
        foreach (var player in players)
        {
            if (!readyStatus.ContainsKey(player))
                readyStatus.Add(player, false);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_RequestUpdatePlayerList()
    {
        RPC_UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        List<PlayerRef> players = networkManager.GetAllPlayers();
        players.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));

        foreach (Transform child in playerSlotContainer)
        {
            if (child == playerSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (PlayerRef player in players)
        {
            //Spawn Avatar (model 3D)
            if (!spawnedAvatars.ContainsKey(player))
            {
                var avatar = Runner.Spawn(playerPrefabs[0],
                                          avatarStandingPosition[player.PlayerId - 1].position,
                                          Quaternion.Euler(0, 180, 0), player);
                spawnedAvatars.Add(player, avatar);
            }

            //Spawn Slot UI cho Player
            var p = Instantiate(playerSlotTemplate, playerSlotContainer);
            p.gameObject.SetActive(true);
            p.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = player.PlayerId.ToString();

            PlayerSlotUI playerSlotUI = p.GetComponent<PlayerSlotUI>();
            bool isReady = readyStatus.Get(player);
            var readyPanel = playerSlotUI.readyPanel;
            readyPanel.SetActive(isReady);

            if (Runner.LocalPlayer != player)
            {
                playerSlotUI.unreadyButton.SetActive(false);
                playerSlotUI.unreadyPanel.SetActive(!isReady);
                playerSlotUI.adjustAppearancePanel.SetActive(false);
                playerSlotUI.afterJoinPanel.SetActive(false);
                playerSlotUI.customizePanel.SetActive(false);
            }
        }

        if (Object.HasStateAuthority)
            RPC_UpdatePlayerList();

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdatePlayerList()
    {
        if (Object.HasStateAuthority) return;

        List<PlayerRef> players = networkManager.GetAllPlayers();
        players.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));

        foreach (Transform child in playerSlotContainer)
        {
            if (child == playerSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (PlayerRef player in players)
        {
            var p = Instantiate(playerSlotTemplate, playerSlotContainer);
            p.gameObject.SetActive(true);
            p.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = player.PlayerId.ToString();

            PlayerSlotUI playerSlotUI = p.GetComponent<PlayerSlotUI>();
            bool isReady = readyStatus.Get(player);
            var readyPanel = playerSlotUI.readyPanel;
            readyPanel.SetActive(isReady);

            if (Runner.LocalPlayer != player)
            {
                playerSlotUI.unreadyButton.SetActive(false);
                playerSlotUI.unreadyPanel.SetActive(!isReady);
                playerSlotUI.adjustAppearancePanel.SetActive(false);
                playerSlotUI.afterJoinPanel.SetActive(false);
                playerSlotUI.customizePanel.SetActive(false);
            }
        }
    }
    #endregion

    #region CustomCharacter
    public void NextHair()
    {
        var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        foreach (var p in playerCustoms)
            if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().NextHair();
    }

    public void PrevHair()
    {
        var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        foreach (var p in playerCustoms)
            if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().PrevHair();
    }


    public void NextColor()
    {
        var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        foreach (var p in playerCustoms)
            if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().NextColor();

    }

    public void PrevColor()
    {
        var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        foreach (var p in playerCustoms)
            if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().PrevColor();

    }

    public void NextBodypart()
    {
        var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        foreach (var p in playerCustoms)
            if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().NextBodypart();

    }

    public void PrevBodypart()
    {
        var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        foreach (var p in playerCustoms)
            if (p.HasInputAuthority) p.GetComponent<PlayerCustom>().PrevBodypart();
    }

    public void RequestApplyCustom()
    {
        var playerCustoms = FindObjectsByType<PlayerCustom>(FindObjectsSortMode.None);
        foreach (var p in playerCustoms)
            if (p.HasInputAuthority) p.RequestApplyCustom(p.currentHairIndex, p.currentColorIndex, p.currentBodypartIndex);
    }
    #endregion
}

