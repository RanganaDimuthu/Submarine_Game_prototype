using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro; // <<< ADD THIS for TMP support

public class TargetManager : MonoBehaviour
{
    [Header("Progress Settings")]
    public int CurrentScro;
    [SerializeField] int TargetScro;

    [Header("Task UI")]
    [SerializeField] Text TargetedText; // Still using normal Text here for your progress bar text
    [SerializeField] TMP_Text TitleText; // <<< TMP
    [SerializeField] TMP_Text DescriptionText; // <<< TMP
    [SerializeField] Slider ProgressSlider;

    [Header("Task Details")]
    [SerializeField] string DisplayName;
    [TextArea]
    [SerializeField] string Description;

    [Header("Events")]
    [SerializeField] UnityEvent WhenTargetFinished;

    private void Start()
    {
        Update_TaskUI();
    }

    private void Update()
    {
        Update_TaskUI();
        if (CurrentScro >= TargetScro)
        {
            WhenTargetFinished.Invoke();
        }
    }

    public void Add_a_Point(int Addp)
    {
        CurrentScro += Addp;
        Update_TaskUI();
    }

    void Update_TaskUI()
    {
        if (TargetedText != null)
            TargetedText.text = "Progress" + CurrentScro + "/" + TargetScro;

        if (TitleText != null)
            TitleText.text = DisplayName;

        if (DescriptionText != null)
            DescriptionText.text = Description;

        if (ProgressSlider != null)
        {
            ProgressSlider.maxValue = TargetScro;
            ProgressSlider.value = CurrentScro;
        }
    }

    public void Reset_Task()
    {
        CurrentScro = 0;
        Update_TaskUI();
    }
}
