using UnityEngine;

public class IslandClickDetector : MonoBehaviour
{
    [SerializeField] private ESGIslandController ESGIslandController;
    [SerializeField] private GameObject outlineObject; // Assign the outline child here in Inspector

    private void Start()
    {
        if (outlineObject != null)
            outlineObject.SetActive(false); // make sure it's hidden at start
    }

    void OnMouseEnter()
    {
        if (!ESGIslandController.isCompleted && Interactive.Interacting)
        {
            if (outlineObject != null)
                outlineObject.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        if (outlineObject != null)
            outlineObject.SetActive(false);
    }

    void OnMouseDown()
    {
        if (Interactive.Interacting)
        {
            ESGIslandController.OnIslandClicked();
        }
        
    }
}
