using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AutoBoxColliderFitter : MonoBehaviour
{
    void Start()
    {
        FitColliderToModel();
    }

    void FitColliderToModel()
    {
        // Combine all Renderer bounds in children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found to calculate bounds.");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        // Convert world bounds to local space
        Vector3 localCenter = transform.InverseTransformPoint(bounds.center);
        Vector3 localSize = transform.InverseTransformVector(bounds.size);

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.center = localCenter;
        boxCollider.size = localSize;

        Debug.Log("✅ Box Collider adjusted to fit: " + gameObject.name);
    }
}
