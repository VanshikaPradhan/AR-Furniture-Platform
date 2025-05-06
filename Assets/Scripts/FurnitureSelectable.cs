using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class FurnitureSelectable : MonoBehaviour
{
    private ARSelectionInteractable selection;

    void Awake()
    {
        selection = GetComponent<ARSelectionInteractable>();
        if (selection != null)
        {
            selection.selectEntered.AddListener(OnSelected);
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        Debug.Log("🟨 Furniture selected: " + gameObject.name);
        FurnitureSelectionManager manager = FindObjectOfType<FurnitureSelectionManager>();
        if (manager != null)
        {
            manager.SetSelectedObject(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (selection != null)
        {
            selection.selectEntered.RemoveListener(OnSelected);
        }
    }
}
