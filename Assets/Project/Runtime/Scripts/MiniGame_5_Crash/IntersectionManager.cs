using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionManager : MonoBehaviour
{
    //public static IntersectionManager Instance;

    //// Vùng giao nhau (tâm bàn cờ)
    //public Vector3 intersectionCenter = Vector3.zero;
    //public float intersectionRadius = 1.5f; // bán kính vùng check

    //private Queue<Wave_AI> queue = new Queue<Wave_AI>();
    //private HashSet<Wave_AI> inside = new HashSet<Wave_AI>();

    //void Awake()
    //{
    //    if (Instance == null) Instance = this;
    //    else Destroy(gameObject);
    //}

    //void Update()
    //{
    //    // Nếu không ai trong vùng → cấp quyền cho enemy kế tiếp trong queue
    //    if (inside.Count == 0 && queue.Count > 0)
    //    {
    //        Wave_AI next = queue.Dequeue();
    //        next.AllowPass(true);
    //        inside.Add(next);
    //        Debug.Log($"[Intersection] {next.name} GRANTED to pass intersection.");
    //    }

    //    // Clear enemy đã ra khỏi vùng
    //    List<Wave_AI> toRemove = new List<Wave_AI>();
    //    foreach (var ai in inside)
    //    {
    //        float dist = Vector3.Distance(ai.transform.position, intersectionCenter);
    //        if (dist > intersectionRadius * 2f) // ra xa hẳn
    //        {
    //            toRemove.Add(ai);
    //        }
    //    }
    //    foreach (var ai in toRemove) inside.Remove(ai);
    //}

    //public void RequestPass(Wave_AI ai)
    //{
    //    // Nếu đã trong queue hoặc đang được cấp quyền thì thôi
    //    if (queue.Contains(ai) || inside.Contains(ai)) return;

    //    queue.Enqueue(ai);
    //    ai.AllowPass(false);
    //    Debug.Log($"[Intersection] {ai.name} REQUEST pass, added to queue.");
    //}
}
