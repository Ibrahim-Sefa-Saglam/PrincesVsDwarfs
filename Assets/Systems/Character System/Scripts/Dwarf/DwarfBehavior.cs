using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DwarfBehavior : MonoBehaviour
{
    public GameObject gunObject;
    public SimpleAIBrain brain;
    public PointSight pointSight;
    
    private BaseGun gunScript;
    private Movement MovementScript;
    
    
    void Start()
    {
        MovementScript = GetComponent<Movement>();
        if (MovementScript == null) { Debug.LogError("Movement script is null"); return; }

        gunScript = gunObject.GetComponent<BaseGun>();
        if (gunScript == null) { Debug.LogError("Gun script is null"); return; }
        
    }

    // Update is called once per frame
    private void Update()
    {
        // KeyboardInputs();
        // MouseInputs();
        FeedBrainPercept(pointSight.GetRayInformation());
        
        
    }

    void KeyboardInputs()
    {
        if (Keyboard.current.aKey.isPressed)
        {
            MovementScript.MoveLeft();
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            MovementScript.MoveRight();
        }
        else
        {
            MovementScript.StopHorizontalMovement();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            MovementScript.Jump();
        }
    }

    void MouseInputs()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            gunScript.Fire();       
        }
    }

    void FeedBrainPercept(List<PointSight.RayInfo> rayInfos)
    {
        // Ensure the detection table is initialized
        brain.detectionTable ??= new List<SimpleAIBrain.RayDetectionData>();

        // Clear the previous detection data
        brain.detectionTable.Clear();

        // Convert each RayInfo to RayDetectionData and add to the brain's detectionTable
        foreach (var rayInfo in rayInfos)
        {
            SimpleAIBrain.RayDetectionData rayDetectionData = new SimpleAIBrain.RayDetectionData
            {
                rayLength = rayInfo.rayLength,
                rayAngle = rayInfo.rayAngle,
                hitPosition = rayInfo.hitPosition,
                hitObject = rayInfo.hitObject
            };

            // Add the converted data to the detectionTable
            brain.detectionTable.Add(rayDetectionData);
        }
    }
}