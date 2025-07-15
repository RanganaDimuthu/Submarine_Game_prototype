using System.Collections.Generic;
using UnityEngine;

public class sonar : MonoBehaviour
{
    public float detectionRadius = 5f;
    public LayerMask detectionLayers;
    bool around_interactive = false;
    public Interactive currentinteractive;
    public bool Holding = false;
    private HashSet<Collider> previousHits = new HashSet<Collider>();



    void Update()
    {
        Collider[] currentHits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayers);
        HashSet<Collider> currentSet = new HashSet<Collider>(currentHits);

        // Handle Enter
        /*foreach (Collider col in currentSet)
        {
            if (!previousHits.Contains(col))
            {
                Debug.Log("ENTER: " + col.name);
            }
        }*/

        
        // Handle Stay
        foreach (Collider col in currentSet)
        {
            if (previousHits.Contains(col))
            {
                currentinteractive = col.GetComponent<Interactive>();
               
                    around_interactive = true;
            }
        }

        // Handle Exit
        foreach (Collider col in previousHits)
        {
            if (!currentSet.Contains(col))
            {
               
                around_interactive = false;
                currentinteractive = null;
            }
        }

        // Update previousHits for next frame
        previousHits = currentSet;
    }



    public void Interaction()
    {
        if (around_interactive)
        {
            if (currentinteractive == null) return;
            currentinteractive.CallResult();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
