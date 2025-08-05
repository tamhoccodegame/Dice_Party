using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Unity.Cinemachine;

public class TNT : MonoBehaviour
{
    public GameObject tntPrefab;
    public Transform spawnPoint;
    public GameObject player;
    private bool isDropping = false;
    public CinemachineCamera tntCam;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isDropping)
        {
            SpawnTNT();
            isDropping = true;
        }
    }

    public void SpawnTNT()
    {
        GameObject tnt = Instantiate(tntPrefab, tntPrefab.transform.position, Quaternion.identity);

        if (tntCam != null)
        {
            tntCam.Follow = tnt.transform;
            tntCam.LookAt = tnt.transform;
            tntCam.Priority = 20;
        }
    }



}
