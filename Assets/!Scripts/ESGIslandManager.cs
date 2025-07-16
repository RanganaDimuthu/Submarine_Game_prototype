using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class ESGIslandController : MonoBehaviour
{
    public bool isAnswering = false;
    public bool isAnswered = false;

    [Header("Island Settings")]
    public string islandName;
    public string questionText;
    public GameObject dimmedVersion;
    public GameObject litVersion;
    public GameObject popUpPod;
    public GameObject factCard;

    [Header("Interaction")]
    public Button[] choiceButtons;

    [Header("Correct Answers")]
    public bool isACorrect;
    public bool isBCorrect;
    public bool isCCorrect;
    public bool isDCorrect;
    public bool isECorrect;

    public UnityEvent onCorrectInteraction;

    [Header("Optional Text Feedback")]
    public TMP_Text feedbackText;

    private bool[] clicked;
    private int selectedCount = 0;
    private int attempts = 0;
    public bool isCompleted = false;

    void Start()
    {
        SetDimmedState();
        SetupButtons();

        if (feedbackText != null)
            feedbackText.text = questionText;
    }

    public void OnIslandClicked()
    {
        if (!isCompleted && !isAnswering)
        {
            popUpPod.SetActive(true);
            isAnswering = true;
            ToggleIslandClickers(false);
        }
    }

    private void CompleteIsland()
    {
        isCompleted = true;
        popUpPod.SetActive(false);
        factCard.SetActive(true);
        SetLitState();
        onCorrectInteraction?.Invoke();

        isAnswering = false;
        ToggleIslandClickers(true);
    }

    private void ToggleIslandClickers(bool state)
    {
        IslandClickDetector[] allDetectors = FindObjectsOfType<IslandClickDetector>();
        foreach (IslandClickDetector isc in allDetectors)
        {
            MeshCollider mc = isc.GetComponent<MeshCollider>();
            if (mc != null)
                mc.enabled = state;
        }
    }

    private void SetDimmedState()
    {
        dimmedVersion.SetActive(true);
        litVersion.SetActive(false);
    }

    private void SetLitState()
    {
        dimmedVersion.SetActive(false);
        litVersion.SetActive(true);
    }

    private void SetupButtons()
    {
        clicked = new bool[choiceButtons.Length];

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int index = i;
            choiceButtons[index].onClick.RemoveAllListeners();
            

            choiceButtons[index].onClick.AddListener(() =>
            {
                if (isCompleted || clicked[index] || attempts >= 2) return;

                clicked[index] = true;
                attempts++;

                bool isCorrect = IsButtonCorrect(index);
                var img = choiceButtons[index].GetComponent<Image>();

                if (isCorrect)
                {
                    img.color = Color.green;
                    selectedCount++;
                    if(attempts == 1)
                    {
                        StartCoroutine(ResetAndComplete());
                    }
                    if (feedbackText != null)
                        feedbackText.text = attempts == 1 ? "You got it!" : "Correct!";

                    StartCoroutine(ResetAndComplete());
                }
                else
                {
                    img.color = Color.red;

                    if (feedbackText != null)
                        feedbackText.text = attempts < 2
                            ? "That dims the reef’s future... try again."
                            : "Out of chances.";

                    if (attempts >= 2 && selectedCount == 0)
                    {
                        StartCoroutine(HandleFailedAttempt());
                    }
                    else
                    {
                        StartCoroutine(ResetQuestionText());
                    }
                }
            });
        }
    }

    private bool IsButtonCorrect(int index)
    {
        return index switch
        {
            0 => isACorrect,
            1 => isBCorrect,
            2 => isCCorrect,
            3 => isDCorrect,
            4 => isECorrect,
            _ => false,
        };
    }

    IEnumerator HandleFailedAttempt()
    {
        yield return new WaitForSeconds(2.5f);
        if (feedbackText != null)
            feedbackText.text = questionText;

        popUpPod.SetActive(false);
        isAnswered = true;
        isAnswering = false;
        ToggleIslandClickers(true);
    }

    IEnumerator ResetAndComplete()
    {
        yield return new WaitForSeconds(1f);
        if (feedbackText != null)
            feedbackText.text = questionText;
        isAnswered = true;
        CompleteIsland();
    }

    IEnumerator ResetQuestionText()
    {
        yield return new WaitForSeconds(2f);
        if (feedbackText != null)
            feedbackText.text = questionText;
    }
}
