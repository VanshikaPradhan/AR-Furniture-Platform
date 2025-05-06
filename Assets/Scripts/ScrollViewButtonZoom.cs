using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening; // Import DOTween
using System.Linq;      

public class ScrollViewButtonZoomDOTween : MonoBehaviour
{
    public ScrollRect scrollRect; // Scroll View reference
    public RectTransform contentPanel; // Content inside the Scroll View
    public RectTransform selectionPointer; // Invisible selection pointer in center
    public float maxScale = 1.5f; // Maximum zoom scale
    public float minScale = 1f; // Minimum scale
    public float zoomDuration = 0.3f; // DOTween animation duration

    public List<RectTransform> buttons = new List<RectTransform>();
    private RectTransform lastClosestButton;

    void Start()
    {
        RefreshButtons();
        
    }

    void Update()
    {

        buttons.RemoveAll(btn => btn == null);

        if (buttons == null || buttons.Count == 0 || selectionPointer == null) return;

        RectTransform closestButton = null;
        float closestDistance = float.MaxValue;

        // Find the button closest to the Selection Pointer
        foreach (var btn in buttons)
        {
            if (btn == null) continue; //skip if destroyed
            float distance = Mathf.Abs(btn.position.x - selectionPointer.position.x);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestButton = btn;
            }
        }

        // Only animate when the closest button changes
        if (closestButton != lastClosestButton)
        {
            foreach (var btn in buttons)
            {
                if (btn == null) continue; //Prevents animating destroyed cards

                if (!btn.gameObject.activeInHierarchy) continue;

                try
                {

                    if (btn == closestButton)
                    {
                        btn.DOScale(Vector3.one * maxScale, zoomDuration).SetEase(Ease.OutQuad);
                    }
                    else
                    {
                        btn.DOScale(Vector3.one * minScale, zoomDuration).SetEase(Ease.OutQuad);
                    }
                }
                catch (System.Exception ex)
                { 
                    Debug.LogWarning("DOTween animation failed: " + ex.Message);
                }
            }
            lastClosestButton = closestButton;
        }
    }

    public void RefreshButtons()
    {
        buttons = contentPanel.GetComponentsInChildren<RectTransform>()
            .Where(child => child != contentPanel && child.GetComponent<Button>() != null)
            .ToList();

        Debug.Log("✅ Buttons refreshed: " + buttons.Count);
    }

    public void ScrollToCenterofFirstButton()
    {
        if(buttons.Count == 0 || scrollRect == null || contentPanel == null) return;

        RectTransform firstButton = buttons[0];

        float viewportWidth = scrollRect.viewport.rect.width;
        float contentWidth = contentPanel.rect.width;
        float buttonCenterX = firstButton.localPosition.x + (firstButton.rect.width / 2f);

        float scrollOffset = (buttonCenterX - (viewportWidth / 2f)) / (contentWidth - viewportWidth);
        scrollOffset = Mathf.Clamp01(scrollOffset);

        scrollRect.horizontalNormalizedPosition = scrollOffset;
    }

    //public void CenterOnButton(RectTransform target)
    //{
    //    if (target == null || scrollRect == null || contentPanel == null) return;

    //    Canvas.ForceUpdateCanvases(); // Ensure layout is updated

    //    float viewportWidth = scrollRect.viewport.rect.width;
    //    float contentWidth = contentPanel.rect.width;

    //    float targetCenterX = -target.localPosition.x - (target.rect.width / 2);
    //    float centerOffset = (viewportWidth / 2f);

    //    float targetPos = (targetCenterX + centerOffset) / (contentWidth - viewportWidth);
    //    scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(targetPos);
    //}

    public void CenterButton(RectTransform target)
    {
        if (target == null || scrollRect == null) return;

        Canvas.ForceUpdateCanvases(); // Ensure layout is ready

        // Convert target's local position into ScrollRect's coordinate space
        Vector2 contentPos = contentPanel.anchoredPosition;
        float targetPosX = target.localPosition.x;

        float viewportHalf = scrollRect.viewport.rect.width / 2f;
        float targetHalf = target.rect.width / 2f;

        float newX = -targetPosX + viewportHalf - targetHalf;

        // Clamp based on content bounds
        float minX = 0f;
        float maxX = contentPanel.rect.width - scrollRect.viewport.rect.width;
        float clampedX = Mathf.Clamp(-newX, -maxX, -minX);

        contentPanel.anchoredPosition = new Vector2(clampedX, contentPanel.anchoredPosition.y);

        Debug.Log("✅ Centered first button: " + target.name);
    }

    public void CenterOnFirstButton()
    {
        if (buttons == null || buttons.Count == 0) return;

        RectTransform target = buttons[0];

        // Force layout rebuild first
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel);

        // Calculate offset to center the first button
        float contentWidth = contentPanel.rect.width;
        float viewportWidth = scrollRect.viewport.rect.width;

        float targetPos = -target.localPosition.x - (target.rect.width / 2f) + (viewportWidth / 2f);
        float clampedX = Mathf.Clamp(targetPos, 0, contentWidth - viewportWidth);

        // Apply scroll position
        contentPanel.anchoredPosition = new Vector2(clampedX, contentPanel.anchoredPosition.y);

        Debug.Log("✅ First button centered at runtime.");
    }

    public void CenterUsingAnchoredPosition(RectTransform target)
    {
        if (target == null || scrollRect == null || contentPanel == null) return;

        Canvas.ForceUpdateCanvases(); // ensure layout is built

        float viewportWidth = scrollRect.viewport.rect.width;
        float targetCenterX = target.localPosition.x + (target.rect.width / 2f);
        float scrollOffset = targetCenterX - (viewportWidth / 2f);

        // Apply offset directly to anchored position
        Vector2 newAnchoredPos = new Vector2(scrollOffset, contentPanel.anchoredPosition.y);
        contentPanel.anchoredPosition = -newAnchoredPos;

        Debug.Log("✅ Anchored position scroll applied: " + contentPanel.anchoredPosition.x);
    }

}
