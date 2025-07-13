using UnityEngine;
using TMPro;

public class NPCBehaviour : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool isFullyComplete = false;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private string halfCompleteDialogue = "I still need more help...";
    [SerializeField] private string fullCompleteDialogue = "Thanks! Everything's done.";
    [SerializeField] private float dialogueDuration = 3f;

    [Header("NPC Name")]
    [SerializeField] private string npcName = "NPC Name";
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] GameObject SpeechIcon;
    [SerializeField] GameObject GameHud;

    private void Start()
    {
        if (npcNameText != null)
            npcNameText.text = npcName;

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            SpeechIcon.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SpeechIcon.gameObject.SetActive(false);
        }
    }

    public void Interact()
    {
        string message = isFullyComplete ? fullCompleteDialogue : halfCompleteDialogue;
        ShowDialogue(message);
        
    }

    private void ShowDialogue(string message)
    {
        if (dialogueCanvas == null || dialogueText == null) return;
        GameHud.SetActive(false);
        dialogueCanvas.SetActive(true);
        dialogueText.text = message;
        CancelInvoke(nameof(HideDialogue));
        Invoke(nameof(HideDialogue), dialogueDuration);
    }

    private void HideDialogue()
    {
        GameHud.SetActive(true);
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);
    }

    // You can call this from other scripts when the quest is completed
    public void SetCompleteState(bool complete)
    {
        isFullyComplete = complete;
    }

    public bool IsComplete()
    {
        return isFullyComplete;
    }
}
