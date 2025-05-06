using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;

public class PreviewManager : MonoBehaviour
{
    public GameObject projectCardPrefab;
    public Transform cardContainer;
    public GameObject tagPrefab; // Optional
    public GameObject deleteConfirmationPanel;
    public Button yesButton;
    public Button cancelButton;


    void Start()
    {
        LoadProjectCards();
    }

    void LoadProjectCards()
    {
        string folderPath = ScreenshotPathHelper.GetScreenshotFolder();

        if (!Directory.Exists(folderPath))
        {
            Debug.Log("Screenshot folder not found.");
            Directory.CreateDirectory(folderPath);
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.png");
        if (files.Length == 0)
        {
            Debug.LogWarning("⚠️ No screenshots found in " + folderPath);
            return;
        }

        for (int i = 0; i < files.Length; i++)
        {
            string path = files[i];
            if (!File.Exists(path))
            {
                Debug.LogWarning("File does not exist: " + path);
                continue;
            }
            byte[] bytes = File.ReadAllBytes(path);
            if (bytes == null || bytes.Length == 0)
            {
                Debug.LogWarning("Empty file: " + path);
                continue;
            }
            Texture2D tex = new Texture2D(2, 2);
            if (!tex.LoadImage(bytes))
            {
                Debug.LogWarning("Failed to load image: " + path);
                continue;
            }

            if (projectCardPrefab == null || cardContainer == null)
            {
                Debug.LogError("ProjectCardPrefab or CardContainer is missing in the Inspector!");
                return;
            }

            GameObject card = Instantiate(projectCardPrefab, cardContainer);

            string fileName = Path.GetFileName(path); // e.g. "project1.png"
            // Get a unique ID for the card (we'll use the screenshot file name)
            string cardId = Path.GetFileNameWithoutExtension(path);

            // Set Screenshot Image
            Transform imgObj = card.transform.Find("ProjectImage");
            if (imgObj != null)
            {
                Image img = imgObj.GetComponent<Image>();
                Rect rect = new Rect(0, 0, tex.width, tex.height);
                img.sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
            }
            // Set Title
            Transform titleObj = card.transform.Find("ProjectTitle");
            TextMeshProUGUI titleText = titleObj?.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                string savedName = ProjectNameStore.GetProjectName(fileName);
                titleText.text = string.IsNullOrEmpty(savedName) ? $"Project {i + 1}" : savedName;
            }
            // Setup Rename Button
            Transform renameBtnObj = card.transform.Find("RenameButton");
            Transform renameInputObj = card.transform.Find("RenameInputField");
            Transform confirmBtnObj = card.transform.Find("RenameConfirmButton");

            Button renameBtn = renameBtnObj.GetComponent<Button>();
            TMP_InputField inputField = renameInputObj.GetComponent<TMP_InputField>();
            Button confirmBtn = confirmBtnObj?.GetComponent<Button>();

            if (renameBtn != null && inputField != null && titleText != null && confirmBtn != null)
            {

                inputField.gameObject.SetActive(false); // Hide initially
                renameBtn.onClick.AddListener(() =>
                {
                    inputField.gameObject.SetActive(true);
                    confirmBtn.gameObject.SetActive(true);
                    inputField.text = titleText.text;

                    // Auto-focus the input field
                    EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                    inputField.ActivateInputField();
                });

                // Add listener to confirm button
                confirmBtn.onClick.RemoveAllListeners(); // Clear any existing listeners
                confirmBtn.onClick.AddListener(() =>
                {
                    string newName = inputField.text.Trim();
                    if (!string.IsNullOrEmpty(newName))
                    {
                        titleText.text = newName;
                        ProjectNameStore.SetProjectName(fileName, newName); // Save the new name
                    }
                    inputField.gameObject.SetActive(false);
                    confirmBtn.gameObject.SetActive(false); // Hide confirm button after rename
                });
                inputField.onEndEdit.RemoveAllListeners();
                inputField.onEndEdit.AddListener((newName) =>
                {
                        if (!string.IsNullOrEmpty(newName))
                        {
                            titleText.text = newName;
                            ProjectNameStore.SetProjectName(fileName, newName);
                            
                        }
                        inputField.gameObject.SetActive(false);
                         if (confirmBtn != null) confirmBtn.gameObject.SetActive(false); // Hide confirm button

                });
                
            }
            string imagePath = path;
            GameObject cardRef = card; // Capture the current card for later


            //Delete button
            Transform deleteBtn = card.transform.Find("DeleteButton");
            if (deleteBtn != null)
            {
                Button btn = deleteBtn.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() =>
                    {
                        // Show confirmation popup
                        deleteConfirmationPanel.SetActive(true);

                        // Clear previous listeners (to avoid duplicates)
                        yesButton.onClick.RemoveAllListeners();
                        cancelButton.onClick.RemoveAllListeners();

                        yesButton.onClick.AddListener(() =>
                        {
                            if (File.Exists(imagePath))
                            {
                                File.Delete(imagePath); // 🔥 Delete the image file permanently
                                Debug.Log($"Deleted: {imagePath}");
                            }
                            else
                            {
                                Debug.LogWarning("File already deleted or missing: " + imagePath);
                            }
                            Destroy(card); // ❌ Removes this card from scene
                            ProjectNameStore.ClearProjectName(fileName);
                            deleteConfirmationPanel.SetActive(false);
                        });
                        cancelButton.onClick.AddListener(() =>
                        {
                            deleteConfirmationPanel.SetActive(false);
                        });
                    });
                }
            }

            // Add Tags
            if (tagPrefab != null)
            {
                Transform tagParent = card.transform.Find("Tags");
                if (tagParent != null)
                {
                    string[] tags = { "furniture", "wall decor", "paint", "amazon" };

                    foreach (string tag in tags)
                    {
                        GameObject tagObj = Instantiate(tagPrefab, tagParent);
                        if (tagObj != null)
                        {
                            TextMeshProUGUI tagText = tagObj.GetComponentInChildren<TextMeshProUGUI>();
                            if (tagText != null)
                            {
                                tagText.text = tag;
                            }
                            else
                            {
                                Debug.LogWarning($"⚠️ No Text component found inside tag prefab '{tagPrefab.name}'");
                            }
                        }
                    }
                }
            }
           


        }
    }
}
