using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Transform Target;
    [SerializeField] float CamerafollowSpeed = 5f;


    private void Update()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = Target.transform.position;

        // Keep Z unchanged
        Vector3 desiredPos = new Vector3(targetPos.x, targetPos.y, currentPos.z);

        transform.position = Vector3.Lerp(currentPos, desiredPos, Time.deltaTime * CamerafollowSpeed);


    }


    public void changeTarget(Transform transform)
    {
        Target = transform;
    }
    public void setspeedcam(float speed)
    {
        CamerafollowSpeed  = speed;
    }
}
