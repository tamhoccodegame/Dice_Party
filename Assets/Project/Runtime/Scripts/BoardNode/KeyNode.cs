using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyNode : BoardNode
{
    public GameObject keyPrefab;

    //Hàm này tất cả client đều chạy
    public override void ProcessNode(PlayerInput playerInput, Transform playerTransform)
    {
        {
            int keyQty = Random.Range(1, 4); // 1 -> 3 chìa

           StartCoroutine(ProcessCoroutine(playerInput, playerTransform, keyQty));
        }
    }

    IEnumerator ProcessCoroutine(PlayerInput playerInput, Transform playerTransform, int keyQty)
    {
        List<bool> collected = new List<bool>();

        for (int i = 0; i < keyQty; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * 1f;
            randomOffset.y = Mathf.Abs(randomOffset.y);

            GameObject key = Instantiate(
                keyPrefab,
                transform.position + Vector3.up * 0.5f + randomOffset,
                Quaternion.identity
            );

            // 💥 Add force để văng ra như explosion
            Rigidbody rb = key.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDir = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.4f, 1.2f),
                    Random.Range(-1f, 1f)
                ).normalized;

                float explosionForce = 30f;
                rb.AddForce(randomDir * explosionForce, ForceMode.Impulse);
            }

            // Bắt đầu bay về player
            KeyPickupMover mover = key.GetComponent<KeyPickupMover>();

            mover.Init(playerTransform, () =>
            {
                WizardPartyData.instance.UpdatePlayerKey(playerInput, 1);
                TurnManager.instance.UpdatePlayerDataUI();
                collected.Add(true);
            });
        }
        yield return new WaitUntil(() => collected.Count >= keyQty);
        yield return new WaitForSeconds(0.15f);


        yield return new WaitForSeconds(0.3f); // Delay nhẹ cho mượt
        EndTurn(playerInput);
    }
}
