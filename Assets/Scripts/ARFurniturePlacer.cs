using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ARFurniturePlacer : MonoBehaviour
{
    private ARPlacementInteractable placementInteractable;

    void Awake()
    {
        placementInteractable = GetComponent<ARPlacementInteractable>();
    }

    public void AssignPlacementPrefab(GameObject prefab)
    {
        if (prefab != null)
        {
            placementInteractable.placementPrefab = prefab;
            Debug.Log("✅ Placement prefab updated: " + prefab.name);
        }
    }
}
