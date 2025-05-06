using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TapToDeselect : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (Touchscreen.current == null || Touchscreen.current.primaryTouch.press.wasReleasedThisFrame == false)
            return;

        Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
        if (raycastManager.Raycast(touchPos, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            // Tapped on empty AR space (not on furniture)
            FurnitureSelectionManager manager = FindObjectOfType<FurnitureSelectionManager>();
            if (manager != null)
                manager.ClearSelection();
        }
    }
}
