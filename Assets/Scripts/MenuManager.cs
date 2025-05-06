using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject outsideBackground;

    public void ToggleMenu()
    {
        bool isActive = menuPanel.activeSelf;

        menuPanel.SetActive(!isActive);
        outsideBackground.SetActive(!isActive);
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        outsideBackground.SetActive(false);
    }
}
