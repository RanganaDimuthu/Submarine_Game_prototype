using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;
using TMPro;

public class PuzzlePiece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    
    private RectTransform rectTransform; private Canvas canvas; private CanvasGroup canvasGroup; private Vector2 originalPosition; private Transform originalParent;

    [Header("Snap Settings")]
    [SerializeField] private RectTransform targetTransform;
    [SerializeField] private float snapDistance = 50f;
    [SerializeField] private bool lockWhenCorrect = true;
    [SerializeField] private bool matchRotation = true;
    [SerializeField] private bool matchScale = true;

    [Header("Pop-Up Settings")]
    [SerializeField] private string pieceInfoText; // Text to display when this piece snaps
    [SerializeField] private Image sharedPopUpImage; // Shared background image for all pieces
    [SerializeField] private TMPro.TMP_Text sharedPopUpText; // TextMeshPro component on shared image
    [SerializeField] private float popUpDuration = 2f;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Final Pop-Ups")]
    [SerializeField] private Image finalPopUpImage1; // First final image
    [SerializeField] private Image finalPopUpImage2; // Second final image
    public GameObject PiecesPanel;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource snapSound; // Sound when piece snaps to target
    [SerializeField] private AudioSource completionSound; // Sound when puzzle is completed

    [Header("Squishy Effect Settings")]
    [SerializeField] private float squishDuration = 0.3f; // Duration of squishy effect for pop-ups
    [SerializeField] private float squishScale = 0.8f; // Scale factor for squish
    [SerializeField] private float stretchScale = 1.2f; // Scale factor for stretch

    public bool IsCorrect { get; private set; } = false;
    private static int correctPiecesCount = 0;
    private static int totalPieces = 0;
    private static bool isShowingFinalPopUp = false;

    private Coroutine currentPopUpCoroutine;


    public GameManager gamemanager;
    public GameObject PowerUpOrb;


    public bool ProlougeMode;
    public LevelManager levelmanager;

    public bool IsCompleted;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (rectTransform == null || canvas == null || canvasGroup == null)
        {
            Debug.LogError($"Missing required component on {gameObject.name}");
            enabled = false;
            return;
        }

        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        InitializePopUp(sharedPopUpImage);
        InitializePopUp(finalPopUpImage1);
        InitializePopUp(finalPopUpImage2);

        totalPieces++;
        Debug.Log($"Total pieces: {totalPieces}, GameObject: {gameObject.name}");
    }

    private void InitializePopUp(Image image)
    {
        if (image != null)
        {
            image.gameObject.SetActive(false);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            if (image.rectTransform != null)
            {
                image.rectTransform.localScale = (image == finalPopUpImage2) ? new Vector3(0.59f, 0.59f, 0.59f) : Vector3.one;
            }
        }
        if (image == sharedPopUpImage && sharedPopUpText != null)
        {
            sharedPopUpText.text = "";
            sharedPopUpText.alpha = 0f;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsCorrect && lockWhenCorrect) return;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsCorrect && lockWhenCorrect) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);

        if (IsCorrect && lockWhenCorrect) return;

        if (targetTransform != null && ShouldSnap())
        {
            SnapToTarget();
            if (sharedPopUpImage != null && !isShowingFinalPopUp)
            {
                PuzzlePiece[] allPieces = FindObjectsOfType<PuzzlePiece>();
                foreach (var piece in allPieces)
                {
                    if (piece != this && piece.currentPopUpCoroutine != null)
                    {
                        piece.StopCoroutine(piece.currentPopUpCoroutine);
                        piece.InitializePopUp(piece.sharedPopUpImage);
                    }
                }
                if (currentPopUpCoroutine != null) StopCoroutine(currentPopUpCoroutine);
                currentPopUpCoroutine = StartCoroutine(ShowPopUp(sharedPopUpImage, popUpDuration));
            }
            CheckAllPiecesCorrect();
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    private bool ShouldSnap()
    {
        Vector2 pieceCenter = rectTransform.TransformPoint(rectTransform.rect.center);
        Vector2 targetCenter = targetTransform.TransformPoint(targetTransform.rect.center);
        return Vector2.Distance(pieceCenter, targetCenter) <= snapDistance;
    }

    private void SnapToTarget()
    {
        Vector2 piecePivotOffset = rectTransform.rect.size * (rectTransform.pivot - Vector2.one * 0.5f);
        Vector2 targetPivotOffset = targetTransform.rect.size * (targetTransform.pivot - Vector2.one * 0.5f);

        Vector2 snapPosition = (Vector2)targetTransform.position + (targetPivotOffset - piecePivotOffset);
        rectTransform.position = snapPosition;

        if (matchRotation) rectTransform.rotation = targetTransform.rotation;
        rectTransform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

        if (!IsCorrect)
        {
            IsCorrect = true;
            correctPiecesCount++;
            Debug.Log($"Piece snapped! Correct pieces: {correctPiecesCount}/{totalPieces}");
            if (snapSound != null) snapSound.Play();
        }
        canvasGroup.blocksRaycasts = false;
    }

    private void CheckAllPiecesCorrect()
    {
        Debug.Log($"Checking puzzle completion: {correctPiecesCount}/{totalPieces}, isShowingFinalPopUp: {isShowingFinalPopUp}");
        if (correctPiecesCount == totalPieces && !isShowingFinalPopUp)
        {
            if (currentPopUpCoroutine != null)
            {
                Debug.Log("Waiting for last pop-up to finish");
                StartCoroutine(ShowFinalAfterLastPopUp());
            }
            else
            {
                Debug.Log("Showing final pop-ups directly");
                ShowFinalPopUps();
            }
        }
    }

    private IEnumerator ShowFinalAfterLastPopUp()
    {
        yield return currentPopUpCoroutine;
        ShowFinalPopUps();
    }

    private void ShowFinalPopUps()
    {
        Debug.Log("Attempting to show final pop-ups");
        if (finalPopUpImage1 == null || finalPopUpImage2 == null)
        {
            Debug.LogError("Final pop-up images not assigned in Inspector!");
            return;
        }

        PuzzlePiece[] allPieces = FindObjectsOfType<PuzzlePiece>();
        foreach (var piece in allPieces)
        {
            if (piece.currentPopUpCoroutine == null)
            {
                piece.InitializePopUp(piece.sharedPopUpImage);
            }
        }

        finalPopUpImage1.gameObject.SetActive(true);
        finalPopUpImage1.color = Color.white;
        Sequence squishSequence1 = DOTween.Sequence();
        squishSequence1.Append(finalPopUpImage1.rectTransform.DOScale(new Vector3(squishScale, squishScale, 1f), squishDuration * 0.5f))
                       .Append(finalPopUpImage1.rectTransform.DOScale(new Vector3(stretchScale, stretchScale, 1f), squishDuration * 0.25f))
                       .Append(finalPopUpImage1.rectTransform.DOScale(Vector3.one, squishDuration * 0.25f));

        finalPopUpImage2.gameObject.SetActive(true);
        finalPopUpImage2.color = Color.white;
        Sequence squishSequence2 = DOTween.Sequence();
        squishSequence2.Append(finalPopUpImage2.rectTransform.DOScale(new Vector3(squishScale, squishScale, 0.59f), squishDuration * 0.5f))
                       .Append(finalPopUpImage2.rectTransform.DOScale(new Vector3(stretchScale, stretchScale, 0.59f), squishDuration * 0.25f))
                       .Append(finalPopUpImage2.rectTransform.DOScale(new Vector3(0.59f, 0.59f, 0.59f), squishDuration * 0.25f));

        if (PiecesPanel != null)
        {
            PiecesPanel.SetActive(false);
            Debug.Log("PiecesPanel hidden");
        }

        if (completionSound != null)
        {
            completionSound.Play();
            Debug.Log("Completion sound played");
        }
        isShowingFinalPopUp = true;
        Debug.Log("Final pop-ups displayed, isShowingFinalPopUp set to true");



        //Add Score

        gamemanager.Addscore(80);

        //Add PowerUp
        PowerUpOrb.SetActive(true);



        //Complete
        IsCompleted = true;
    }

    private IEnumerator ShowPopUp(Image image, float duration)
    {
        if (image == null || sharedPopUpText == null) yield break;

        image.gameObject.SetActive(true);
        sharedPopUpText.text = pieceInfoText;
        sharedPopUpText.alpha = 0f;

        Sequence popUpSequence = DOTween.Sequence();
        popUpSequence.Append(image.rectTransform.DOScale(new Vector3(squishScale, squishScale, image == finalPopUpImage2 ? 0.59f : 1f), squishDuration * 0.5f))
                     .Append(image.rectTransform.DOScale(new Vector3(stretchScale, stretchScale, image == finalPopUpImage2 ? 0.59f : 1f), squishDuration * 0.25f))
                     .Append(image.rectTransform.DOScale(image == finalPopUpImage2 ? new Vector3(0.59f, 0.59f, 0.59f) : Vector3.one, squishDuration * 0.25f))
                     .Join(image.DOFade(1f, fadeDuration))
                     .Join(sharedPopUpText.DOFade(1f, fadeDuration));

        yield return popUpSequence.WaitForCompletion();
        yield return new WaitForSeconds(duration);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            sharedPopUpText.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        sharedPopUpText.alpha = 0f;
        image.gameObject.SetActive(false);
        sharedPopUpText.text = "";
    }

    public void ReturnToOriginalPosition()
    {
        if (!IsCorrect)
        {
            rectTransform.anchoredPosition = originalPosition;
            if (matchRotation) rectTransform.rotation = Quaternion.identity;
            rectTransform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        }
    }

    public void ResetPiece()
    {
        if (IsCorrect) correctPiecesCount--;
        IsCorrect = false;
        ReturnToOriginalPosition();
        canvasGroup.blocksRaycasts = true;
    }

    public static void ResetPuzzle()
    {
        Debug.Log("Resetting puzzle");
        correctPiecesCount = 0;
        totalPieces = 0;
        isShowingFinalPopUp = false;
    }

}