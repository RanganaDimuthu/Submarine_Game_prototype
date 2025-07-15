using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class SquarePairingGame : MonoBehaviour
{
    private List<Square> selectedSquares = new List<Square>();
    private int totalPairs = 8; // 16 buttons (8 pairs, 2 pairs per color)
    private int matchedPairs = 0;
    private bool isCheckingMatch = false;

    [SerializeField] private float matchCheckDelay = 2f; // Matches Square's delayBeforeFlipBack

    [Header("Gift Buttons")]
    [SerializeField] private Button redGiftButton; // Assign in Inspector
    [SerializeField] private Sprite redLockedSprite; // Locked image for Red Gift
    [SerializeField] private Sprite redUnlockedSprite; // Unlocked image for Red Gift
    [SerializeField] private Button blueGiftButton; // Assign in Inspector
    [SerializeField] private Sprite blueLockedSprite; // Locked image for Blue Gift
    [SerializeField] private Sprite blueUnlockedSprite; // Unlocked image for Blue Gift
    [SerializeField] private Button greenGiftButton; // Assign in Inspector
    [SerializeField] private Sprite greenLockedSprite; // Locked image for Green Gift
    [SerializeField] private Sprite greenUnlockedSprite; // Unlocked image for Green Gift
    [SerializeField] private Button yellowGiftButton; // Assign in Inspector
    [SerializeField] private Sprite yellowLockedSprite; // Locked image for Yellow Gift
    [SerializeField] private Sprite yellowUnlockedSprite; // Unlocked image for Yellow Gift

    [Header("Gift Panels")]
    [SerializeField] private GameObject redGiftPanel; // Assign in Inspector
    [SerializeField] private Sprite redGiftSprite; // Gift image for Red panel
    [SerializeField] private GameObject blueGiftPanel; // Assign in Inspector
    [SerializeField] private Sprite blueGiftSprite; // Gift image for Blue panel
    [SerializeField] private GameObject greenGiftPanel; // Assign in Inspector
    [SerializeField] private Sprite greenGiftSprite; // Gift image for Green panel
    [SerializeField] private GameObject yellowGiftPanel; // Assign in Inspector
    [SerializeField] private Sprite yellowGiftSprite; // Gift image for Yellow panel

    [Header("Audio")]
    [SerializeField] private AudioClip selectSquareSound; // Sound for selecting a square
    [SerializeField] private AudioClip matchSound; // Sound for successful match
    [SerializeField] private AudioClip noMatchSound; // Sound for failed match
    [SerializeField] private AudioClip unlockGiftSound; // Sound for unlocking gift button
    [SerializeField] private AudioClip panelOpenSound; // Sound for opening gift panel
    [SerializeField] private AudioClip panelCloseSound; // Sound for closing gift panel

    private AudioSource audioSource;

    private Dictionary<string, int> revealedCardsByColor = new Dictionary<string, int>();
    private Dictionary<string, Button> giftButtonsByColor = new Dictionary<string, Button>();
    private Dictionary<string, Sprite> lockedSpritesByColor = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> unlockedSpritesByColor = new Dictionary<string, Sprite>();
    private Dictionary<string, GameObject> giftPanelsByColor = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> giftSpritesByColor = new Dictionary<string, Sprite>();
    private readonly int cardsPerColor = 4; // Each color has 4 cards


    public GameObject PowerUpOrb;
    private bool Completed;
    public GameManager gamemanager;

    private void Start()
    {
        // Initialize AudioSource
        if (!TryGetComponent<AudioSource>(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Initialize gift buttons and sprites dictionaries
        giftButtonsByColor.Add("red", redGiftButton);
        giftButtonsByColor.Add("blue", blueGiftButton);
        giftButtonsByColor.Add("green", greenGiftButton);
        giftButtonsByColor.Add("yellow", yellowGiftButton);

        lockedSpritesByColor.Add("red", redLockedSprite);
        lockedSpritesByColor.Add("blue", blueLockedSprite);
        lockedSpritesByColor.Add("green", greenLockedSprite);
        lockedSpritesByColor.Add("yellow", yellowLockedSprite);

        unlockedSpritesByColor.Add("red", redUnlockedSprite);
        unlockedSpritesByColor.Add("blue", blueUnlockedSprite);
        unlockedSpritesByColor.Add("green", greenUnlockedSprite);
        unlockedSpritesByColor.Add("yellow", yellowUnlockedSprite);

        // Initialize gift panels and sprites dictionaries
        giftPanelsByColor.Add("red", redGiftPanel);
        giftPanelsByColor.Add("blue", blueGiftPanel);
        giftPanelsByColor.Add("green", greenGiftPanel);
        giftPanelsByColor.Add("yellow", yellowGiftPanel);

        giftSpritesByColor.Add("red", redGiftSprite);
        giftSpritesByColor.Add("blue", blueGiftSprite);
        giftSpritesByColor.Add("green", greenGiftSprite);
        giftSpritesByColor.Add("yellow", yellowGiftSprite);

        // Set initial state for gift buttons and panels
        foreach (var pair in giftButtonsByColor)
        {
            if (pair.Value != null)
            {
                pair.Value.interactable = false;
                Image buttonImage = pair.Value.GetComponent<Image>();
                if (buttonImage != null && lockedSpritesByColor[pair.Key] != null)
                {
                    buttonImage.sprite = lockedSpritesByColor[pair.Key];
                    Debug.Log($"Set {pair.Key} Gift Button to locked sprite.");
                }
                else
                {
                    Debug.LogWarning($"Image component or locked sprite missing for {pair.Key} Gift Button!");
                }
                pair.Value.onClick.AddListener(() => OnGiftButtonClicked(pair.Key));
                Debug.Log($"Initialized {pair.Key} Gift Button: {(pair.Value != null ? "Assigned" : "Null")}");
            }
            else
            {
                Debug.LogWarning($"Gift button for {pair.Key} is not assigned in Inspector!");
            }

            // Disable gift panel and ensure CanvasGroup for fade effect
            if (giftPanelsByColor[pair.Key] != null)
            {
                CanvasGroup canvasGroup = giftPanelsByColor[pair.Key].GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = giftPanelsByColor[pair.Key].AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0f;
                giftPanelsByColor[pair.Key].SetActive(false);
                Debug.Log($"Initialized {pair.Key} Gift Panel: Disabled with CanvasGroup");
            }
            else
            {
                Debug.LogWarning($"Gift panel for {pair.Key} is not assigned in Inspector!");
            }
        }

        // Initialize revealed cards tracker dynamically
        Square[] squares = FindObjectsOfType<Square>();
        foreach (Square square in squares)
        {
            square.squarePairingGame = this; // Assign this manager to each Square
            if (!string.IsNullOrEmpty(square.color) && !revealedCardsByColor.ContainsKey(square.color))
            {
                revealedCardsByColor.Add(square.color, 0);
                Debug.Log($"Initialized color tracker for: {square.color}");
            }
        }

        // Log all colors found
        Debug.Log($"Tracked colors: {string.Join(", ", revealedCardsByColor.Keys)}");
    }

    private void Update()
    {
        if(Completed &&  totalPairs == 8)
        {
            PowerUpOrb.SetActive(true);
            gamemanager.Addscore(80);
            Completed = false;
        }
    }

    public void OnSquareSelected(Square square)
    {
        if (isCheckingMatch || square.IsRevealed || selectedSquares.Contains(square)) return;

        // Play select square sound
        if (selectSquareSound != null)
        {
            audioSource.PlayOneShot(selectSquareSound);
        }

        Debug.Log($"Selected square {square.gameObject.name} with color: {square.color}, IsRevealed: {square.IsRevealed}");
        selectedSquares.Add(square);

        if (selectedSquares.Count == 2)
        {
            isCheckingMatch = true;
            CheckForMatch();
        }
    }

    private void CheckForMatch()
    {
        Square firstSquare = selectedSquares[0];
        Square secondSquare = selectedSquares[1];

        Debug.Log($"Checking match: {firstSquare.color} (Square {firstSquare.gameObject.name}) vs {secondSquare.color} (Square {secondSquare.gameObject.name})");

        if (firstSquare.color == secondSquare.color)
        {
            // Play match sound
            if (matchSound != null)
            {
                audioSource.PlayOneShot(matchSound);
            }

            // Match found
            firstSquare.IsRevealed = true;
            secondSquare.IsRevealed = true;
            matchedPairs++;
            Debug.Log($"Match found for {firstSquare.color}! Setting IsRevealed = true for {firstSquare.gameObject.name} and {secondSquare.gameObject.name}. Total matched pairs: {matchedPairs}");
            UpdateRevealedCards(firstSquare.color);

            selectedSquares.Clear();
            isCheckingMatch = false;

            if (matchedPairs >= totalPairs)
            {
                Debug.Log("Game Won! All pairs matched!");
                // Add game win logic here (e.g., show win screen)
            }
        }
        else
        {
            // Play no-match sound
            if (noMatchSound != null)
            {
                audioSource.PlayOneShot(noMatchSound);
            }

            // No match, flip back after delay
            Debug.Log($"No match between {firstSquare.color} and {secondSquare.color}. Flipping back after delay.");
            DOVirtual.DelayedCall(matchCheckDelay, () =>
            {
                if (!firstSquare.IsRevealed) firstSquare.FlipBack();
                if (!secondSquare.IsRevealed) secondSquare.FlipBack();
                selectedSquares.Clear();
                isCheckingMatch = false;
            });
        }
    }

    private void UpdateRevealedCards(string color)
    {
        if (revealedCardsByColor.ContainsKey(color))
        {
            revealedCardsByColor[color] += 2; // Two cards matched
            Debug.Log($"Revealed {revealedCardsByColor[color]} cards for color {color}");

            // Fallback: Verify revealed count directly
            int actualRevealed = CountRevealedCards(color);
            Debug.Log($"Fallback check: {actualRevealed} cards revealed for color {color}");

            if (revealedCardsByColor[color] >= cardsPerColor || actualRevealed >= cardsPerColor)
            {
                UnlockGiftButton(color);
            }
        }
        else
        {
            Debug.LogError($"Color {color} not found in revealedCardsByColor dictionary!");
        }
    }

    private int CountRevealedCards(string color)
    {
        int count = 0;
        Square[] squares = FindObjectsOfType<Square>();
        foreach (Square square in squares)
        {
            if (square.color == color && square.IsRevealed)
            {
                count++;
            }
        }
        return count;
    }

    private void UnlockGiftButton(string color)
    {
        if (giftButtonsByColor.ContainsKey(color) && giftButtonsByColor[color] != null)
        {
            // Play unlock sound
            if (unlockGiftSound != null)
            {
                audioSource.PlayOneShot(unlockGiftSound);
            }

            // Add scale punch effect
            Transform buttonTransform = giftButtonsByColor[color].transform;
            buttonTransform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => buttonTransform.DOScale(1f, 0.2f));

            giftButtonsByColor[color].interactable = true;
            Image buttonImage = giftButtonsByColor[color].GetComponent<Image>();
            if (buttonImage != null && unlockedSpritesByColor.ContainsKey(color) && unlockedSpritesByColor[color] != null)
            {
                buttonImage.sprite = unlockedSpritesByColor[color];
                Debug.Log($"Unlocked {color} Gift Button and set to unlocked sprite!");
            }
            else
            {
                Debug.LogWarning($"Image component or unlocked sprite missing for {color} Gift Button! Button is still interactable.");
            }
        }
        else
        {
            Debug.LogWarning($"No gift button assigned for color {color}!");
        }
    }

    private void OnGiftButtonClicked(string color)
    {
        if (giftPanelsByColor.ContainsKey(color) && giftPanelsByColor[color] != null)
        {
            CanvasGroup canvasGroup = giftPanelsByColor[color].GetComponent<CanvasGroup>();
            bool isPanelActive = giftPanelsByColor[color].activeSelf;

            if (isPanelActive)
            {
                // Play close sound
                if (panelCloseSound != null)
                {
                    audioSource.PlayOneShot(panelCloseSound);
                }

                // Fade out panel
                if (canvasGroup != null)
                {
                    canvasGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
                    {
                        giftPanelsByColor[color].SetActive(false);
                        Debug.Log($"Closed {color} Gift Panel.");
                    });
                }
                else
                {
                    giftPanelsByColor[color].SetActive(false);
                    Debug.Log($"Closed {color} Gift Panel (no CanvasGroup).");
                }
            }
            else
            {
                // Play open sound
                if (panelOpenSound != null)
                {
                    audioSource.PlayOneShot(panelOpenSound);
                }
                /*
                // Close other panels to avoid overlap
                foreach (var panelPair in giftPanelsByColor)
                {
                    if (panelPair.Key != color && panelPair.Value != null)
                    {
                        CanvasGroup otherCanvasGroup = panelPair.Value.GetComponent<CanvasGroup>();
                        if (otherCanvasGroup != null)
                        {
                            otherCanvasGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad).OnComplete(() => panelPair.Value.SetActive(false));
                        }
                        else
                        {
                            panelPair.Value.SetActive(false);
                        }
                    }
                }*/

                // Open the panel and set gift image
                giftPanelsByColor[color].SetActive(true);
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
                }

                Image giftImage = giftPanelsByColor[color].GetComponentInChildren<Image>(true);
                if (giftImage != null && giftSpritesByColor[color] != null)
                {
                    // Find the specific gift image
                    Image[] images = giftPanelsByColor[color].GetComponentsInChildren<Image>(true);
                    foreach (Image img in images)
                    {
                        if (img.gameObject.name.Contains("GiftImage"))
                        {
                            giftImage = img;
                            break;
                        }
                    }
                    if (giftImage != null)
                    {
                        giftImage.sprite = giftSpritesByColor[color];
                        Debug.Log($"Opened {color} Gift Panel with gift sprite.");
                    }
                    else
                    {
                        Debug.LogWarning($"GiftImage component not found in {color} Gift Panel!");
                    }
                }
                else
                {
                    Debug.LogWarning($"Gift image component or sprite missing for {color} Gift Panel!");
                }
            }
        }
        else
        {
            Debug.LogWarning($"No gift panel assigned for color {color}!");
        }
    }

    public void CloseGiftPanel(string color)
    {
        if (giftPanelsByColor.ContainsKey(color) && giftPanelsByColor[color] != null)
        {
            // Play close sound
            if (panelCloseSound != null)
            {
                audioSource.PlayOneShot(panelCloseSound);
            }

            // Fade out panel
            CanvasGroup canvasGroup = giftPanelsByColor[color].GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    giftPanelsByColor[color].SetActive(false);
                    Debug.Log($"Closed {color} Gift Panel via Close button.");
                });
            }
            else
            {
                giftPanelsByColor[color].SetActive(false);
                Debug.Log($"Closed {color} Gift Panel via Close button (no CanvasGroup).");
            }
        }
    }

    public bool CanSelectSquare()
    {
        return selectedSquares.Count < 2 && !isCheckingMatch;
    }

    [ContextMenu("Log Revealed Counts")]
    private void LogRevealedCounts()
    {
        foreach (var pair in revealedCardsByColor)
        {
            int actualCount = CountRevealedCards(pair.Key);
            Debug.Log($"Color {pair.Key}: {pair.Value} cards revealed (Dictionary), {actualCount} cards revealed (Actual)");
        }
    }

    [ContextMenu("Log All Square Colors")]
    private void LogAllSquareColors()
    {
        Square[] squares = FindObjectsOfType<Square>();
        foreach (Square square in squares)
        {
            Debug.Log($"Square {square.gameObject.name}: Color = {square.color}, IsRevealed = {square.IsRevealed}");
        }
    }
}