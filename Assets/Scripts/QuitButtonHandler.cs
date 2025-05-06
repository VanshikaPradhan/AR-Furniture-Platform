using UnityEngine;

public class QuitButtonHandler : MonoBehaviour
{
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApp();
            }
        }
    }
    public void QuitApp()
    {
        Debug.Log("👋 Quit button clicked!");

#if UNITY_EDITOR
        // Stop play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        // Close Android app
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call("finishAndRemoveTask");
#elif UNITY_IOS
        // On iOS, quitting programmatically is discouraged, but still:
        Application.Quit();
#else
        // Windows, macOS, Linux
        Application.Quit();
#endif
    }
}
