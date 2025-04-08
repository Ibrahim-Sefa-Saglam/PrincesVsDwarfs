using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour
{
    private Movement MovementScript;
    // Start is called before the first frame update
    void Start()
    {
        MovementScript = GetComponent<Movement>();   
    }

    // Update is called once per frame
    private void Update()
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

}
