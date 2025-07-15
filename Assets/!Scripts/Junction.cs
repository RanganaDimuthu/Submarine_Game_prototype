using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Junction : MonoBehaviour
{
    private Button junctionButton;
    private Image buttonImage;
    public bool hasLiquid { get; set; } = false; // Boolean to track if liquid is flowing through the junction
    public MonoBehaviour leftObject; // Reference to the left object (Pipe or Junction)
    [SerializeField] private List<MonoBehaviour> rightObjects = new List<MonoBehaviour>(); // References to right objects (Pipes or Junctions)
    [SerializeField] private List<float> connectionAngles = new List<float> { 0f, 180f }; // Angles where objects are connected
    private ILiquidHolder leftLiquidHolder; // Cached interface for left object
    private List<ILiquidHolder> rightLiquidHolders = new List<ILiquidHolder>(); // Cached interfaces for right objects

    void Start()
    {
        // Get the Button and Image components
        junctionButton = GetComponent<Button>();
        if (junctionButton == null)
        {
            Debug.LogError($"Junction {name} is missing Button component!");
        }
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError($"Junction {name} is missing Image component!");
        }

        // Cache ILiquidHolder interface for leftObject
        if (leftObject != null)
        {
            leftLiquidHolder = leftObject.GetComponent<ILiquidHolder>();
            if (leftLiquidHolder == null)
            {
                Debug.LogWarning($"Junction {name}: leftObject {leftObject.name} does not implement ILiquidHolder!");
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
                    Debug.LogWarning($"Junction {name}: rightObject {rightObj.name} does not implement ILiquidHolder!");
                }
            }
        }

        // Add a listener to the button's onClick event
        junctionButton.onClick.AddListener(OnJunctionButtonClicked);

        // Initial liquid flow update
        UpdateLiquidFlow();
        UpdateButtonColor();
    }

    void Update()
    {
        // Continuously update liquid flow to reflect changes in leftObject
        UpdateLiquidFlow();
        UpdateButtonColor();
    }

    void OnJunctionButtonClicked()
    {
        // Rotate the button 90 degrees around the Z-axis
        transform.Rotate(0, 0, 90);

        // Update liquid flow based on rotation
        UpdateLiquidFlow();
        UpdateButtonColor();
    }

    void UpdateButtonColor()
    {
        // Set junction color to green only if liquid is flowing through it
        buttonImage.color = hasLiquid ? Color.green : Color.white;
    }

    void UpdateLiquidFlow()
    {
        // Normalize rotation to 0-360 degrees
        float zRotation = transform.eulerAngles.z % 360;
        if (zRotation < 0) zRotation += 360;

        // Check if current rotation is in connectionAngles (within a small tolerance)
        bool isConnected = false;
        foreach (float angle in connectionAngles)
        {
            if (Mathf.Abs(zRotation - angle) < 0.1f)
            {
                isConnected = true;
                break;
            }
        }

        if (leftLiquidHolder != null)
        {
            if (isConnected)
            {
                // If connected, propagate liquid from left object
                hasLiquid = leftLiquidHolder.hasLiquid;
                foreach (var rightHolder in rightLiquidHolders)
                {
                    rightHolder.hasLiquid = hasLiquid;
                }
            }
            else
            {
                // If disconnected, no liquid flows through junction
                hasLiquid = false;
                // Right objects depend on their own leftObject or isLiquidSource
                for (int i = 0; i < rightLiquidHolders.Count; i++)
                {
                    var rightHolder = rightLiquidHolders[i];
                    var rightObj = rightObjects[i];
                    if (rightHolder != null && rightObj.GetComponent<Pipe>()?.isLiquidSource != true)
                    {
                        rightHolder.hasLiquid = false;
                    }
                }
            }
        }
        else
        {
            // No left object, no liquid flow
            hasLiquid = false;
            for (int i = 0; i < rightLiquidHolders.Count; i++)
            {
                var rightHolder = rightLiquidHolders[i];
                var rightObj = rightObjects[i];
                if (rightHolder != null && rightObj.GetComponent<Pipe>()?.isLiquidSource != true)
                {
                    rightHolder.hasLiquid = false;
                }
            }
        }
    }
}
