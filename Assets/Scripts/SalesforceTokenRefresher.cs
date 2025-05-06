using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SalesforceTokenRefresher : MonoBehaviour
{

    [Header("Salesforce OAuth Settings")]
    public string clientId = "3MVG9PwZx9R6_UrcLfI6KrlW15Iy.YAdCPxHQCi8z3KmKq5o4CyMhhXJVzRp6i9JifWQhf8BXdRGZo6DFJu5E";
    public string clientSecret = "F9C074748AC3AAE76621E014A0391C2CC24B252576B9E0C948D8368171E62617";
    public string refreshToken = "5Aep8610lkY9CyPXX2rUoqlHl_Ia0uzVjEpIAs5PpmbS5w8sc9MKvk4vbkGjGti3498Sij06kCdlhPD.qlG459Q\r\n";
    public string tokenEndpoint = "https://login.salesforce.com/services/oauth2/token";

    [Header("Access Token (Updated Automatically)")]
    public string currentAccessToken;

    public string lastInstanceUrl;

    public void Start()
    {
        StartCoroutine(RefreshAccessToken());
    }

    public IEnumerator RefreshAccessToken()
    {
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "refresh_token");
        form.AddField("client_id", clientId);
        form.AddField("client_secret", clientSecret);
        form.AddField("refresh_token", refreshToken);

        UnityWebRequest www = UnityWebRequest.Post(tokenEndpoint, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Token Refresh Success:\n" + www.downloadHandler.text);

            // Parse JSON to extract the access_token
            SalesforceTokenResponse token = JsonUtility.FromJson<SalesforceTokenResponse>(www.downloadHandler.text);
            currentAccessToken = token.access_token;

            lastInstanceUrl = token.instance_url;
        }
        else
        {
            Debug.LogError("❌ Token Refresh Failed: " + www.error);
            Debug.LogError(www.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class SalesforceTokenResponse
    {
        public string access_token;
        public string instance_url;
        public string id;
        public string issued_at;
        public string signature;
        public string token_type;
    }
}
