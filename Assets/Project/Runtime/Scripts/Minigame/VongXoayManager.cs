using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class VongXoayManager : WizardMiniGameManager
{
    public static VongXoayManager instance;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        if (PlayerManager.instance == null) return;
        UpdateHUD();
    }

    public override bool CheckGameOver()
    {
        return time <= 0;
    }
}
