using UnityEngine;

public class ColorLerpController : MonoBehaviour
{
    [Header("Settings")]
    public bool isCharging = false;               // Your control bool
    public float lerpSpeed = 2f;                  // Speed of transition
    public Renderer targetRenderer;               // The Renderer with the material

    private Color colorStart = Color.black;
    private Color colorEnd = Color.white;
    private float lerpValue = 0f;

    void Update()
    {
        // Smoothly adjust lerp value based on the bool
        if (isCharging)
        {
            lerpValue = Mathf.MoveTowards(lerpValue, 1f, Time.deltaTime * lerpSpeed);
        }
        else
        {
            lerpValue = Mathf.MoveTowards(lerpValue, 0f, Time.deltaTime * lerpSpeed);
        }

        // Apply color based on current lerpValue
        Color lerpedColor = Color.Lerp(colorStart, colorEnd, lerpValue);
        targetRenderer.material.color = lerpedColor;
    }

    public void SetState(bool State)
    {
        isCharging=State;
    }
}
