using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapActivationManager : MonoBehaviour
{
    public Transform player;
    public float scanInterval = 0.2f;

    private float timer = 0f;
    private List<TrapActivator> traps = new List<TrapActivator>();

    public void Awake()
    {
        // Tìm tất cả trap trong scene (có thể tối ưu nếu có nhiều)


        Debug.Log($"[🧠 TrapActivationManager] Tìm thấy {traps.Count} traps");
    }

    public void SetPlayer()
    {
        player = FindFirstObjectByType<MNGChayTruongController>().transform;
        TrapActivator[] foundTraps = FindObjectsOfType<TrapActivator>(true); // true để tìm cả những trap bị tắt
        foreach (var trap in foundTraps)
        {
            trap.Init(player);
            traps.Add(trap);
        }
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= scanInterval)
        {
            timer = 0f;
            foreach (var trap in traps)
            {
                trap.CheckActivation();
            }
        }
    }
}
