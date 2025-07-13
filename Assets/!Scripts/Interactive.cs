using UnityEngine;
using UnityEngine.Events;

public class Interactive : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] UnityEvent Result;
    [SerializeField] float DelayTime = 5f;
    [SerializeField] GameObject TargetCanvas;
    [SerializeField] bool DisableInputOnInteraction = true;
    [SerializeField] PlayerController PlayerController;

    [Header("Reusability")]
    [SerializeField] private bool canReUse = false; // 👈 Toggle this in Inspector
    [SerializeField] private float reuseCooldown = 1f; // optional cooldown (seconds)

    [Header("Debug/State")]
    [SerializeField] private bool hasInteracted = false;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerController = player.GetComponent<PlayerController>();
    }

    public void CallResult()
    {
        if (!hasInteracted)
        {
            hasInteracted = true;
            Invoke(nameof(R), DelayTime);

            if (canReUse)
            {
                // Reset interaction after delay + cooldown
                Invoke(nameof(ResetInteraction), DelayTime + reuseCooldown);
            }
        }
    }

    private void R()
    {
        Result.Invoke();
    }

    private void ResetInteraction()
    {
        hasInteracted = false;
    }

    private void Update()
    {
        if (DisableInputOnInteraction && IsPlayerNearby())
        {
            if (TargetCanvas.activeInHierarchy)
            {
                PlayerController.SetUpInputOfPlayer(false);
            }
            else
            {
                PlayerController.SetUpInputOfPlayer(true);
            }
        }
    }

    private bool IsPlayerNearby()
    {
        float checkRadius = 3f;
        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                return true;
        }

        return false;
    }

    public void debug(string Log)
    {
        Debug.Log(Log);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }

    public bool HasInteracted => hasInteracted;
}
