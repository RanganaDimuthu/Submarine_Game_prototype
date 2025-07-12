using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 10f;

    [Header("Basic Controlls")]
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float deceleration = 5f;
    [SerializeField] float currentSpeed = 0f;
    [SerializeField] float Z_Rotate = 5f;
    [SerializeField] GameObject SonarObj;


    [Header("Illution")]
    [SerializeField] GameObject MeshOfPlayer;
    [SerializeField] float MeshMoveSmoothSpeed = 10f;
    [SerializeField] float MeshOfPlayerTrunSpeed = 10f;


    [Header("Aniamtions")]
    [SerializeField] Transform Propeller;
    [SerializeField] float Defaultspeed = 5f, maxspedforProp = 15f, TargetSpeed = 5f, SmoothingOfAnimation = 1f;



    [Header("Effects")]
    [SerializeField] ParticleSystem sonar;
    [SerializeField] AudioSource SonarAudioSource;
    [SerializeField] AudioSource HoldingAudio;
    [SerializeField] sonar SonarScript;

    [Header("Raycast From Custom Transform")]
    [SerializeField] Transform raycastOrigin;         // Drag any object here in Inspector
    [SerializeField] float rayLength = 2f;
    [SerializeField] LayerMask collisionLayers;
    [SerializeField] bool showDebugRays = true;
    private bool isBlocked = false;
    bool anyBlocked;
    bool blockRight = false;
    bool blockLeft = false;
    bool blockUp = false;
    bool blockDown = false;

    [Header("Public Data")]
    public bool ActiveInputs = true;
    void Update()
    {

        if (ActiveInputs)
        {
            

            //Handle Rotation...
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector3 direction = mousePos - transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            // Smoothly rotate from current rotation to target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


            //Handle Throttle...
            if (Input.GetMouseButton(0) && !isBlocked) 
            {
                // Accelerate throttle towards maxSpeed
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
                TargetSpeed = Mathf.Lerp(TargetSpeed, maxspedforProp, SmoothingOfAnimation * Time.deltaTime);
            }
            else
            {
                // Decelerate or stop if blocked or not holding button
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
                TargetSpeed = Mathf.Lerp(TargetSpeed, Defaultspeed, SmoothingOfAnimation * Time.deltaTime);
            }
            // Move object forward at current speed (assuming facing right direction)
            if (!blockRight)
            {
                transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
            }

        }

       


        //Illution...

        //position...
        MeshOfPlayer.transform.position = Vector3.Lerp(
            MeshOfPlayer.transform.position,
                transform.position,
                    Time.deltaTime * MeshMoveSmoothSpeed // <- smooth speed (adjust as needed)
                            );


        //rotation
        // Get mouse screen position
        Vector3 mouseScreenPos = Input.mousePosition;

        // Get MeshOfPlayer's screen position
        Vector3 meshScreenPos = Camera.main.WorldToScreenPoint(MeshOfPlayer.transform.position);

        // Compare X positions (left/right)
        bool isMouseOnLeft = mouseScreenPos.x < meshScreenPos.x;

        // Target Y rotation
        float targetYRotation = isMouseOnLeft ? 180f : 0f;

        // Get current rotation
        Vector3 currentRotation = MeshOfPlayer.transform.eulerAngles;

        // Smoothly rotate only Y axis
        float smoothedY = Mathf.LerpAngle(currentRotation.y, targetYRotation, Time.deltaTime * MeshOfPlayerTrunSpeed);
        MeshOfPlayer.transform.eulerAngles = new Vector3(currentRotation.x, smoothedY, currentRotation.z);

        

        

        // Compare Y positions (up/down)
        bool isMouseAbove = mouseScreenPos.y > meshScreenPos.y;

        // Target Z rotation
        float targetZRotation = isMouseAbove ? Z_Rotate : -Z_Rotate; // adjust values as needed

        // Get current rotation
        Vector3 currentRotationR = MeshOfPlayer.transform.eulerAngles;

        // Smoothly rotate only Z axis
        float smoothedZ = Mathf.LerpAngle(currentRotationR.z, targetZRotation, Time.deltaTime * MeshOfPlayerTrunSpeed);

        // Apply back to transform
        MeshOfPlayer.transform.eulerAngles = new Vector3(currentRotationR.x, currentRotationR.y, smoothedZ);


        //Propeller Illution animation
        if (Propeller != null)
        {
           Propeller.transform.Rotate(0f, 0f, TargetSpeed * Time.deltaTime);
        }

        //Right Mouse Click - Activates the sonar
        if (Input.GetMouseButtonDown(1))
        {
            sonar.Play();
            SonarAudioSource.Play();
            if (!SonarScript.Holding)
            {
                SonarScript.Interaction();
            }
            if (SonarScript.Holding && SonarScript.currentinteractive != null)
            {
                HoldingAudio.Play();
            }
           
        }else if (Input.GetMouseButtonUp(1))
        {
            sonar.Stop();
            SonarAudioSource.Stop();
        }



        if (raycastOrigin != null)
        {
            Vector3 origin = raycastOrigin.position;
            Vector3 right = raycastOrigin.right;
            Vector3 left = -raycastOrigin.right;
            Vector3 up = raycastOrigin.up;
            Vector3 down = -raycastOrigin.up;

            // Perform raycasts
            blockRight = Physics.Raycast(origin, right, rayLength, collisionLayers);
            blockLeft = Physics.Raycast(origin, left, rayLength, collisionLayers);
            blockUp = Physics.Raycast(origin, up, rayLength, collisionLayers);
            blockDown = Physics.Raycast(origin, down, rayLength, collisionLayers);

            if (showDebugRays)
            {
                Debug.DrawRay(origin, right * rayLength, blockRight ? Color.red : Color.green);
                Debug.DrawRay(origin, left * rayLength, blockLeft ? Color.red : Color.green);
                Debug.DrawRay(origin, up * rayLength, blockUp ? Color.red : Color.green);
                Debug.DrawRay(origin, down * rayLength, blockDown ? Color.red : Color.green);
            }
        }

        
        anyBlocked = blockRight || blockLeft || blockUp || blockDown;

        if (anyBlocked)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                MeshOfPlayer.transform.position,
                Time.deltaTime * 15f
            );
        }



    }


    public void SetUpInputOfPlayer(bool state)
    {
        ActiveInputs = state;
        SonarObj.SetActive(state);
    }

    void OnDrawGizmos()
    {
        if (MeshOfPlayer != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(MeshOfPlayer.transform.position, transform.position);
        }
    }

}
