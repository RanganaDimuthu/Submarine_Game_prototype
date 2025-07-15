using UnityEngine;
using DG.Tweening;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] panels; // Array to hold the 7 UI panels
    [SerializeField] private int selectedPanelIndex; // Index of the panel to open, set in Inspector
    private GameObject currentPanel; // Tracks the currently open panel

    private void Start()
    {
        // Initialize all panels as inactive
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
    }

    // Function to open the panel selected in the Inspector
    public void OpenPanel()
    {
        // Validate panel index
        if (selectedPanelIndex < 0 || selectedPanelIndex >= panels.Length)
        {
            Debug.LogWarning("Invalid panel index set in Inspector!");
            return;
        }

        // Close current panel if one is open
        if (currentPanel != null)
        {
            ClosePanel();
        }

        // Set the new panel as active
        currentPanel = panels[selectedPanelIndex];
        currentPanel.SetActive(true);

        // Reset panel's initial state for animation
        RectTransform panelRect = currentPanel.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = currentPanel.GetComponent<CanvasGroup>();

        // Ensure CanvasGroup exists for fade animations
        if (canvasGroup == null)
        {
            canvasGroup = currentPanel.AddComponent<CanvasGroup>();
        }

        // Initialize DOTween
        DOTween.Kill(panelRect); // Clear any existing animations

        // Apply a default scale animation
        panelRect.localScale = Vector3.zero;
        panelRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad);
    }

    // Function to close the currently selected panel with animation
    public void ClosePanel()
    {
        if (currentPanel == null)
        {
            return;
        }

        RectTransform panelRect = currentPanel.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = currentPanel.GetComponent<CanvasGroup>();

        // Ensure CanvasGroup exists
        if (canvasGroup == null)
        {
            canvasGroup = currentPanel.AddComponent<CanvasGroup>();
        }

        // Clear any existing animations
        DOTween.Kill(panelRect);
        DOTween.Kill(canvasGroup);

        // Animate scale down and fade out
        Sequence closeSequence = DOTween.Sequence();
        closeSequence.Append(panelRect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));
        closeSequence.Join(canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InQuad));
        closeSequence.OnComplete(() =>
        {
            currentPanel.SetActive(false);
            Interactive.Interacting = false;
            // Reset scale and alpha for future animations
            panelRect.localScale = Vector3.one;
            canvasGroup.alpha = 1f;
            currentPanel = null;
        });
    }
}