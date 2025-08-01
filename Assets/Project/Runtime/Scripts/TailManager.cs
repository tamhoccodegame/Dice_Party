using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TentacleState
{
    public Animator animator;
    [HideInInspector] public bool isCoolingDown = false;
}
public class TailManager : MonoBehaviour
{
    public List<TentacleState> tentacles;
    public List<CrateRowManager> crateRows;

    public float startDelay = 1f;
    public float minDelay = 0.5f;
    public float speedUpRate = 0.1f;
    public float battleStartDelay = 3f;
    public float warningDuration = 0.5f;
    public float pauseAfter3Attacks = 3f;
    public GameObject warningPrefab;
    void Start()
    {

        StartCoroutine(LoopTentacleAttacks());
    }

    IEnumerator LoopTentacleAttacks()
    {
        yield return new WaitForSeconds(battleStartDelay);

        int attackCounter = 0;
        float currentDelay = startDelay;
        while (true)
        {
            StartCoroutine(TriggerRandomTentacleAttack());
            yield return new WaitForSeconds(currentDelay);

            attackCounter++;
            currentDelay = Mathf.Max(minDelay, currentDelay - speedUpRate);

            if (attackCounter >= 3)
            {
                yield return new WaitForSeconds(pauseAfter3Attacks);
                attackCounter = 0;
            }
        }
    }
    int lastIndex = -1;
    IEnumerator TriggerRandomTentacleAttack()
    {
        if (tentacles.Count == 0 || crateRows.Count == 0)
            yield break;


        List<int> availableIndices = new List<int>();
        for (int i = 0; i < tentacles.Count; i++)
        {
            if (!tentacles[i].isCoolingDown)
                availableIndices.Add(i);
        }

        if (availableIndices.Count == 0)
            yield break;
        int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];

        var tentacle = tentacles[randomIndex];
        var row = crateRows[randomIndex];
        tentacle.isCoolingDown = true;

        List<GameObject> warnings = new List<GameObject>();
        foreach (Transform crate in row.cratesInRow)
        {
            GameObject warning = Instantiate(warningPrefab, crate.position + Vector3.up * 5f, Quaternion.identity);
            warnings.Add(warning);
        }

        yield return new WaitForSeconds(warningDuration);


        foreach (var w in warnings)
        {
            Destroy(w);
        }

        tentacle.animator.CrossFade("Attack", 0.2f);

        yield return new WaitForSeconds(7f);

        tentacle.animator.CrossFade("IdleTails", 0.2f);

        tentacle.isCoolingDown = false;
    }
}
