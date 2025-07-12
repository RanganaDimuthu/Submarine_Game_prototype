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

    [Header("Debug/State")]
    [SerializeField] private bool hasInteracted = false;  //  New interaction flag

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
        }
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

    private void R()
    {
        Result.Invoke();
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

    //Public getter if other scripts need to check
    public bool HasInteracted => hasInteracted;
}
