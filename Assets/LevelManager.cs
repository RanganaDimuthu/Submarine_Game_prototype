using UnityEngine;
using System.Collections;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public Transform TeleportPos;
    public GameObject Player, Submarine;
    public PuzzlePiece JigsawPuzzle;
    [SerializeField] public int ObjectstoInteract, InteractedObjects;
    public TextMeshProUGUI InfoText;
    [SerializeField] private CanvasGroup fadePanel; // Added for fade effect

    [SerializeField] GameObject HUD;

    private void Awake()
    {
        InfoText.gameObject.SetActive(false);
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f; // Ensure panel is transparent at start
        }
    }

    private void Update()
    {
        
        HUD.SetActive(!Interactive.Interacting);
    }


    public void Teleport()
    {
        StartCoroutine(TeleportPlayer());
    }

    public IEnumerator TeleportPlayer()
    {
        // Fade to black
        if (fadePanel != null)
        {
            float elapsedTime = 0f;
            float fadeDuration = 1f; // Duration of fade in seconds
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadePanel.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }
            fadePanel.alpha = 1f; // Ensure fully opaque
        }

        yield return new WaitForSeconds(2);
        if (JigsawPuzzle.PowerUpOrb.activeSelf)
        {
            Player.transform.position = TeleportPos.position;
            Submarine.transform.position = TeleportPos.position;
        }

        // Fade back to transparent
        if (fadePanel != null)
        {
            float elapsedTime = 0f;
            float fadeDuration = 1f; // Duration of fade in seconds
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadePanel.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                yield return null;
            }
            fadePanel.alpha = 0f; // Ensure fully transparent
        }
    }

    public void ANewObjectInteracted()
    {
        InteractedObjects++;
    }

    public void ShowInfoTextPrimaryObj()
    {
        InfoText.gameObject.SetActive(true);
        InfoText.text = " Objects Interacted" + InteractedObjects + "/" + ObjectstoInteract;
        StartCoroutine(HideText());
    }

    IEnumerator HideText()
    {
        yield return new WaitForSeconds(3);
        InfoText.gameObject.SetActive(false);
    }
}