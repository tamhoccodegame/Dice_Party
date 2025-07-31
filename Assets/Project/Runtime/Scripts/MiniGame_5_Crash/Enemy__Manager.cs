using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy__Manager : MonoBehaviour
{
    public Enemy__Spawner spawner;
    public TMP_Text timerText;
    public float waveInterval = 10f;

    private float timeSurvived = 0f;
    private float nextWaveTime;

    void Start()
    {
        nextWaveTime = waveInterval;
    }

    void Update()
    {
        timeSurvived += Time.deltaTime;
        timerText.text = timeSurvived.ToString("F1") + "s";

        if (timeSurvived >= nextWaveTime)
        {
            spawner.NextWave();
            nextWaveTime += waveInterval;
        }
    }
}
