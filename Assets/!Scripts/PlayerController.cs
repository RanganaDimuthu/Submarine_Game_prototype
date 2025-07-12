using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 10f;
    

    [Header("Basic Controlls")]
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float deceleration = 5f;
    [SerializeField] float currentSpeed = 0f;


    [Header("Illution")]
    [SerializeField] GameObject MeshOfPlayer;
    [SerializeField] float MeshMoveSmoothSpeed = 10f;
    [SerializeField] float MeshOfPlayerTrunSpeed = 10f;


    [Header("Aniamtions")]
    [SerializeField] Transform Propeller;
    [SerializeField] float Defaultspeed = 5f, maxspedforProp = 15f, TargetSpeed = 5f, SmoothingOfAnimation = 1f;

    void Update()
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
        if (Input.GetMouseButton(0))
        {
            // Accelerate throttle towards maxSpeed
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);

            TargetSpeed = Mathf.Lerp(TargetSpeed, maxspedforProp, SmoothingOfAnimation * Time.deltaTime);
        }
        else
        {
            // Decelerate throttle towards 0
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            TargetSpeed = Mathf.Lerp(TargetSpeed, Defaultspeed, SmoothingOfAnimation * Time.deltaTime);
        }
        // Move object forward at current speed (assuming facing right direction)
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);



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



        //Propeller Illution animation
        if (Propeller != null)
        {
           Propeller.transform.Rotate(0f, 0f, TargetSpeed * Time.deltaTime);
        }















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
