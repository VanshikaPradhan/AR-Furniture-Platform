using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using Unity.XR.CoreUtils; // Needed for XROrigin

public class ARInteractableInitializer : MonoBehaviour
{
    void Awake()
    {
        var xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogWarning("❗ XR Origin not found in scene.");
            return;
        }

        // Assign to ARTranslationInteractable
        var translation = GetComponent<ARTranslationInteractable>();
        if (translation != null)
        {
            translation.xrOrigin = xrOrigin;
        }

        // Assign to ARRotationInteractable
        var rotation = GetComponent<ARRotationInteractable>();
        if (rotation != null)
        {
            rotation.xrOrigin = xrOrigin;
        }

        // Assign to ARScaleInteractable
        var scale = GetComponent<ARScaleInteractable>();
        if (scale != null)
        {
            scale.xrOrigin = xrOrigin;
        }

        Debug.Log("✅ Interactables initialized on: " + gameObject.name);
    }
}
