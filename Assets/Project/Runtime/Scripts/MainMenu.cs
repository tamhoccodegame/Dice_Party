using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        if(PlayerManager.instance != null)
        Destroy(PlayerManager.instance.gameObject);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
