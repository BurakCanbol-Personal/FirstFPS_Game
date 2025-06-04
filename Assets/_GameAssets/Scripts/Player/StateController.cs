using System;
using UnityEngine;
using UnityEngine.XR;

public class StateController : MonoBehaviour
{
    private PlayerState currentPlayerState = PlayerState.Idle;

    public void ChangeState(PlayerState newState)
    {
        if (currentPlayerState == newState) { return; }

        currentPlayerState = newState;
    }

    public PlayerState GetCurrentState()
    {
        return currentPlayerState;
    }

    private void Start()
    {
        ChangeState(PlayerState.Idle);
    }
}
