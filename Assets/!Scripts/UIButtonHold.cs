using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hold Settings")]
    public float holdDuration = 5f;
    public UnityEvent onHoldStart;
    public UnityEvent onHoldStartInvert;
    public UnityEvent onHoldComplete;
    [SerializeField] Image fillImage;
    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool hasStarted = false;


    [SerializeField] GameObject s1, s2, s3, s4, s5, sdefault;

    void Update()
    {
        if (isHolding)
        {
            if (!hasStarted)
            {
                hasStarted = true;
                onHoldStart.Invoke();
            }
            holdTimer += Time.deltaTime;
            fillImage.fillAmount = holdTimer / holdDuration;

           
            
            float percent = holdTimer / holdDuration;

            // Update which image is active
            UpdateCountdownVisual(percent);

            if (holdTimer >= holdDuration)
            {
                isHolding = false;
                hasStarted = false;
                ShowOnly(sdefault); // Reset to default
                onHoldComplete.Invoke();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        holdTimer = 0f;
        isHolding = true;
        hasStarted = false;
        ShowOnly(s1);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onHoldStartInvert.Invoke();
        isHolding = false;
        hasStarted = false;
        ShowOnly(sdefault);
    }

    void UpdateCountdownVisual(float percent)
    {
        if (percent < 0.2f)
            ShowOnly(s1);
        else if (percent < 0.4f)
            ShowOnly(s2);
        else if (percent < 0.6f)
            ShowOnly(s3);
        else if (percent < 0.8f)
            ShowOnly(s4);
        else if (percent < 1f)
            ShowOnly(s5);
    }

    void ShowOnly(GameObject target)
    {
        s1.SetActive(target == s1);
        s2.SetActive(target == s2);
        s3.SetActive(target == s3);
        s4.SetActive(target == s4);
        s5.SetActive(target == s5);
        sdefault.SetActive(target == sdefault);
    }
}
