using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Square : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float flipDuration = 0.5f;
    [SerializeField] private float delayBeforeFlipBack = 2f;

    [Header("Images")]
    [SerializeField] private Image frontImage; // Assign in Inspector
    [SerializeField] private Image backImage;  // Assign in Inspector

    [Header("Audio")]
    [SerializeField] private AudioClip flipSound; // Sound for flipping to back
    [SerializeField] private AudioClip flipBackSound; // Sound for flipping to front

    private bool isRotating = false;
    public bool IsRevealed;

    [Header("References")]
    public SquarePairingGame squarePairingGame; // Reference to the game manager

    [Header("Card")]
    [SerializeField] public string color;

    private AudioSource audioSource;

    private void Start()
    {
        // Ensure backImage is hidden initially
        backImage.gameObject.SetActive(false);

        // Add button component if missing
        if (!TryGetComponent<Button>(out var button))
        {
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(OnSquareClicked);

        // Add AudioSource if missing
        if (!TryGetComponent<AudioSource>(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnSquareClicked()
    {
        // Check if the manager allows selecting this square
        if (squarePairingGame == null || !squarePairingGame.CanSelectSquare() || isRotating || IsRevealed) return;

        isRotating = true;
        squarePairingGame.OnSquareSelected(this); // Notify manager

        // Play flip sound
        if (flipSound != null)
        {
            audioSource.PlayOneShot(flipSound);
        }

        // Add scale punch effect
        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => transform.DOScale(1f, 0.2f));

        // First half of flip (90°): Hide front, show back
        transform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                frontImage.gameObject.SetActive(false);
                backImage.gameObject.SetActive(true);

                // Second half of flip (to 180°)
                transform.DORotate(new Vector3(0, 180, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        isRotating = false; // Animation complete, allow manager to handle logic
                    });
            });
    }

    public void FlipBack()
    {
        if (IsRevealed) return; // Prevent flipping back if matched

        // Play flip back sound
        if (flipBackSound != null)
        {
            audioSource.PlayOneShot(flipBackSound);
        }

        // Add scale punch effect
        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => transform.DOScale(1f, 0.2f));

        // First half of reverse flip (90°): Hide back, show front
        transform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                backImage.gameObject.SetActive(false);
                frontImage.gameObject.SetActive(true);

                // Second half (back to 0°)
                transform.DORotate(new Vector3(0, 0, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => isRotating = false);
            });
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}