using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BoardNode : NetworkBehaviour
{
    public bool isStartNode = false;
    public List<BoardNode> nextNodes;

    public ParticleSystem nodeEffect;

    public enum EventType
    {
        None,
        Key,
        Heal,
        RareChest,
        GoldChest,
    }

    public EventType eventType;

    public GameObject keyPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ProcessNode(PlayerRef player)
    {
        //if(eventType == EventType.None)
        //nodeEffect.Play();
        //else
        {
            EndTurn(player);
        }

            switch (eventType)
            {
                case EventType.Key:
                    //StartCoroutine(AddKeyCoroutine(player));
                    Debug.Log("Add Key");
                    break;
                case EventType.Heal:
                    //TurnManager.instance.RequestUpdateHealth(player, 20);
                    Debug.Log("Heal");
                    break;
            }
    }

    void EndTurn(PlayerRef player)
    {
        BoardGameController[] players = FindObjectsByType<BoardGameController>(FindObjectsSortMode.None);

        foreach (var p in players)
        {
            if (p.Object.InputAuthority == player)
            {
                p.EndTurn();
            }
            else continue;
        }
    }

    IEnumerator AddKeyCoroutine(PlayerRef player)
    {
        for(int i = 0; i < 10; i++)
        {
            if (keyPrefab == null) break;
            var rb = Instantiate(keyPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * 10f);
        }

        yield return new WaitForSecondsRealtime(3f);
        TurnManager.instance.RequestUpdateKey(player, 5);
        EndTurn(player);
    }

}