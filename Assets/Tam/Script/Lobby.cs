using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lobby : NetworkBehaviour
{
    public static Lobby instance;

    public Transform lobbySlotContainer;
    public Transform lobbySlotTemplate;

    [Networked, Capacity(8)]
    public NetworkLinkedList<NetworkString<_16>> players { get; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public override void Spawned()
    {
        Invoke(nameof(AddPlayer), 2f);
    }

    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void AddPlayer()
    {
        

        if (players.Count < 8) // Giới hạn 8 slot
        {
            players.Add("Alabatrap"); // Add chứ không phải Set
        }


        RefreshLobbyUI();
    }

    public void RefreshLobbyUI()
    {
        foreach (Transform child in lobbySlotContainer.transform)
        {
            if (child == lobbySlotTemplate) continue;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count; i++)
        {
            string playerName = players.Get(i).ToString();

            if (!string.IsNullOrEmpty(playerName))
            {
                var slot = Instantiate(lobbySlotTemplate, lobbySlotContainer);
                slot.gameObject.SetActive(true);
                slot.GetComponent<TextMeshProUGUI>().text = playerName;
            }
        }

    }

}
