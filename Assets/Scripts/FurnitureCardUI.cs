using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class FurnitureCardUI : MonoBehaviour
{
    public RawImage furnitureImage;
    public TMP_Text nameText;
    public GameObject modelPrefab;


  
    public void SetData(string name, string imageUrl)
    {
        nameText.text = name;
        StartCoroutine(LoadImage(imageUrl));
    }

    void OnDestroy()
    {
        DOTween.Kill(gameObject); // 🧹 Kill any active tweens on this GameObject
    }

    public void OnCardClicked()
    {

        Debug.Log("🟨 OnCardClicked triggered. Prefab = " + (modelPrefab? modelPrefab.name : "null"));

        if (modelPrefab != null)
        {
            var placer = FindObjectOfType<FurniturePlacementManager>();
            if (placer != null)
            {
                placer.SetSelectedModel(modelPrefab);
                Debug.Log("✅ Furniture prefab assigned to FurniturePlacementManager: " + modelPrefab.name);
            }
            else
            {
                Debug.LogWarning("⚠️ FurniturePlacementManager not found in scene.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No model prefab assigned to this card.");
        }
    }
    IEnumerator LoadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            furnitureImage.texture = texture;
        }
        else
        {
            Debug.LogWarning("Failed to load image from URL: " + url);
        }
    }
}
