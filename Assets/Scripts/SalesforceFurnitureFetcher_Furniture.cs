using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SalesforceFurnitureFetcher_Furniture : MonoBehaviour
{
    public GameObject loadingPanel;
    public GameObject furnitureCardPrefab;
    public Transform contentPanel;

    private SalesforceTokenRefresher tokenRefresher;

    void Start()
    {
        tokenRefresher = GameObject.Find("SalesforceAuth").GetComponent<SalesforceTokenRefresher>();
        StartCoroutine(WaitForTokenAndFetch());
    }

    IEnumerator WaitForTokenAndFetch()
    {
        loadingPanel.SetActive(true); //SHow Spinner
        while (string.IsNullOrEmpty(tokenRefresher.currentAccessToken))
        {
            Debug.Log("⏳ Waiting for token...");
            yield return new WaitForSeconds(0.2f);
        }

        yield return StartCoroutine(FetchFurnitureData(tokenRefresher.currentAccessToken));

        loadingPanel.SetActive(false); //Hide Spinner

    }

    IEnumerator FetchFurnitureData(string accessToken)
    {
        string instanceUrl = tokenRefresher.lastInstanceUrl;
        string soqlQuery = "SELECT Name, Name__c, Category__c, Price__c, Image_URL__c FROM Furniture__c LIMIT 20";
        string queryUrl = instanceUrl + "/services/data/v59.0/query/?q=" + UnityWebRequest.EscapeURL(soqlQuery);

        UnityWebRequest request = UnityWebRequest.Get(queryUrl);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            FurnitureWrapper furnitureData = JsonUtility.FromJson<FurnitureWrapper>(request.downloadHandler.text);
            Debug.Log("✅ Furniture count: " + furnitureData.totalSize);

            foreach (Transform child in contentPanel)
                Destroy(child.gameObject);

            int count = 0;
            foreach (var item in furnitureData.records)
            {
                GameObject card = Instantiate(furnitureCardPrefab, contentPanel);
                FurnitureCardUI_Furniture ui = card.GetComponent<FurnitureCardUI_Furniture>();
                GameObject prefab = Resources.Load<GameObject>("FurnitureModels/" + item.Name__c + "/" + item.Name__c);
                ui.SetData(item.Image_URL__c, prefab);

                count++;
                if (count % 5 == 0)
                    yield return null;
            }
            // 🟰 ADD THESE TWO LINES AFTER spawning all cards
            yield return new WaitForEndOfFrame(); // Wait 1 frame
                                                  // 🟰 Add a tiny delay to make sure other managers are ready
            yield return new WaitForSeconds(0.1f);
            CenterSelectedFurniture();            // Center the selected furniture
        }
        else
        {
            Debug.LogError("❌ Salesforce fetch error: " + request.error);
        }
    }

    void CenterSelectedFurniture()
    {
        if (FurnitureSelectionManager_Home.Instance == null)
        {
            Debug.LogWarning("FurnitureSelectionManager_Home.Instance is null. Cannot center furniture.");
            return;
        }
        string selectedName = FurnitureSelectionManager_Home.Instance.GetSelectedFurniture(); // Get the selected furniture name

        if (string.IsNullOrEmpty(selectedName))
        {
            Debug.LogWarning("No furniture selected to center.");
            return;
        }
        int index = -1;
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            FurnitureCardUI_Furniture cardUI = contentPanel.GetChild(i).GetComponent<FurnitureCardUI_Furniture>();
            if (cardUI.modelPrefab != null && cardUI.modelPrefab.name == selectedName)
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            // Assuming Horizontal Scroll View
            UnityEngine.UI.ScrollRect scrollRect = contentPanel.GetComponentInParent<UnityEngine.UI.ScrollRect>();
            float normalizedPosition = (float)index / (contentPanel.childCount - 1);
            scrollRect.horizontalNormalizedPosition = normalizedPosition;

            // If your scroll view is vertical, then use:
            // scrollRect.verticalNormalizedPosition = 1f - normalizedPosition;
        }
        else
        {
            Debug.LogWarning("Selected furniture not found in AR scroll cards.");
        }
    }

}
