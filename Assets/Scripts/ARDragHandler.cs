using UnityEngine;

public class ARDragHandler : MonoBehaviour
{
    private Camera arCamera;
    private bool isDragging;

    void Start()
    {
        arCamera = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Ray ray = arCamera.ScreenPointToRay(touch.position);

        if (touch.phase == TouchPhase.Began)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                    isDragging = true;
            }
        }

        if (touch.phase == TouchPhase.Moved && isDragging)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Move the object to the touch point on the AR plane
                Vector3 newPos = hit.point;
                transform.position = newPos;
            }
        }

        if (touch.phase == TouchPhase.Ended)
        {
            isDragging = false;
        }
    }
}
