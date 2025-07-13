
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(TriggerEvents))]
public class TriggerPro : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Finish the SetUp.");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Finish"))
        {
            TriggerEvents triggerEvents = (TriggerEvents)target;
            triggerEvents.FisshSetUp();
        }
        if (GUILayout.Button("Reset"))
        {
            TriggerEvents triggerEvents = (TriggerEvents)target;
            triggerEvents.Reset();
        }
        GUILayout.EndHorizontal();
    }
}
#endif

[RequireComponent(typeof(BoxCollider))]
[RequireComponent (typeof(Rigidbody))]
public class TriggerEvents : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool Triggerd = false;
    [SerializeField] UnityEvent WhenTrigger;
    [SerializeField] bool UseDelayFunction;
    [SerializeField] float DelayTime = 2f;
    [SerializeField] UnityEvent DelayFunction;
    [SerializeField] bool EnableReActiveWhenExit;
    [SerializeField] Color ColorDefaulf = Color.red;
    
    bool inSide = false;
    [SerializeField] string Tagname = "Player";

    [Header("KeyMapping")]
    [SerializeField] bool UseInterActions;
    [SerializeField] bool InteractionStats = false;

    [SerializeField] KeyCode KeyBord = KeyCode.E;
    [SerializeField] KeyCode GamePad = KeyCode.JoystickButton1;

    [Header("Other events")]
    [SerializeField] bool useStay;
    [SerializeField] UnityEvent Stay;
    [SerializeField] bool UseStayDelay;
    [SerializeField] float StayDelayTime = 2f;
    [SerializeField] UnityEvent InStayInteractionEnd;
    [SerializeField] bool useexit;
    [SerializeField] UnityEvent Exit;

    [Header("UI")]
    [SerializeField] Text ResultText;// Assign your UI Text in the Inspector
    [SerializeField] string massgae;
    [SerializeField] float fadeDuration = 1f; // Time for fading in/out
    [SerializeField] float waitTime = 2f; // Time to wait before fading out
    [SerializeField] bool afterInteractionActive; // activate after the Interaction



    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == Tagname && !Triggerd && !EnableReActiveWhenExit)
        {
            if (ResultText != null && !Triggerd && !afterInteractionActive) { StartCoroutine(FadeInAndOut()); ResultText.text = massgae; }
            WhenTrigger.Invoke();
            Triggerd = true; 
            if (UseDelayFunction)
            {
                Invoke("DelayedDunction", DelayTime);
            }
        }
        if (EnableReActiveWhenExit)
        {

            if (collider.gameObject.tag == Tagname)
            {
                WhenTrigger.Invoke();
                if (ResultText != null && !afterInteractionActive) { StartCoroutine(FadeInAndOut());  ResultText.text = massgae; }
                Triggerd = true;
                if (UseDelayFunction)
                {
                    Invoke("DelayedDunction", DelayTime);
                }
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (EnableReActiveWhenExit)
        {
            if (other.gameObject.tag == Tagname)
            {
                if (ResultText != null) { ResultText.text = ""; }
                
                if (useexit)
                {

                    Exit.Invoke();
                    if (UseDelayFunction)
                    {
                        Invoke("DelayedDunction", DelayTime);
                    }
                }
                Triggerd = false;
                inSide = false;
            }
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (useStay)
        {
            if (other.gameObject.tag == Tagname)
            {
                if (UseInterActions)
                {
                    if(Input.GetKeyDown(KeyBord) || Input.GetKeyDown(GamePad))
                    {
                        inSide = true;
                        Stay.Invoke();

                        if (UseStayDelay)
                        {
                            Invoke("StayDelayFunction", StayDelayTime);
                        }

                        if ((ResultText != null && afterInteractionActive) && !InteractionStats) { StartCoroutine(FadeInAndOut()); ResultText.text = massgae; InteractionStats = true; }
                    }
                    
                }
                if (!UseInterActions)
                {
                    inSide = true;
                    Stay.Invoke();
                }
                
            }
        }
    }

    public void DelayedDunction()
    {
        DelayFunction.Invoke();
    }


    public void StayDelayFunction()
    {
        InStayInteractionEnd.Invoke();
    }
    void OnDrawGizmos()
    { Vector3 Pos = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);
        Gizmos.DrawIcon(Pos, "Trigger_Pro.png");



        if (Triggerd == false)
        {
            Gizmos.color = ColorDefaulf;
        }
        if (Triggerd == true)
        {
            Gizmos.color = Color.green;
        }
        if (useStay && inSide)
        {
            Gizmos.color += Color.yellow;
        }
            
            
            Matrix4x4 originalMatrix = Gizmos.matrix;

            // Apply the GameObject's position and rotation
            Gizmos.matrix = Matrix4x4.TRS(Pos, transform.localRotation, Vector3.one);

            
            Gizmos.DrawWireCube(Vector3.zero, transform.localScale);

            // Restore the original Gizmos matrix
            Gizmos.matrix = originalMatrix;
        
        

    }


    

   


    public void FisshSetUp()
    {
        BoxCollider BC = this.gameObject.GetComponent<BoxCollider>();
        BC.isTrigger = true;
        Vector3 Pos = new Vector3(0, .5f, 0);
        BC.center = Pos;
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void Reset()
    {
        this.gameObject.transform.localScale = Vector3.one;
        FisshSetUp();
        Triggerd = false;
    }

    IEnumerator FadeInAndOut()
    {
        yield return StartCoroutine(FadeText(0f, 1f)); // Fade in
        yield return new WaitForSeconds(waitTime); // Wait before fading out
        yield return StartCoroutine(FadeText(1f, 0f)); // Fade out
    }

    IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = ResultText.color;
        color.a = startAlpha;
        ResultText.color = color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            ResultText.color = color;
            yield return null;
        }

        color.a = endAlpha;
        ResultText.color = color;
    }

}
