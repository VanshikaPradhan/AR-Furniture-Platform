using UnityEngine;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
public class ARPhotoCapture : MonoBehaviour
{
    public Camera arCamera; // Assign your AR Camera here (from XR Origin)
    public GameObject feedbackPanel; // UI panel to show "Captured!" meassage
    public RawImage screenshotThumbnail; // assign this in the inspector
    public GameObject flashPanel; // Assign in Inspector
    public AudioSource shutterAudio;


#if UNITY_ANDROID
    void Start()
    {
    if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
    {
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);
    }
    }
#endif
    public void TakeScreenshot()
    {
        StartCoroutine(CaptureAROnly());
    }

    IEnumerator CaptureAROnly()
    {
        if (flashPanel !=  null)
        {
            flashPanel.SetActive(true);
            yield return new WaitForSeconds(0.05f); // flash visible for just 0.05 seconds
            flashPanel.SetActive(false);
        }
        yield return new WaitForEndOfFrame();

        // Temporarily hide UI layer
        int originalCullingMask = arCamera.cullingMask;
        arCamera.cullingMask = originalCullingMask & ~(1 << LayerMask.NameToLayer("UIOnly"));

        // Render to Texture
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        RenderTexture previousRT = RenderTexture.active;

        arCamera.targetTexture = rt;
        RenderTexture.active = rt;
        arCamera.Render();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        //Restore Render State
        arCamera.targetTexture = null;
        RenderTexture.active = previousRT;
        rt.Release();
        Destroy(rt);

       


        // Restore the UI
        arCamera.cullingMask = originalCullingMask;

        // Play shutter sound
        if (shutterAudio != null)
        {
            shutterAudio.Play();
        }


        // Save image
        string filename = "AR_Photo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string path = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        string dcimPath = Path.Combine("/storage/emulated/0/DCIM", "AR_Captures");
        if (!Directory.Exists(dcimPath))
            Directory.CreateDirectory(dcimPath);
        path = Path.Combine(dcimPath, filename); // mobile safe path
        File.WriteAllBytes(path, screenImage.EncodeToPNG());

        // 📣 Trigger media scan to appear in gallery
        using (AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject mediaScanIntent = new AndroidJavaObject("android.content.Intent", "android.intent.action.MEDIA_SCANNER_SCAN_FILE"))
        using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
        {
            AndroidJavaObject fileObj = new AndroidJavaObject("java.io.File", path);
            AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObj);
            mediaScanIntent.Call<AndroidJavaObject>("setData", uri);
            activity.Call("sendBroadcast", mediaScanIntent);
        }
#else
        //For editor or desktop
        string picturesPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
        string captureFolder = Path.Combine(picturesPath, "AR_Captures");

        if (!Directory.Exists(captureFolder))
        {
            Directory.CreateDirectory(captureFolder);
            Debug.Log("📂 Created folder: " + captureFolder);
        }
       
        path = Path.Combine(captureFolder, filename);
        File.WriteAllBytes(path, screenImage.EncodeToPNG());
#endif

        if (ProjectCardManager.Instance != null)
        {
            ProjectCardManager.Instance.CreateProjectCard(path);
        }
        else
        {
            Debug.LogWarning("⚠️ ProjectCardManager.Instance is null. Cannot create project card.");
        }


        Debug.Log("✅ Screenshot saved to " + path);


        //Show feedback 
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            feedbackPanel.SetActive(false);
        }

       

        //show thumbnail preview
        if (screenshotThumbnail != null)
        {
            screenshotThumbnail.texture = screenImage;
            screenshotThumbnail.gameObject.SetActive(true);
            StartCoroutine(HideThumbnailAfterDelay(3f));
        }

       
    }

    IEnumerator HideThumbnailAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (screenshotThumbnail != null)
        {
            screenshotThumbnail.gameObject.SetActive(false);
        }
    }
    public void HideThumbnailManually()
    {
        if (screenshotThumbnail != null)
        {
            screenshotThumbnail.gameObject.SetActive(false);
        }
    }

    IEnumerator FadeOutFlash(CanvasGroup canvasGroup)
    {
        yield return new WaitForSeconds(0.01f); // almost instant
        float duration = 0.05f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / duration);
            yield return null;
        }

        flashPanel.SetActive(false);
        canvasGroup.alpha = 1;
    }

}
