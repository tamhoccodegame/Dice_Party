//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//public class Horse : BoardItem
//{
//    public GameObject horsePrefab;
//    public Transform spawnPoint;
//    public Transform player;

//    private bool isUsing = false;
//    private bool isAttack = false;

//    private BoardPath boardPath;
//    private int currentIndex = 0;
//    private GameObject currentHorse;
//    private NewBoardGameController controller;

//    public override void Use(NewBoardGameController controller)
//    {
//        this.controller = controller;
//        player = controller.transform;
//        boardPath = FindAnyObjectByType<BoardPath>();
//        if (spawnPoint == null)
//        {
//            Debug.LogError("HorseSpawnPoint not assigned in controller!");
//            return;
//        }
//        controller.StartCoroutine(SpawnHorse());
//    }
//    public IEnumerator SpawnHorse()
//    {
//        if (currentHorse != null) yield break;

//        Transform firstSlot = boardPath.GetSlot(currentIndex);
//        if (firstSlot == null)
//        {
//            Debug.LogError("Path chưa có slot!");
//            yield break;
//        }
//        currentHorse = Instantiate(horsePrefab, spawnPoint.position + new Vector3(0, -2, 0), spawnPoint.rotation);


//        yield return new WaitForSeconds(0.5f);

//        Transform mountPoint = currentHorse.transform.Find("MountPoint");
//        if (mountPoint == null)
//        {
//            Debug.LogError("Ngựa chưa có MountPoint!");
//            yield break;
//        }

//        // Đặt player vào ngồi
//        player.SetParent(mountPoint);
//        player.localPosition = Vector3.zero;
//        player.localRotation = Quaternion.identity;
//        player.GetComponent<PlayerController>().enabled = false;
//        isUsing = true;

//        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
//        while (isUsing)
//        {
//            if (Input.GetMouseButtonDown(0) && !isAttack)
//            {
//                yield return controller.StartCoroutine(HorseHandle());
//            }
//            yield return null;
//        }

//    }
//    private IEnumerator HorseHandle(int stepCount = 7, float speed = 10f, float rotateSpeed = 10f)
//    {
//        isAttack = true;

//        currentIndex = boardPath.GetNearestSlotIndex(currentHorse.transform.position);
//        currentHorse.transform.position = boardPath.GetSlot(currentIndex).position;

//        List<Vector3> pathPoints = new List<Vector3>();
//        for (int step = 1; step <= stepCount; step++)
//        {
//            int nextIndex = (currentIndex + step) % boardPath.totalSlots;
//            pathPoints.Add(boardPath.GetSlot(nextIndex).position);
//        }

//        int segIndex = 0;
//        Vector3 start = boardPath.GetSlot(currentIndex).position;
//        Vector3 end = pathPoints[0];

//        while (segIndex < pathPoints.Count)
//        {
//            float segmentLength = Vector3.Distance(start, end);
//            float t = 0f;

//            while (t < 1f)
//            {
//                t += Time.deltaTime * speed / segmentLength;

//                // ease in out (tăng tốc rồi giảm tốc)
//                float easedT = EaseInOutQuad(t);
//                Vector3 newPos = Vector3.Lerp(start, end, easedT);

//                // xoay hướng ngựa
//                Vector3 dir = (newPos - currentHorse.transform.position).normalized;
//                if (dir.sqrMagnitude > 0.0001f)
//                {
//                    Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
//                    targetRot *= Quaternion.Euler(0f, -90f, 0f);
//                    currentHorse.transform.rotation = Quaternion.Slerp(
//                        currentHorse.transform.rotation,
//                        targetRot,
//                        Time.deltaTime * rotateSpeed
//                    );
//                }

//                currentHorse.transform.position = newPos;
//                yield return null;
//            }

//            segIndex++;
//            if (segIndex < pathPoints.Count)
//            {
//                start = end;
//                end = pathPoints[segIndex];
//            }
//        }

//        currentIndex = (currentIndex + stepCount) % boardPath.totalSlots;
//        isAttack = false;
//        Debug.Log("Ngựa dừng di chuyển");
//        player.SetParent(null);
//        player.GetComponent<PlayerController>().enabled = true;
//        Destroy(currentHorse);
//    }
//    private float EaseInOutQuad(float x)
//    {
//        if (x < 0f) return 0f;
//        if (x > 1f) return 1f;

//        return x < 0.5f
//            ? 2f * x * x
//            : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
//    }

//}
