using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float locomotionBlendSpeed = 0.2f;


    private PlayerLocomotionInput _playerLocomotionInput;
    private PlayerStates _platerStates;

    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");
    private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
    private static int isGroundedHash = Animator.StringToHash("isGrounded");
    private static int isFallingHash = Animator.StringToHash("isFalling");
    private static int isJumpingHash = Animator.StringToHash("isJumping");

    private Vector3 _currentBlendInput = Vector3.zero;

    private void Awake()
    {
        _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        _platerStates = GetComponent<PlayerStates>();
    }

    private void Update()
    {
        UpdateAnimatorParameters();
    }

    private void UpdateAnimatorParameters()
    {
        bool isIdling = _platerStates.CurrentMovementState == PlayerMovementState.Idling;
        bool isRunning = _platerStates.CurrentMovementState == PlayerMovementState.Running;
        bool isSprinting = _platerStates.CurrentMovementState == PlayerMovementState.Sprinting;
        bool isJumping = _platerStates.CurrentMovementState == PlayerMovementState.Jumping;
        bool isFalling = _platerStates.CurrentMovementState == PlayerMovementState.Falling;
        bool isGrounded = _platerStates.InGroundedState();


        Vector2 inputTarget = isSprinting ? _playerLocomotionInput.MovementInput * 1.5f :
                                isRunning ? _playerLocomotionInput.MovementInput * 1f :
                                _playerLocomotionInput.MovementInput * 0.5f;
                                
        _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);


        _animator.SetBool(isGroundedHash, isGrounded);
        _animator.SetBool(isFallingHash, isFalling);
        _animator.SetBool(isJumpingHash, isJumping);

        _animator.SetFloat(inputXHash, _currentBlendInput.x);
        _animator.SetFloat(inputYHash, _currentBlendInput.y);
        _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
    }
}
