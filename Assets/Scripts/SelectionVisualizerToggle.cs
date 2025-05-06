using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class SelectionVisualizerToggle : MonoBehaviour
{
    public GameObject selectionCube;

    private ARSelectionInteractable selection;

    void Awake()
    {
        selection = GetComponent<ARSelectionInteractable>();
        selection.selectEntered.AddListener((args) => selectionCube.SetActive(true));
        selection.selectExited.AddListener((args) => selectionCube.SetActive(false));
        selectionCube.SetActive(false); // Start hidden
    }
}
