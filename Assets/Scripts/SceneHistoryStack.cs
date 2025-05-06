using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHistoryStack : MonoBehaviour
{
    public static SceneHistoryStack Instance { get; private set; }

    private static Stack<string> sceneStack = new Stack<string>();

    public static void PushScene(string sceneName)
    {
        sceneStack.Push(sceneName);
    }

    public static void PushCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        sceneStack.Push(currentSceneName);
    }
    public static string PopScene()
    {
        if (sceneStack.Count > 0)
        {
            return sceneStack.Pop();
        }
        else
        {
            Debug.LogWarning("SceneHistoryStack: No previous scene to pop.");
            return null;
        }
    }

    public static void ClearHistory()
    {
        sceneStack.Clear();
    }
}
