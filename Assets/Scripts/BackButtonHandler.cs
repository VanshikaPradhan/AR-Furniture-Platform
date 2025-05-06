using UnityEngine;
using UnityEngine.InputSystem;

public class BackButtonHandler : MonoBehaviour
{
    private SceneNavigator sceneNavigator;

    void Start()
    {
        sceneNavigator = FindObjectOfType<SceneNavigator>();
        if (sceneNavigator == null)
        {
            Debug.LogWarning("BackButtonHandler: SceneNavigator not found in scene.");
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (sceneNavigator != null)
            {
                sceneNavigator.GoBack();
            }
            else
            {
                Debug.LogWarning("BackButtonHandler: No SceneNavigator to handle back action.");
            }
        }
    }
}
