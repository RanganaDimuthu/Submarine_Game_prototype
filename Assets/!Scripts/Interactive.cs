using UnityEngine;
using UnityEngine.Events;

public class Interactive : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] UnityEvent Result;
    [SerializeField] float DelayTime = 5f;
   // [SerializeField] GameObject TargetCanvas;
    [SerializeField] bool DisableInputOnInteraction = true;
    [SerializeField] PlayerController PlayerController;

    [Header("Reusability")]
    [SerializeField] private bool canReUse = false; // Toggle this in Inspector
    [SerializeField] private float reuseCooldown = 1f; // optional cooldown (seconds)

    [Header("Debug/State")]
    [SerializeField] private bool hasInteracted = false;

    public GameObject ObjectHightlighticon , DotIcon;

    public PanelManager PanelManager;
    public bool PanelOpened;

    public bool PrimaryObjective,SecondaryObjective;
    public static bool Interacting;
    public bool CoralMode, CoralRestored;


    public LevelManager LevelManager;
    public bool interactedthisobject;

    private void Awake()
    {
        ObjectHightlighticon.SetActive(false);
        DotIcon.SetActive(true);
    }

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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (ObjectHightlighticon != null)
            {
                ObjectHightlighticon.SetActive(true);
            }
            if (DotIcon != null)
            {
                DotIcon.SetActive(false);
            }
        }

        if (other.gameObject.CompareTag("Sonar"))
        {
            CallResult();
            if (!PanelOpened && PrimaryObjective && !SecondaryObjective)
            {
                if(LevelManager.InteractedObjects == LevelManager.ObjectstoInteract ){
                    PanelManager.OpenPanel();
                    PanelOpened = true;
                    Interacting = true;
                }
                else
                {
                    LevelManager.ShowInfoTextPrimaryObj();
                }

            }
            else if(SecondaryObjective && !PanelOpened)
            {
                if (CoralMode)
                {
                    
                    if (!CoralRestored)
                    {
                        Interacting = true;
                    }
                }
                else
                {
                    if (!interactedthisobject)
                    {
                        LevelManager.ANewObjectInteracted();
                        interactedthisobject = true;
                    }
                   
                    Interacting = true;
                    
                }

               
            }
            
        }



    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (ObjectHightlighticon != null)
            {
                ObjectHightlighticon.SetActive(false);
            }
            if (DotIcon != null)
            {
                DotIcon.SetActive(true);
            }
        }
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


    public void CoralRestoredCall()
    {
        CoralRestored = true;
    }
}
