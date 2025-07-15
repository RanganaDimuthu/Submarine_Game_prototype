using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button challengeButton;
    [SerializeField] private Button challengeInfoButton;
    [SerializeField] private Button guestInfoButton;
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private CanvasGroup loadingCanvasGroup;
    [SerializeField] private GameObject loadingIcon;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private CanvasGroup infoPanelCanvasGroup;
    [SerializeField] private TextMeshProUGUI infoPanelText;
    [SerializeField] private Button infoPanelCloseButton;
    [SerializeField] private CanvasGroup titleScreenCanvasGroup;

    [Header("Audio Elements")]
    [SerializeField] private AudioSource buttonClickAudio;
    [SerializeField] private AudioSource loadingSonarAudio;
    [SerializeField] private AudioClip bubbleClickClip;
    [SerializeField] private AudioClip sonarPingClip;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float buttonScaleDuration = 0.3f;
    [SerializeField] private float buttonPunchScale = 0.1f;
    [SerializeField] private float panelPopDuration = 0.5f;
    [SerializeField] private float titleScreenDuration = 2.5f;

    private void Start()
    {
        // Initialize canvas groups
        mainMenuCanvasGroup.alpha = 0f;
        mainMenuCanvasGroup.gameObject.SetActive(false);
        loadingCanvasGroup.alpha = 0f;
        loadingCanvasGroup.gameObject.SetActive(false);
        infoPanelCanvasGroup.alpha = 0f;
        infoPanelCanvasGroup.gameObject.SetActive(false);
        titleScreenCanvasGroup.alpha = 0f;

        // Ensure loading icon is inactive initially
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(false);
        }

        // Start title screen sequence
        StartCoroutine(ShowTitleScreen());
    }

    private IEnumerator ShowTitleScreen()
    {
        // Fade in title screen
        titleScreenCanvasGroup.gameObject.SetActive(true);
        yield return titleScreenCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();

        // Hold title screen for duration
        yield return new WaitForSeconds(titleScreenDuration);

        // Fade out title screen
        yield return titleScreenCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
        titleScreenCanvasGroup.gameObject.SetActive(false);

        // Show main menu
        mainMenuCanvasGroup.gameObject.SetActive(true);
        mainMenuCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        challengeButton.transform.localScale = Vector3.zero;
        challengeButton.transform.DOScale(1f, buttonScaleDuration).SetEase(Ease.OutBack);
        challengeInfoButton.transform.localScale = Vector3.zero;
        challengeInfoButton.transform.DOScale(1f, buttonScaleDuration).SetEase(Ease.OutBack).SetDelay(0.1f);
        guestInfoButton.transform.localScale = Vector3.zero;
        guestInfoButton.transform.DOScale(1f, buttonScaleDuration).SetEase(Ease.OutBack).SetDelay(0.2f);

        // Add button listeners
        challengeButton.onClick.AddListener(OnChallengeButtonClicked);
        challengeInfoButton.onClick.AddListener(() => ShowInfoPanel("Challenge Mode", "Navigate your submarine through treacherous underwater challenges, avoiding obstacles and enemies to reach the goal!"));
        guestInfoButton.onClick.AddListener(() => ShowInfoPanel("Guest Mode", "Explore the underwater world freely with no objectives, perfect for a relaxing submarine adventure."));
        infoPanelCloseButton.onClick.AddListener(CloseInfoPanel);
    }

    private void OnChallengeButtonClicked()
    {
        // Play bubble click sound
        if (buttonClickAudio != null && bubbleClickClip != null)
        {
            buttonClickAudio.PlayOneShot(bubbleClickClip);
        }

        // Button click animation
        challengeButton.transform.DOPunchScale(Vector3.one * buttonPunchScale, buttonScaleDuration, 2, 1f)
            .OnComplete(() => StartCoroutine(LoadChallengeMode()));
    }

    private void ShowInfoPanel(string title, string description)
    {
        // Play bubble click sound
        if (buttonClickAudio != null && bubbleClickClip != null)
        {
            buttonClickAudio.PlayOneShot(bubbleClickClip);
        }

        // Set panel content
        infoPanelText.text = $"**{title}**\n\n{description}";

        // Show and animate panel
        infoPanelCanvasGroup.gameObject.SetActive(true);
        infoPanelCanvasGroup.alpha = 0f;
        infoPanelCanvasGroup.transform.localScale = Vector3.one * 0.8f;
        infoPanelCanvasGroup.DOFade(1f, panelPopDuration).SetEase(Ease.InOutQuad);
        infoPanelCanvasGroup.transform.DOScale(1f, panelPopDuration).SetEase(Ease.OutBack);
    }

    private void CloseInfoPanel()
    {
        // Play bubble click sound
        if (buttonClickAudio != null && bubbleClickClip != null)
        {
            buttonClickAudio.PlayOneShot(bubbleClickClip);
        }

        // Animate panel closing
        infoPanelCanvasGroup.DOFade(0f, panelPopDuration).SetEase(Ease.InOutQuad);
        infoPanelCanvasGroup.transform.DOScale(0.8f, panelPopDuration).SetEase(Ease.InBack)
            .OnComplete(() => infoPanelCanvasGroup.gameObject.SetActive(false));
    }

    private IEnumerator LoadChallengeMode()
    {
        // Fade out main menu
        yield return mainMenuCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
        mainMenuCanvasGroup.gameObject.SetActive(false);

        // Show and fade in loading screen
        loadingCanvasGroup.gameObject.SetActive(true);
        loadingCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);

        // Activate loading icon (assumes it has its own animation)
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(true);
        }

        // Play sonar ping sound
        if (loadingSonarAudio != null && sonarPingClip != null)
        {
            loadingSonarAudio.clip = sonarPingClip;
            loadingSonarAudio.loop = true;
            loadingSonarAudio.Play();
        }

        // Simulate loading time
        yield return new WaitForSeconds(2f);

        // Load Level_1 with async operation
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SeeDemo");
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                // Stop sonar sound
                if (loadingSonarAudio != null)
                {
                    loadingSonarAudio.Stop();
                }

                // Deactivate loading icon
                if (loadingIcon != null)
                {
                    loadingIcon.SetActive(false);
                }

                // Fade out loading screen
                yield return loadingCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
                loadingCanvasGroup.gameObject.SetActive(false);
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private void OnDestroy()
    {
        // Clean up DOTween animations
        DOTween.KillAll();

        // Stop any playing audio
        if (loadingSonarAudio != null)
        {
            loadingSonarAudio.Stop();
        }
    }
}