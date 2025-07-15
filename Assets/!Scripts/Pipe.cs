using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Pipe : MonoBehaviour, ILiquidHolder
{
    private Image pipeImage;
    public bool hasLiquid { get; set; } = false; // Boolean to track if liquid is in the pipe
    public bool isLiquidSource = false; // Boolean to mark if this pipe is a primary liquid source
    public MonoBehaviour leftObject; // Reference to the left object (Pipe or Junction)
    [SerializeField] private List<MonoBehaviour> rightObjects = new List<MonoBehaviour>(); // References to right objects (Pipes or Junctions)
    private ILiquidHolder leftLiquidHolder; // Cached interface for left object
    private List<ILiquidHolder> rightLiquidHolders = new List<ILiquidHolder>(); // Cached interfaces for right objects

    void Start()
    {
        // Get the Image component
        pipeImage = GetComponent<Image>();
        if (pipeImage == null)
        {
            Debug.LogError($"Pipe {name} is missing Image component!");
        }

        // Cache the ILiquidHolder interface for leftObject
        if (leftObject != null)
        {
            leftLiquidHolder = leftObject.GetComponent<ILiquidHolder>();
            if (leftLiquidHolder == null)
            {
                Debug.LogWarning($"Pipe {name}: leftObject {leftObject.name} does not implement ILiquidHolder!");
            }
        }

        // Cache ILiquidHolder interfaces for rightObjects
        foreach (var rightObj in rightObjects)
        {
            if (rightObj != null)
            {
                var holder = rightObj.GetComponent<ILiquidHolder>();
                if (holder != null)
                {
                    rightLiquidHolders.Add(holder);
                }
                else
                {
                    Debug.LogWarning($"Pipe {name}: rightObject {rightObj.name} does not implement ILiquidHolder!");
                }
            }
        }

        // Update color based on initial hasLiquid state
        UpdatePipeColor();
    }

    void Update()
    {
        // Update hasLiquid based on leftObject or isLiquidSource
        if (!isLiquidSource && leftLiquidHolder != null)
        {
            hasLiquid = leftLiquidHolder.hasLiquid;
        }
        else if (isLiquidSource)
        {
            hasLiquid = true; // Primary source always has liquid
        }

        // Propagate liquid to all right objects
        foreach (var rightHolder in rightLiquidHolders)
        {
            rightHolder.hasLiquid = hasLiquid;
        }

        // Update color every frame to reflect changes in hasLiquid
        UpdatePipeColor();
    }

    void UpdatePipeColor()
    {
        // Set pipe color to green if hasLiquid is true, otherwise use default white
        pipeImage.color = hasLiquid ? Color.green : Color.white;
    }
}