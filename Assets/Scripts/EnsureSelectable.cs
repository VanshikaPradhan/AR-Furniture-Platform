using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class EnsureSelectable : MonoBehaviour
{
    void Start()
    {
        if (GetComponent<ARSelectionInteractable>() == null)
            gameObject.AddComponent<ARSelectionInteractable>();
    }
}
