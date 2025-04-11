using UnityEngine;
using UnityEngine.InputSystem;

public class DwarfBehavior : MonoBehaviour
{
    public GameObject gunObject;
    
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
        KeyboardInputs();
        MouseInputs();
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
}