using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class HookController : MonoBehaviour
{
    public Transform hookTransform;
    public Transform playerTransform;
    public float dropSpeed = 3f;
    public float returnSpeed = 5f;
    public float controlSpeed = 5f;
    public float maxDepth = 10f;

    private Vector3 startPos;
    private bool isDropping = false;
    private bool isReturning = false;
    private bool isControlling = false;
    private GameObject hookedFish = null;

    void Start()
    {
        startPos = hookTransform.position;
    }

    void Update()
    {
        // Khi giữ E: điều khiển hook tự do
        if (Input.GetKey(KeyCode.E) && !isDropping && !isReturning)
        {
            isControlling = true;
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(h, 0f, v);
            hookTransform.position += move * controlSpeed * Time.deltaTime;
        }

        // Khi thả E: hook bắt đầu rơi xuống
        if (Input.GetKeyUp(KeyCode.E) && isControlling)
        {
            isControlling = false;
            isDropping = true;
        }

        // Hook rơi xuống
        if (isDropping)
        {
            hookTransform.position += Vector3.down * dropSpeed * Time.deltaTime;

            if (hookTransform.position.y <= startPos.y - maxDepth)
            {
                isDropping = false;
                isReturning = true;
            }
        }

        // Kéo hook về vị trí ban đầu
        if (isReturning)
        {
            hookTransform.position = Vector3.MoveTowards(hookTransform.position, startPos, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(hookTransform.position, startPos) < 0.01f)
            {
                hookTransform.position = startPos;
                isReturning = false;
                hookedFish = null;
            }
        }

        // Kéo cá về player
        if (hookedFish != null)
        {
            hookedFish.transform.position = Vector3.MoveTowards(hookedFish.transform.position, playerTransform.position, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(hookedFish.transform.position, playerTransform.position) < 3f)
            {
                Destroy(hookedFish);
                hookedFish = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDropping && other.CompareTag("Fish") && hookedFish == null)
        {
            hookedFish = other.gameObject;
            isDropping = false;
            isReturning = true;

            Debug.Log("🎣 Bắt được cá!");
        }
    }

}
