using UnityEngine;

public class FurnitureMovementUI : MonoBehaviour
{
    public float moveAmount = 0.1f; // Adjust for sensitivity

    public void MoveLeft() => FurnitureSelectionManager.Instance.MoveSelected(Vector3.left * moveAmount);
    public void MoveRight() => FurnitureSelectionManager.Instance.MoveSelected(Vector3.right * moveAmount);
    public void MoveForward() => FurnitureSelectionManager.Instance.MoveSelected(Vector3.forward * moveAmount);
    public void MoveBackward() => FurnitureSelectionManager.Instance.MoveSelected(Vector3.back * moveAmount);

    // ✅ New methods for vertical movement
    public void MoveUp() => FurnitureSelectionManager.Instance.MoveSelected(Vector3.up * moveAmount);
    public void MoveDown() => FurnitureSelectionManager.Instance.MoveSelected(Vector3.down * moveAmount);
}
