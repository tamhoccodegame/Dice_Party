using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class BoardNode : MonoBehaviour
{
    public bool isStartNode = false;
    public List<BoardNode> nextNodes;

    public SplineContainer splineContainer;
    public float normalizeTime;

    public ParticleSystem nodeEffect;

    protected Coroutine processCoroutine;

    private void Awake()
    {
        splineContainer = GetComponentInParent<SplineContainer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public virtual void ProcessNode(PlayerInput playerInput, Transform playerTransform)
    {
        StartCoroutine(DelayEndTurn(playerInput));
    }

    IEnumerator DelayEndTurn(PlayerInput playerInput)
    {
        yield return new WaitForSeconds(0.5f);
        EndTurn(playerInput);
    }

    protected void EndTurn(PlayerInput playerInput)
    {
        NewBoardGameController[] players = FindObjectsByType<NewBoardGameController>(FindObjectsSortMode.None);

        foreach (var p in players)
        {
            if(p.playerInput == playerInput)
            p.EndTurn();
            else continue;
        }
    }
}