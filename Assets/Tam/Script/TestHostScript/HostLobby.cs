using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostLobby : NetworkBehaviour
{
    private NetworkManager networkManager;

    public Transform[] avatarStandingPosition;
    public GameObject[] playerPrefabs;

    public Transform playerSlotTemplate;
    public Transform playerSlotContainer;

    private void Awake()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();
    }

    public override void Spawned()
    {
        Invoke(nameof(RPC_UpdatePlayerList),0.5f);
        networkManager.onPlayerListChange += NetworkManager_onPlayerListChange;
    }

    private void NetworkManager_onPlayerListChange()
    {
        RPC_RequestUpdatePlayerList();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_RequestUpdatePlayerList()
    {
        if (!HasStateAuthority)
        {
            RPC_UpdatePlayerList();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdatePlayerList()
    {
        List<PlayerRef> players = networkManager.GetAllPlayers();
       
        players.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));

        foreach (PlayerRef player in players)
        {
            var avatar = Runner.Spawn(playerPrefabs[player.PlayerId - 1], 
                                            avatarStandingPosition[player.PlayerId - 1].position, 
                                            Quaternion.identity, player);
            avatar.GetComponent<CharacterController>().enabled = false;
            avatar.GetComponent<PlayerController>().enabled = false;
            avatar.GetComponent<PlayerSetup>().enabled = false;

            PlayerCustom playerCustom = avatar.GetComponent<PlayerCustom>();
            playerCustom.enabled = true;

            var p = Instantiate(playerSlotTemplate, playerSlotContainer);
            p.gameObject.SetActive(true);
            p.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = player.PlayerId.ToString();

            //Lần lượt gán các event cho tương tác với playercustom
            //p.transform.Find("NextButton").GetComponent<Button>().onClick.AddListener(() => { playerCustom.NextHair(); });
        }
    }

    public async void StartGame()
    {
        PlayerCustom[] playerCustoms = FindObjectsOfType<PlayerCustom>();
        foreach (PlayerCustom playerCustom in playerCustoms)
        {
            playerCustom.RPC_ApplyCustom();
        }

        await Task.Delay(1000);

        await Runner.LoadScene("BoardScene");
    }

    
   
}

