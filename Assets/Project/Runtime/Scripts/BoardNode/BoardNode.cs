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
    public virtual void ProcessNode(GameObject player, Transform playerTransform)
    {
        StartCoroutine(DelayEndTurn(player));
    }

    IEnumerator DelayEndTurn(GameObject player)
    {
        yield return new WaitForSeconds(0.5f);
        EndTurn(player);
    }

    protected void EndTurn(GameObject player)
    {
        player.GetComponent<NewBoardGameController>().EndTurn();
    }
}