using System;
using UnityEngine;

public class InputHnadler : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();

        playerInput.Player.Jump.performed += playerJump; 
    }

    private void playerJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Debug.Log(context);

        if (context.ReadValueAsButton())
        {
            PlayerControllerTest.Instance.HandleJump();
        }
    }

    public Vector2 GetMovementInput()
    {
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();
        
        inputVector = inputVector.normalized; // Normalize the input vector to prevent faster diagonal movement
        return inputVector;
    }
}
