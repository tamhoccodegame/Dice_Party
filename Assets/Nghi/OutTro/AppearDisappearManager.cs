using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearDisappearManager : MonoBehaviour
{

    public PopUpAppearUI_Extended panelA;
    public PopUpAppearUI_Extended panelB;

    public Button switchButton;

    void Start()
    {
        switchButton.onClick.AddListener(SwitchPanels);
    }

    void SwitchPanels()
    {
        panelA.PlayDisappear(() => {
            // Sau khi PanelA ẩn xong thì PanelB mới hiện ra
            panelB.PlayAppear();
        });
    }
}
