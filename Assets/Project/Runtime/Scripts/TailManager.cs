using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    public List<Animator> tentacleAnimators;
    public List<CrateRowManager> crateRows;

    public float startDelay = 2f;
    public float minDelay = 0.3f;
    public float speedUpRate = 0.05f;
    public float battleStartDelay = 3f;
    public float warningDuration = 0.8f;
    public GameObject warningPrefab;
    void Start()
    {

        StartCoroutine(LoopTentacleAttacks());
    }

    IEnumerator LoopTentacleAttacks()
    {
        yield return new WaitForSeconds(battleStartDelay);

        float currentDelay = startDelay;

        while (true)
        {
            yield return StartCoroutine(TriggerRandomTentacleAttack());

            yield return new WaitForSeconds(currentDelay);
            currentDelay = Mathf.Max(minDelay, currentDelay - speedUpRate);
        }
    }
    IEnumerator TriggerRandomTentacleAttack()
    {

        if (tentacleAnimators.Count == 0 || crateRows.Count == 0)
            yield break;


        List<int> selectedIndices = new List<int>();
        while (selectedIndices.Count < 3 && selectedIndices.Count < tentacleAnimators.Count)
        {
            int index = Random.Range(0, tentacleAnimators.Count);
            if (!selectedIndices.Contains(index))
            {
                selectedIndices.Add(index);
            }
        }

        List<GameObject> allWarnings = new List<GameObject>();


        foreach (int index in selectedIndices)
        {
            CrateRowManager row = crateRows[index];
            foreach (Transform crate in row.cratesInRow)
            {
                GameObject warning = Instantiate(warningPrefab, crate.position + Vector3.up * 5f, Quaternion.identity);
                allWarnings.Add(warning);
            }
        }

        yield return new WaitForSeconds(warningDuration);


        foreach (var w in allWarnings)
        {
            Destroy(w);
        }



        foreach (int index in selectedIndices)
        {
            tentacleAnimators[index].CrossFade("Attack", 0.2f);
        }

        yield return new WaitForSeconds(7f);


        foreach (int index in selectedIndices)
        {
            tentacleAnimators[index].CrossFade("IdleTails", 0.2f);
        }
    }
}
