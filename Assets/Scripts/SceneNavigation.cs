using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    public void GoToScene(string sceneName)
    {
        // Save the current scene before changing
        if (SceneManager.GetActiveScene().name != "ARScene")
        {
            SceneHistoryStack.PushScene(SceneManager.GetActiveScene().name);
        }
        SceneManager.LoadScene(sceneName);
    }

    

    public void GoBack()
    {
        string previousScene = SceneHistoryStack.PopScene();
        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("No previous scene found in history!");
        }
    }
}
