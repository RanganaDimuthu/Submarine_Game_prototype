using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[System.Serializable]
public class BallTargetPair
{
    public RectTransform ballRectTransform; // The Ball UI Image
    public RectTransform targetRectTransform; // The UI Image to destroy when Ball is collected
}

public class DragCharacter : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform[] wallRectTransforms; // Assign wall UI Images in Inspector
    [SerializeField] private BallTargetPair[] ballTargetPairs; // Assign Ball and target UI Image pairs in Inspector
    private Vector2 lastValidPosition;

    private void Awake()
    {
        // Get the required components
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Add CanvasGroup if not present
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Validate walls
        if (wallRectTransforms == null || wallRectTransforms.Length == 0)
        {
            Debug.LogWarning("No wall RectTransforms assigned in Inspector!");
        }
        else
        {
            foreach (var wall in wallRectTransforms)
            {
                if (wall == null)
                    Debug.LogWarning("One or more wall RectTransforms are null!");
            }
        }

        // Validate ball-target pairs
        if (ballTargetPairs == null || ballTargetPairs.Length == 0)
        {
            Debug.LogWarning("No ball-target pairs assigned in Inspector!");
        }
        else
        {
            foreach (var pair in ballTargetPairs)
            {
                if (pair.ballRectTransform == null)
                    Debug.LogWarning("One or more ball RectTransforms are null!");
                if (pair.targetRectTransform == null)
                    Debug.LogWarning("One or more target RectTransforms are null!");
            }
        }

        // Store initial position as last valid
        lastValidPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
        canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Calculate proposed new position
        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;

        // Get character rect at proposed position
        Rect characterRect = GetCanvasRect(rectTransform, newPosition);

        // Check for overlaps with walls
        bool canMove = true;
        foreach (RectTransform wall in wallRectTransforms)
        {
            if (wall == null || !wall.gameObject.activeSelf) continue;
            Rect wallRect = GetCanvasRect(wall, wall.anchoredPosition);

            if (characterRect.Overlaps(wallRect, true))
            {
                canMove = false;
                Debug.Log($"Collision detected with wall: {wall.gameObject.name}");
                break;
            }
        }

        // Update position if no collision with walls
        if (canMove)
        {
            rectTransform.anchoredPosition = newPosition;
            lastValidPosition = newPosition;

            // Check for overlaps with balls
            foreach (BallTargetPair pair in ballTargetPairs)
            {
                if (pair.ballRectTransform == null || !pair.ballRectTransform.gameObject.activeSelf) continue;
                Rect ballRect = GetCanvasRect(pair.ballRectTransform, pair.ballRectTransform.anchoredPosition);

                if (characterRect.Overlaps(ballRect, true))
                {
                    // Destroy the ball and its associated target UI Image
                    if (pair.ballRectTransform != null)
                    {
                        Debug.Log($"Ball collected: {pair.ballRectTransform.gameObject.name}");
                        Destroy(pair.ballRectTransform.gameObject);
                    }
                    if (pair.targetRectTransform != null)
                    {
                        Debug.Log($"Target UI Image destroyed: {pair.targetRectTransform.gameObject.name}");
                        Destroy(pair.targetRectTransform.gameObject);
                    }
                }
            }
        }
        else
        {
            rectTransform.anchoredPosition = lastValidPosition;
            Debug.Log("Movement blocked by wall, reverting to last valid position.");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
    }

    private Rect GetCanvasRect(RectTransform rt, Vector2 anchoredPos)
    {
        // Get size of the RectTransform
        Vector2 size = rt.sizeDelta;
        Vector2 pivot = rt.pivot;

        // Calculate the min and max corners in canvas space
        Vector2 min = anchoredPos - size * pivot;
        Vector2 max = min + size;

        return new Rect(min, size);
    }
}
