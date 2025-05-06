using UnityEngine;
using System.Collections.Generic;

public class FurnitureHistoryManager : MonoBehaviour
{
    public static FurnitureHistoryManager Instance;

    private Stack<GameObject> undoStack = new Stack<GameObject>();
    private Stack<GameObject> redoStack = new Stack<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterPlacedObject(GameObject obj)
    {
        undoStack.Push(obj);
        redoStack.Clear(); // new action clears redo history
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            GameObject obj = undoStack.Pop();
            if (obj != null)
            {
                obj.SetActive(false);
                redoStack.Push(obj);
            }
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            GameObject obj = redoStack.Pop();
            if (obj != null)
            {
                obj.SetActive(true);
                undoStack.Push(obj);
            }
        }
    }

    public void ClearAll()
    {
        undoStack.Clear();
        redoStack.Clear();
    }

    public void OnUndoButtonClick()
    {
        Undo();
    }

    public void OnRedoButtonClick()
    {
        Redo();
    }

}
