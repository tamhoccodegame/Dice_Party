using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NhayLopManager : WizardMiniGameManager
{
    public static NhayLopManager instance;

    public float time;

    protected override void Awake()
    {
        instance = this;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        time -= Time.deltaTime;
    }

    public override bool CheckGameOver()
    {
        return time <= 0;
    }

    public override void ShowGameOverPanel()
    {
        base.ShowGameOverPanel();
    }

    public override void UpdateHUD()
    {
        int index = 0;
        foreach (var kvp in playerInitLives)
        {
            if (index < playerTextUI.Length)
            {
                playerTextUI[index].text = kvp.Value.ToString();
                index++;
            }
        }
    }
}
