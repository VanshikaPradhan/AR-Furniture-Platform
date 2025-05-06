using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.AR;


public class FurniturePlacementManager : MonoBehaviour
{
    public GameObject placementIndicator;
    public ARRaycastManager raycastManager;
    public Camera arCamera;

    private GameObject selectedPrefab;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private float lastTouchTime = 0f;
    private float touchCooldown = 0.5f;

    public List<GameObject> placedObjects = new List<GameObject>();

    void Start()
    {
        //// 🧪 TEST: Assign a test cube prefab manually
        //GameObject testPrefab = Resources.Load<GameObject>("FurnitureModels/Kitchen_table/Kitchen_table");
        //if (testPrefab != null)
        //{
        //    selectedPrefab = testPrefab;
        //    Debug.Log("🧪 Cube prefab assigned!");
        //}
        //else
        //{
        //    Debug.LogWarning("❌ Could not find TestCube in Resources/Prefabs.");
        //}
        if (FurnitureTransferData.SelectedModel != null)
        {
            SetSelectedModel(FurnitureTransferData.SelectedModel);

            // Optional: Clear it after use
            FurnitureTransferData.SelectedModel = null;
        }
    }
    public void SetSelectedModel(GameObject prefab)
    {
        selectedPrefab = prefab;
        Debug.Log("📦 New model selected: " + prefab.name);
    }

    void Update()
    {
        if (Touchscreen.current == null) return;

        // ✅ Handle placement
        if (selectedPrefab != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            if (Time.time - lastTouchTime < touchCooldown)
                return;

            lastTouchTime = Time.time;
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;



                GameObject newObject = Instantiate(selectedPrefab);
                if (!newObject.GetComponent<ARSelectionInteractable>())
                {
                    newObject.AddComponent<ARSelectionInteractable>();
                }

                newObject.transform.position = hitPose.position;
                newObject.transform.rotation = hitPose.rotation;
                newObject.transform.localScale = Vector3.one * 1f;

                foreach (var r in newObject.GetComponentsInChildren<MeshRenderer>(true))
                {
                    r.enabled = true;
                }

                placedObjects.Add(newObject);

                if (FurnitureHistoryManager.Instance != null)
                {
                    FurnitureHistoryManager.Instance.RegisterPlacedObject(newObject);
                }


                Debug.Log("✅ Spawned prefab: " + newObject.name + " at position: " + hitPose.position);
                selectedPrefab = null;
            }
        }

        // 🟨 Tap-away deselection
        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            if (!IsTouchOverUI(touchPosition))
            {
                Ray ray = arCamera.ScreenPointToRay(touchPosition);
                RaycastHit hit;

                if (!Physics.Raycast(ray, out hit))
                {
                    var selectionManager = FindObjectOfType<FurnitureSelectionManager>();
                    if (selectionManager != null)
                    {
                        selectionManager.ClearSelection();
                        Debug.Log("👆 Deselected model on empty tap.");
                    }
                }
            }
        }
    }

    // ✅ Declare IsTouchOverUI outside of Update()
    private bool IsTouchOverUI(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = position
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0;
    }
}
