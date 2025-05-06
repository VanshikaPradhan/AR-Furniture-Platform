using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;


public class FurnitureCardUI_Furniture : MonoBehaviour
{
    public RawImage furnitureImage;     // For furniture thumbnail
    public TMP_Text furnitureName;          // Standard UI Text (not TMP)
    public Button previewButton;        // Optional preview button
    public GameObject modelPrefab;      // Optional AR prefab reference
    public string ModelName { get; private set; }



    public void SetData(string imageUrl, GameObject prefab = null)
    {
        
        modelPrefab = prefab;

        if (modelPrefab != null)
        {
            ModelName = modelPrefab.name;
            furnitureName.text = ModelName;
        }
        else
        {
            furnitureName.text = "Unknown"; // or fallback text
            ModelName = "Unknown";
            Debug.LogWarning("⚠️ modelPrefab is null while setting data!");
        }

        StartCoroutine(LoadImage(imageUrl));

        if (previewButton != null)
        {
            previewButton.onClick.RemoveAllListeners();
            previewButton.onClick.AddListener(OnCardClicked);
            
        }
    }

    public void OnCardClicked()
    {
        if (modelPrefab != null)
        {
            FurnitureTransferData.SelectedModel = modelPrefab;
            FurnitureTransferData.SelectedModelName = ModelName;


            // Push current scene to SceneHistoryStack for Back button
            SceneHistoryStack.PushScene(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("ARScene"); // replace with your actual scene name
        }
    }



    IEnumerator LoadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            furnitureImage.texture = tex;
        }
        else
        {
            Debug.LogWarning("❌ Failed to load image from: " + url);
        }
    }
}
