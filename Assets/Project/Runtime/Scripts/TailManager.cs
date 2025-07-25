using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    public List<Animator> tentacleAnimators;

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
            yield return TriggerRandomTentacleAttack();

            yield return new WaitForSeconds(currentDelay);

            currentDelay = Mathf.Max(minDelay, currentDelay - speedUpRate);
        }
    }
    IEnumerator TriggerRandomTentacleAttack()
    {

        if (tentacleAnimators.Count == 0)
            yield break;

        int randomIndex = Random.Range(1, tentacleAnimators.Count);
        Animator selectedTentacle = tentacleAnimators[randomIndex];

        Vector3 warningPos = selectedTentacle.transform.position;
        GameObject warning = Instantiate(warningPrefab, warningPos + Vector3.up * 8.5f, Quaternion.identity);

        yield return new WaitForSeconds(warningDuration);

        Destroy(warning);

        selectedTentacle.CrossFade("Attack", 0.2f);

        yield return new WaitForSeconds(7f);

        selectedTentacle.CrossFade("IdleTails", 0.2f);
    }
    
}
