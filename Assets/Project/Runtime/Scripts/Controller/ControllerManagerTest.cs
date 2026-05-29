using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManagerTest : MonoBehaviour
{
    public PlayerController[] controllers;
    public int currentControllerIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentControllerIndex = 0;
        EnabledController();
    }

    void EnabledController()
    {
        for (int i = 0; i < controllers.Length; i++)
        {
            if (i == currentControllerIndex)
            {
                controllers[i].enabled = true;
            }
            else controllers[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                currentControllerIndex++;
                currentControllerIndex = currentControllerIndex % controllers.Length;
                EnabledController();
            }
        }
    }
        
}
