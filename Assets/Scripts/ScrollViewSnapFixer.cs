using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScrollViewSnapFixer : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public float delayToCenter = 0.1f;

    void Start()
    {
        StartCoroutine(CenterFirstButton());
    }

    IEnumerator CenterFirstButton()
    {
        yield return new WaitForSeconds(delayToCenter); // Wait for layout and prefab build

        if (contentPanel.childCount == 0) yield break;

        RectTransform firstItem = contentPanel.GetChild(0).GetComponent<RectTransform>();
        if (firstItem == null) yield break;

        // Force layout rebuild
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel);

        float viewportWidth = scrollRect.viewport.rect.width;
        float contentWidth = contentPanel.rect.width;
        float itemPosX = firstItem.localPosition.x + (firstItem.rect.width / 2f);
        float centerOffset = itemPosX - (viewportWidth / 2f);

        float normalizedPos = Mathf.Clamp01(centerOffset / (contentWidth - viewportWidth));
        scrollRect.horizontalNormalizedPosition = normalizedPos;

        Debug.Log("✅ First card centered with normalized position: " + normalizedPos);
    }
}
