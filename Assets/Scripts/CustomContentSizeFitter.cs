using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomContentSizeFitter : MonoBehaviour
{
    public float buttonWidth = 250f; // Set desired button width
    public float buttonHeight = 100f; // Set desired button height
    private ScrollRect scrollRect;
    private HorizontalLayoutGroup horizontalLayoutGroup;
    private RectTransform rectTransform;

    void Start()
    {
        horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        scrollRect = GetComponentInParent<ScrollRect>();

        if (horizontalLayoutGroup == null || scrollRect == null)
        {
            Debug.LogError("Missing HorizontalLayoutGroup or ScrollRect on " + gameObject.name);
            return;
        }

        AdjustContentSize(); // ✅ Resizes content properly

        // Delayed function call to ensure UI layout is ready
        Invoke(nameof(CenterFirstButton), 0.1f);
    }

    void AdjustContentSize()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        // Resize all buttons dynamically
        foreach (Transform child in transform)
        {
            RectTransform btnRect = child.GetComponent<RectTransform>();
            if (btnRect != null)
            {
                btnRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);
            }
        }

        float totalWidth = horizontalLayoutGroup.spacing * (childCount - 1) + (childCount * buttonWidth);

        // Calculate extra padding so the first button starts in the center
        float viewportWidth = scrollRect.viewport.rect.width;
        float extraPadding = (viewportWidth - buttonWidth) / 2f;
        horizontalLayoutGroup.padding.left = Mathf.RoundToInt(extraPadding);
        horizontalLayoutGroup.padding.right = Mathf.RoundToInt(extraPadding);

        rectTransform.sizeDelta = new Vector2(totalWidth + horizontalLayoutGroup.padding.left + horizontalLayoutGroup.padding.right, rectTransform.sizeDelta.y);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
       // Canvas.ForceUpdateCanvases(); // ✅ Ensures layout updates before scrolling
    }

    void CenterFirstButton()
    {
        // ✅ Set Scroll Rect to position the first button in the center
        scrollRect.horizontalNormalizedPosition = 0f; // Moves to the start (first button)
    }

    public void UpdateSize()
    {
        AdjustContentSize();
    }
}
