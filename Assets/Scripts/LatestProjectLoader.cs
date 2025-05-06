using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LatestProjectLoader : MonoBehaviour
{
    public GameObject projectCardPrefab;
    public Transform cardContainer;

    void Start()
    {
        ShowLatestProject();
    }

    void ShowLatestProject()
    {
        string folderPath = ScreenshotPathHelper.GetScreenshotFolder();

        if (!Directory.Exists(folderPath))
        {
            Debug.Log("Screenshot folder not found.");
            return;
        }

        // Find latest PNG file
        string[] files = Directory.GetFiles(folderPath, "*.png");
        if (files.Length == 0)
        {
            Debug.LogWarning("No screenshots found.");
            return;
        }

        // Sort files by last write time
        string latestFile = null;
        System.DateTime latestTime = System.DateTime.MinValue;

        foreach (string file in files)
        {
            var lastWrite = File.GetLastWriteTime(file);
            if (lastWrite > latestTime)
            {
                latestTime = lastWrite;
                latestFile = file;
            }
        }

        if (string.IsNullOrEmpty(latestFile)) return;

        // Load image
        byte[] bytes = File.ReadAllBytes(latestFile);
        Texture2D tex = new Texture2D(2, 2);
        if (!tex.LoadImage(bytes))
        {
            Debug.LogWarning("Failed to load image.");
            return;
        }

        GameObject card = Instantiate(projectCardPrefab, cardContainer);

        // Set Image
        Transform imgObj = card.transform.Find("ProjectImage");
        if (imgObj != null)
        {
            Image img = imgObj.GetComponent<Image>();
            img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        // Set Title
        Transform titleObj = card.transform.Find("ProjectTitle");
        TextMeshProUGUI titleText = titleObj?.GetComponent<TextMeshProUGUI>();

        string fileName = Path.GetFileName(latestFile);
        string cardId = Path.GetFileNameWithoutExtension(fileName);
        string savedName = PlayerPrefs.GetString("ProjectName_" + cardId, "Latest Click");

        if (titleText != null)
        {
            titleText.text = savedName;
        }
    }
}
