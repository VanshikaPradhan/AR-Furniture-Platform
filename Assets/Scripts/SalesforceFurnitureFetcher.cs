using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class SalesforceFurnitureFetcher : MonoBehaviour
{
    public GameObject furnitureCardPrefab;
    public Transform contentPanel;
    public ScrollViewButtonZoomDOTween zoomScript;
    public GameObject loadingText;
    public Button refreshButton;
    public GameObject loadingSpinner;
    public GameObject noFurniturePanel;

    private SalesforceTokenRefresher tokenRefresher;

    
    void Start()
    {
        // Find the refresher on the SalesforceAuth GameObject
        tokenRefresher = GameObject.Find("SalesforceAuth").GetComponent<SalesforceTokenRefresher>();
        StartCoroutine(WaitForTokenAndFetch());
    }

    IEnumerator WaitForTokenAndFetch()
    {
        loadingSpinner.SetActive(true);
        loadingText.SetActive(true);//show laoding
        refreshButton.interactable = false; //Disable


        // Wait until access token is ready
        while (string.IsNullOrEmpty(tokenRefresher.currentAccessToken))
        {
            Debug.Log("⏳ Waiting for refreshed access token...");
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("✅ Access token is ready!");
        yield return StartCoroutine(FetchFurnitureData(tokenRefresher.currentAccessToken));

        loadingSpinner.SetActive(false);    
        loadingText.SetActive(false); //after loading is done
        refreshButton.interactable = true; //Enable
    }

    IEnumerator FetchFurnitureData(string accessToken)
    {
        string instanceUrl = tokenRefresher.lastInstanceUrl;
        string soqlQuery = "SELECT Name, Name__c, Category__c, Price__c, Image_URL__c FROM Furniture__c";
        string queryUrl = instanceUrl + "/services/data/v59.0/query/?q=" + UnityWebRequest.EscapeURL(soqlQuery);

        UnityWebRequest request = UnityWebRequest.Get(queryUrl);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        Debug.Log("🔎 Request URL: " + queryUrl);
        Debug.Log("🔐 Using Access Token: " + accessToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            // ✅ Parse JSON to C# objects
            FurnitureWrapper furnitureData = JsonUtility.FromJson<FurnitureWrapper>(json);

            Debug.Log("🪑 Total Furniture Items: " + furnitureData.totalSize);

            // Optional: Clear old cards first


            if (furnitureData.totalSize == 0 || furnitureData.records.Length == 0)
            {
                noFurniturePanel.SetActive(true);
            }
            else
            {
                noFurniturePanel.SetActive(false);
                zoomScript.buttons.Clear();
                foreach (Transform child in contentPanel)
                {
                    Destroy(child.gameObject);
                }


                zoomScript.buttons.Clear();

                // Add new cards
                foreach (var item in furnitureData.records)
                {
                    GameObject card = Instantiate(furnitureCardPrefab, contentPanel);
                    FurnitureCardUI ui = card.GetComponent<FurnitureCardUI>();
                    ui.SetData(item.Name__c, item.Image_URL__c);

                    // Load and assign the matching 3D prefab
                    GameObject prefab = Resources.Load<GameObject>("FurnitureModels/" + item.Name__c + "/" + item.Name__c);
                    ui.modelPrefab = prefab;
                    Debug.Log("Displaying furniture " + item.Name__c);

                    if (prefab != null)
                    {

                        Debug.Log("✅ Loaded prefab: " + prefab.name);
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ Prefab not found for: " + item.Name__c + ". Make sure it's in Assets/Resources/Prefabs");
                    }
                }

                // ✅ Refresh the zoom button list after all cards are added
                zoomScript.RefreshButtons();

                // 🎯 Center selected model if transferred
                if (!string.IsNullOrEmpty(FurnitureTransferData.SelectedModelName))
                {
                    StartCoroutine(CenterCardByName(FurnitureTransferData.SelectedModelName));
                }
                else
                {
                    StartCoroutine(DelayedCenter());
                }





                LayoutRebuilder.ForceRebuildLayoutImmediate(zoomScript.contentPanel);
            }
        }
        else
        {
            Debug.LogError("❌ Salesforce API Error: " + request.error);
            Debug.LogError("📦 Response: " + request.downloadHandler.text);
        }
    }

    IEnumerator DelayedCenter()
    {
        yield return new WaitForSeconds(0.2f); // give it more time!

        if (zoomScript.buttons.Count > 0)
        {
            zoomScript.CenterUsingAnchoredPosition(zoomScript.buttons[0]);
            
        }
    }

    public void RefreshFurnitureList()
    {
        StartCoroutine(WaitForTokenAndFetch());
       
    }


    IEnumerator CenterCardByName(string targetName)
    {
        yield return new WaitForEndOfFrame(); // wait a frame for layout to settle

        foreach (var button in zoomScript.buttons)
        {
            var ui = button.GetComponent<FurnitureCardUI>();
            if (ui != null && ui.modelPrefab != null && ui.modelPrefab.name == targetName)
            {
                zoomScript.CenterUsingAnchoredPosition(button.GetComponent<RectTransform>());
                break;
            }
        }

        FurnitureTransferData.SelectedModelName = null; // clear after use
    }

}
