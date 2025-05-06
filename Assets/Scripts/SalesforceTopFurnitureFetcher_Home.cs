using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SalesforceTopFurnitureFetcher_Home : MonoBehaviour
{
    public GameObject loadingPanel;
    public GameObject furnitureCardPrefab;
    public Transform gridParent;

    private SalesforceTokenRefresher tokenRefresher;

    void Start()
    {
        tokenRefresher = GameObject.Find("SalesforceAuth").GetComponent<SalesforceTokenRefresher>();
        StartCoroutine(WaitForTokenAndFetch());
    }

    IEnumerator WaitForTokenAndFetch()
    {
        loadingPanel.SetActive(true);

        while (string.IsNullOrEmpty(tokenRefresher.currentAccessToken))
        {
            Debug.Log("⏳ Waiting for token...");
            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(FetchTopFurnitureData(tokenRefresher.currentAccessToken));

        loadingPanel.SetActive(false);
    }

    IEnumerator FetchTopFurnitureData(string accessToken)
    {
        string instanceUrl = tokenRefresher.lastInstanceUrl;
        string soqlQuery = "SELECT Name, Name__c, Category__c, Price__c, Image_URL__c FROM Furniture__c LIMIT 4";
        string queryUrl = instanceUrl + "/services/data/v59.0/query/?q=" + UnityWebRequest.EscapeURL(soqlQuery);

        using (UnityWebRequest request = UnityWebRequest.Get(queryUrl))
        {
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Salesforce fetch error: " + request.error);
                yield break; // Stop further execution if fetch fails
            }

            FurnitureWrapper furnitureData = JsonUtility.FromJson<FurnitureWrapper>(request.downloadHandler.text);
            if (furnitureData == null || furnitureData.records == null || furnitureData.records.Length == 0)
            {
                Debug.LogWarning("⚠️ No furniture records found or deserialization failed.");
                yield break;
            }

            Debug.Log("✅ Top Furniture count: " + furnitureData.totalSize);

            foreach (Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in furnitureData.records)
            {
                if (string.IsNullOrEmpty(item.Name__c))
                {
                    Debug.LogWarning("⚠️ Skipping furniture item due to missing Name__c field.");
                    continue;
                }

                GameObject card = Instantiate(furnitureCardPrefab, gridParent);
                if (card == null)
                {
                    Debug.LogError("❌ Failed to instantiate furniture card prefab.");
                    continue;
                }

                FurnitureCardUI_Furniture ui = card.GetComponent<FurnitureCardUI_Furniture>();
                if (ui == null)
                {
                    Debug.LogError("❌ Missing FurnitureCardUI_Furniture script on prefab. Please fix the prefab!");
                    Destroy(card);
                    continue;
                }

                GameObject prefabModel = Resources.Load<GameObject>($"FurnitureModels/{item.Name__c}/{item.Name__c}");
                if (prefabModel == null)
                {
                    Debug.LogWarning($"⚠️ Model prefab not found for furniture: {item.Name__c}");
                }
                if (item == null)
                {
                    Debug.LogError("❌ Salesforce item is null!");
                }
                if (string.IsNullOrEmpty(item.Image_URL__c))
                {
                    Debug.LogWarning("⚠️ Image URL missing for item: " + (item != null ? item.Name__c : "Unknown"));
                }

                ui?.SetData(item.Image_URL__c, prefabModel);

                // Setup button click safely
                string furnitureNameCopy = item.Name__c; // capture copy for closure
                Button button = card.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() =>
                    {
                        
                        FurnitureSelectionManager_Home.Instance.SetSelectedFurniture(furnitureNameCopy);
                        // ⭐ FIX: Push current scene to history stack
                        SceneHistoryStack.PushCurrentScene();
                        UnityEngine.SceneManagement.SceneManager.LoadScene("ARScene");
                    });
                }
                else
                {
                    Debug.LogWarning($"⚠️ Button component missing on instantiated card for {item.Name__c}");
                }
            }
        }
    }

    // This will be called when a furniture card is clicked
    void OnFurnitureCardClicked(GameObject selectedModel, string selectedModelName)
    {
        Debug.Log("🖱 Clicked Furniture: " + selectedModelName);
        FurnitureTransferData.SelectedModel = selectedModel;
        FurnitureTransferData.SelectedModelName = selectedModelName;

        // Push current scene before moving to AR
        SceneHistoryStack.PushScene(SceneManager.GetActiveScene().name);

        // Now load AR scene
        SceneManager.LoadScene("ARScene");
    }
}
