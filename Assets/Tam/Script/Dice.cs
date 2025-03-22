using Fusion;
using TMPro;
using UnityEngine;

public class Dice : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(90, 90, 90) * 10f * Time.deltaTime);    
    }

    public override void FixedUpdateNetwork()
    {
        transform.Rotate(new Vector3(90, 90, 90) * 10f * Runner.DeltaTime);
    }

    public void DestroySelf()
    {
        Runner.Despawn(GetComponent<NetworkObject>());
    }
}
