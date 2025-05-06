using System.IO;

public static class ScreenshotPathHelper
{
    public static string GetScreenshotFolder()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return Path.Combine("/storage/emulated/0/DCIM", "AR_Captures");
#elif UNITY_EDITOR || UNITY_STANDALONE
        return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "AR_Captures");
#else
        return Path.Combine(UnityEngine.Application.persistentDataPath, "AR_Captures");
#endif
    }
}
