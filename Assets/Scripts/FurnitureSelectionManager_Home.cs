using UnityEngine;

public class FurnitureSelectionManager_Home : MonoBehaviour
{
    public static FurnitureSelectionManager_Home Instance;

    public string selectedFurnitureName = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedFurniture(string furnitureName)
    {
        selectedFurnitureName = furnitureName;
    }

    public string GetSelectedFurniture()
    {
        return selectedFurnitureName;
    }

}
