using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

public class FurnitureSelectionManager : MonoBehaviour
{
    public static FurnitureSelectionManager Instance { get; private set; }

    public GameObject deleteButtonUI; // Assign in inspector
    public ARRaycastManager raycastManager;
    private GameObject selectedObject;
    private ARSelectionInteractable currentSelectable;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        if (raycastManager == null)
            raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        // Tap detection to clear selection
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            if (IsTouchOverUI(touch)) return;
            // 🕵️‍♀️ Check if touch is on a detected plane
            if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                // 🤚 Tapped on plane, not furniture — clear selection
                ClearSelection();
            }
        }
    }

    //bool IsTouchOnEmptySpace(Touch touch)
    //{
    //    return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    //}
    public void SetSelectedObject(GameObject obj)
    {
        //// If the same object is already selected, ignore
        //if (selectedObject == obj)
        //    return;
        // Deselect previous if any
        if (currentSelectable != null && currentSelectable.isSelected)
        {
            ForceDeselect(currentSelectable);
        }
        selectedObject = obj;
        currentSelectable = obj.GetComponent<ARSelectionInteractable>();

        if (deleteButtonUI != null)
        {
            deleteButtonUI.SetActive(true);
        }// Show delete button
        // var rb = obj.GetComponent<Rigidbody>();

       

        Debug.Log("✅ Selected: " + obj.name);
    }
    public void DeleteSelectedObject()
    {
        if (selectedObject != null)
        {
            Debug.Log("🗑️ Deleting: " + selectedObject.name);
            if (currentSelectable != null && currentSelectable.isSelected)
            {
                ForceDeselect(currentSelectable);
            }
            // Deselect explicitly before deletin
            Destroy(selectedObject);
            selectedObject = null;
            currentSelectable = null;

            if (deleteButtonUI != null)
                deleteButtonUI.SetActive(false);

            Debug.Log("❌ Object deleted.");
        }
    }

    public void ClearSelection()
    {
        if (selectedObject == null)
            return;

        //var rb = selectedObject.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.isKinematic = true;
        //}
        if (currentSelectable != null &&
            currentSelectable.isSelected)
        {
            ForceDeselect(currentSelectable);
        }
        selectedObject = null;

        currentSelectable = null;


        if (deleteButtonUI != null)
        {
            deleteButtonUI.SetActive(false);
        }

        Debug.Log("❌ Selection cleared.");
    }

    public void MoveSelected(Vector3 direction)
    {
        if (selectedObject != null)
        {
            Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 newPosition = rb.position + direction;
                rb.MovePosition(newPosition);
                Debug.Log("Moved object using Rigidbody to: " + newPosition);
            }
            else
            {
                // Fallback if no rigidbody
                selectedObject.transform.position += direction;
            }
        }
    }




    private void ForceDeselect(ARSelectionInteractable selectable)
    {
        if (selectable.firstInteractorSelecting != null &&
           selectable.interactionManager != null)
        {
            selectable.interactionManager.SelectExit(selectable.firstInteractorSelecting, selectable);
            Debug.Log("🔄 Forced deselect: " + selectable.name);
        }
    }
    private bool IsTouchOverUI(Touch touch)
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }

}
