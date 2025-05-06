using UnityEngine;

public static class ProjectNameStore
{
    /// <summary>
    /// Retrieves the saved project name for the given file name.
    /// </summary>
    /// <param name="fileName">The name of the screenshot file (e.g., "project1.png")</param>
    /// <returns>The saved project name or empty string if not set.</returns>
    public static string GetProjectName(string fileName)
    {
        return PlayerPrefs.GetString("ProjectName_" + fileName, "");
    }

    /// <summary>
    /// Saves the custom project name using the file name as the key.
    /// </summary>
    /// <param name="fileName">The name of the screenshot file.</param>
    /// <param name="name">The custom name to save.</param>
    public static void SetProjectName(string fileName, string name)
    {
        PlayerPrefs.SetString("ProjectName_" + fileName, name);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Clears the saved name for a specific project.
    /// </summary>
    /// <param name="fileName">The screenshot file name.</param>
    public static void ClearProjectName(string fileName)
    {
        PlayerPrefs.DeleteKey("ProjectName_" + fileName);
    }

    /// <summary>
    /// Clears all saved project names.
    /// </summary>
    public static void ClearAll()
    {
        // WARNING: This clears ALL PlayerPrefs, use with caution.
        PlayerPrefs.DeleteAll();
    }
}
