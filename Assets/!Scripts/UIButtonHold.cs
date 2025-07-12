using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hold Settings")]
    public float holdDuration = 3f; // seconds to hold
    public Image fillImage;         // assign a UI Image (e.g. radial or bar)
    public UnityEngine.Events.UnityEvent onHoldComplete;

    private float holdTimer = 0f;
    private bool isHolding = false;

    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            fillImage.fillAmount = holdTimer / holdDuration;

            if (holdTimer >= holdDuration)
            {
                isHolding = false;
                onHoldComplete.Invoke();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        holdTimer = 0f;
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        fillImage.fillAmount = 0f;
    }
}
