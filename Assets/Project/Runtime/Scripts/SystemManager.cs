using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    public static SystemManager instance;

    public TMP_InputField sessionNameInput;
    private string sessionName;

    public event Action onSceneLoaded;
    public event Action onPlayerListChange;

    public static CustomData customData;


    private void Awake()
    {
        instance = this;

        QualitySettings.vSyncCount = 0; // Tắt VSync
        Application.targetFrameRate = 60; // Lock FPS ở mức vừa phải
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        customData = GetComponent<CustomData>();
        MusicManager.instance.PlayMusic(MusicManager.MusicType.Menu);
    }

    private void Update()
    {
        
    }
   
}
