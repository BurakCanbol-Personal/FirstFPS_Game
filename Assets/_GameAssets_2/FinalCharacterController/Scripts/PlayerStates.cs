using UnityEngine;

public class PlayerStates : MonoBehaviour
{

    [field: SerializeField] public PlayerMovementState CurrentMovementState { get; private set; } = PlayerMovementState.Idling;

    public void SetPlayerMovementState(PlayerMovementState newState)
    {
        CurrentMovementState = newState;
        // Additional logic can be added here if needed when the state changes
    }

    public bool InGroundedState()
    {
        return IsStateGroundedState(CurrentMovementState);
    }

    public bool IsStateGroundedState(PlayerMovementState movementState)
    {
        return movementState == PlayerMovementState.Idling ||
               movementState == PlayerMovementState.Walking ||
               movementState == PlayerMovementState.Running ||
               movementState == PlayerMovementState.Sprinting;
    }
}

public enum PlayerMovementState
    {
        Idling = 0,
        Walking = 1,
        Running = 2,
        Sprinting = 3,
        Jumping = 4,
        Falling = 5,
        Strafing = 6

    }
