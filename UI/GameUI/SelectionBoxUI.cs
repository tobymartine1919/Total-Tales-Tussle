using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles rendering the drag-selection rectangle on a Screen Space Overlay canvas.
/// Attach this to the SelectionBox Image GameObject.
/// PlayerController holds a reference to this and passes it to SelectionIdleState.
/// </summary>
public class SelectionBoxUI : MonoBehaviour
{
    [SerializeField] private RectTransform boxRect;

    private void Awake()
    {
        // Auto-grab own RectTransform if not assigned
        if (boxRect == null)
            boxRect = GetComponent<RectTransform>();

        Hide();
    }

    /// <summary>
    /// Call every frame while dragging.
    /// screenStart and screenEnd are raw screen-space pixel positions.
    /// </summary>
    public void UpdateBox(Vector2 screenStart, Vector2 screenEnd)
    {
        boxRect.gameObject.SetActive(true);

        // Compute center and size in screen space
        Vector2 center = (screenStart + screenEnd) / 2f;
        Vector2 size = new Vector2(
            Mathf.Abs(screenEnd.x - screenStart.x),
            Mathf.Abs(screenEnd.y - screenStart.y)
        );

        // Canvas is Screen Space Overlay, so screen pixels == canvas pixels
        boxRect.position = center;   // world-position works directly for Overlay canvas
        boxRect.sizeDelta = size;
    }

    /// <summary>Call when drag ends or is cancelled.</summary>
    public void Hide()
    {
        boxRect.gameObject.SetActive(false);
    }
}