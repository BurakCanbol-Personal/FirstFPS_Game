using System;
using UnityEngine;
using UnityEngine.XR;

[DefaultExecutionOrder(-1)]
public class PlayerContoller2 : MonoBehaviour
{
    #region Class Variables
    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;



    [Header("Movement Settings")]
    [SerializeField] private float runAcceleration = 0.25f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float sprintAcceleration = 0.5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float drag = 0.1f;
    [SerializeField] private float jumpSpeed = 1.0f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float movingThreshold = 0.01f; // Threshold to consider movement input significant
    [SerializeField] private float _verticalVelocity = 0f; // Vertical velocity for jumping and falling




    [Header("Look Settings")]
    [SerializeField] private float lookSensitivityH = 0.1f;
    [SerializeField] private float lookSensitivityV = 0.1f;
    [SerializeField] private float lookLimitV = 80f; // Limit vertical look angle




    private PlayerLocomotionInput _playerLocomotionInput;
    private PlayerStates _playerStates; // Reference to PlayerStates for managing movement state

    private Vector2 _cameraRotation = Vector2.zero; // Store camera rotation to apply look input
    private Vector2 _playerTargerRotation = Vector2.zero; // Store player target rotation to apply look input

    #endregion



    #region StartUp
    private void Awake()
    {
        _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        _playerStates = GetComponent<PlayerStates>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion



    #region Update Logic
    private void Update()
    {
        UpdateMovementState();
        HandleVerticalMovement();
        HandleLateralMovement();
    }

    private void UpdateMovementState()
    {
        // Handle any physics-related updates here if needed
        bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;
        bool isGrounded = IsGrounded();

        PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting :
                                            isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

        _playerStates.SetPlayerMovementState(lateralState);

        // Handle jumping and falling states
        if (!isGrounded && _characterController.velocity.y > 0f)
        {
            _playerStates.SetPlayerMovementState(PlayerMovementState.Jumping);
        }
        else if (!isGrounded && _characterController.velocity.y <= 0f)
        {
            _playerStates.SetPlayerMovementState(PlayerMovementState.Falling);
        }
    }

    private void HandleVerticalMovement()
    {
        bool isGrounded = _playerStates.InGroundedState();

        if (isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = 0f;
        }

        _verticalVelocity += gravity * Time.deltaTime; // Apply gravity when grounded
        
        if (_playerLocomotionInput.JumpPressed && isGrounded)
        {
            _verticalVelocity += Mathf.Sqrt(jumpSpeed * -3.0f * gravity);
        }
        
    }


    private void HandleLateralMovement()
    {
        bool isSprinting = _playerStates.CurrentMovementState == PlayerMovementState.Sprinting;
        bool isGrounded = _playerStates.InGroundedState();

        // state dependent movement parameters
        float lateralAcceleration = isSprinting ? sprintAcceleration : runAcceleration;
        float clampLateralMagnitude = isSprinting ? sprintSpeed : runSpeed;

        Vector3 cameraFowardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
        Vector3 movementDirection = (cameraFowardXZ * _playerLocomotionInput.MovementInput.y + cameraRightXZ * _playerLocomotionInput.MovementInput.x).normalized;

        Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
        Vector3 newVelocity = _characterController.velocity + movementDelta;


        //Apply drag to the velocity
        Vector3 dragVector = newVelocity.normalized * drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - dragVector : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralMagnitude);
        newVelocity.y += _verticalVelocity; // Add vertical velocity to the new velocity

        //Move character (Unity suggests only calling this once per frame)
        _characterController.Move(newVelocity * Time.deltaTime);
    }

    #endregion

    #region LateUpdate Logic

    private void LateUpdate()
    {
        _cameraRotation.x += _playerLocomotionInput.LookInput.x * lookSensitivityH;
        // _cameraRotation.y -= _playerLocomotionInput.LookInput.y * lookSensitivityV;
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSensitivityV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

        _playerTargerRotation.x += transform.eulerAngles.x + lookSensitivityH * _playerLocomotionInput.LookInput.x;
        transform.rotation = Quaternion.Euler(0f, _playerTargerRotation.x, 0f);

        _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);
    }

    #endregion

    #region State Check
    private bool IsMovingLaterally()
    {
        Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.y);
        return lateralVelocity.magnitude > movingThreshold; // Adjust the threshold as needed
    }

    private bool IsGrounded()
    {
        return _characterController.isGrounded;
        
    }

    #endregion
}
