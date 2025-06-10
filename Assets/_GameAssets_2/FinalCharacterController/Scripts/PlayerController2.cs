using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

[DefaultExecutionOrder(-1)]
public class PlayerContoller2 : MonoBehaviour
{
    #region Class Variables
    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;

    public float RotationMismatch { get; set; } = 0.0f; // Used to adjust the rotation mismatch between player and camera
    public bool isRotationgToTarget { get; set; } = false; // Flag to indicate if the player is rotating to a target direction




    [Header("Movement Settings")]
    [Header("Walk")]
    [SerializeField] private float walkAcceleration = 25f;
    [SerializeField] private float walkSpeed = 2f;

    [Header("Run")]
    [SerializeField] private float runAcceleration = 35f;
    [SerializeField] private float runSpeed = 4f;

    [Header("Sprint")]
    [SerializeField] private float sprintAcceleration = 50f;
    [SerializeField] private float sprintSpeed = 7f;

    [Header("In Air")]
    [SerializeField] private float inAirAceleration = 25f;
    [SerializeField] private float terminalVelocity = 50f; // Maximum speed in the air, used to limit the horizontal velocity when airborne
    [SerializeField] private float inAirDrag = 5f;

    [Header("Rest")]
    [SerializeField] private float drag = 20f;
    [SerializeField] private float jumpSpeed = 1.0f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float movingThreshold = 0.01f; // Threshold to consider movement input significant
    [SerializeField] private float _verticalVelocity = 0f; // Vertical velocity for jumping and falling



    // [Header("Animation Settings")]
    // [SerializeField] private float playerModelRotationSpeed;
    // [SerializeField] private float rotationToTargetTime = 0.25f; // Time to rotate to target direction


    [Header("Environment Settings")]
    [SerializeField] private LayerMask _groundLayers; // Layer mask for ground detection




    [Header("Look Settings")]
    [SerializeField] private float lookSensitivityH = 0.1f;
    [SerializeField] private float lookSensitivityV = 0.1f;
    [SerializeField] private float lookLimitV = 80f; // Limit vertical look angle




    private PlayerLocomotionInput _playerLocomotionInput;
    private PlayerStates _playerStates; // Reference to PlayerStates for managing movement state
    private PlayerMovementState _lastMovementState = PlayerMovementState.Falling;



    private Vector2 _cameraRotation = Vector2.zero; // Store camera rotation to apply look input
    private Vector2 _playerTargerRotation = Vector2.zero; // Store player target rotation to apply look input

    private float _antiBump;
    private float _stepOffset;
    private bool _jumpedLastFrame = false;

    // private Vector3 _horizontalVelocity = Vector3.zero;
    // private float _rotatingToTargetTimer = 0f; // Timer for rotation to target direction


    #endregion



    #region StartUp
    private void Awake()
    {
        _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        _playerStates = GetComponent<PlayerStates>();

        _antiBump = sprintSpeed;
        _stepOffset = _characterController.stepOffset;
        
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _verticalVelocity = 0f;
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

        _lastMovementState = _playerStates.CurrentMovementState;
        // Handle any physics-related updates here if needed
        bool canRun = CanRun();
        bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;
        bool isWalking = isMovingLaterally && (!canRun || _playerLocomotionInput.WalkToggleOn);
        bool isGrounded = IsGrounded();

        PlayerMovementState lateralState = isWalking ? PlayerMovementState.Walking :
                                            isSprinting ? PlayerMovementState.Sprinting :
                                            isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

        _playerStates.SetPlayerMovementState(lateralState);

        // Handle jumping and falling states
        if ((!isGrounded || _jumpedLastFrame) && _characterController.velocity.y > 0f)
        {
            _playerStates.SetPlayerMovementState(PlayerMovementState.Jumping);
            _jumpedLastFrame = false;
            _characterController.stepOffset = 0f;
        }
        else if ((!isGrounded || _jumpedLastFrame) && _characterController.velocity.y <= 0f)
        {
            _playerStates.SetPlayerMovementState(PlayerMovementState.Falling);
            _jumpedLastFrame = false;
            _characterController.stepOffset = 0f;
        }
        else
        {
            _characterController.stepOffset = _stepOffset;
        }
    }

    private void HandleVerticalMovement()
    {
        bool isGrounded = _playerStates.InGroundedState();

        _verticalVelocity -= gravity * Time.deltaTime;

        if (isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -_antiBump;
        }


        if (_playerLocomotionInput.JumpPressed && isGrounded)
        {
            _verticalVelocity += Mathf.Sqrt(jumpSpeed * 3.0f * gravity);
            _jumpedLastFrame = true;
        }

        if (_playerStates.IsStateGroundedState(_lastMovementState) && !isGrounded)
        {
            _verticalVelocity += _antiBump; // Reset vertical velocity when transitioning from grounded to airborne
        }
        
        if(Mathf.Abs(_verticalVelocity) > Mathf.Abs(terminalVelocity))
        {
            _verticalVelocity = -1f * Mathf.Abs(terminalVelocity); // Limit vertical velocity to terminal velocity
        }

    }


    private void HandleLateralMovement()
    {
        bool isSprinting = _playerStates.CurrentMovementState == PlayerMovementState.Sprinting;
        bool isGrounded = _playerStates.InGroundedState();
        bool isWalking = _playerStates.CurrentMovementState == PlayerMovementState.Walking;

        // state dependent movement parameters
        float lateralAcceleration = isGrounded ? inAirAceleration :
                                    isWalking ? walkAcceleration :
                                    isSprinting ? sprintAcceleration : runAcceleration;

        float clampLateralMagnitude = isGrounded ? sprintSpeed :
                                       isWalking ? walkSpeed :
                                       isSprinting ? sprintSpeed : runSpeed;

        Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
        Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
        Vector3 newVelocity = _characterController.velocity + movementDelta;

        // Add drag to player
        float dragMagnitude = isGrounded ? drag : inAirDrag;
        Vector3 currentDrag = newVelocity.normalized * dragMagnitude * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > dragMagnitude * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), clampLateralMagnitude);
        newVelocity.y += _verticalVelocity;
        newVelocity = !isGrounded ? HandleSteepWalls(newVelocity) : newVelocity;

        // Move character (Unity suggests only calling this once per tick)
        _characterController.Move(newVelocity * Time.deltaTime);
    }



    private Vector3 HandleSteepWalls(Vector3 velocity)
    {
        Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
        float angle = Vector3.Angle(normal, Vector3.up);
        bool validAngle = angle <= _characterController.slopeLimit;

        if (!validAngle && _verticalVelocity < 0f)
            velocity = Vector3.ProjectOnPlane(velocity, normal);

        return velocity;
    }


    #endregion

    #region LateUpdate Logic

    private void LateUpdate()
    {
        UpdateCameraRotation();
    }

    private void UpdateCameraRotation()
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
        Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);
        return lateralVelocity.magnitude > movingThreshold; // Adjust the threshold as needed
    }

    private bool IsGrounded()
    {
        bool grounded = _playerStates.InGroundedState() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborne();
        return grounded;

    }

    private bool IsGroundedWhileGrounded()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _characterController.radius, transform.position.z);

        bool grounded = Physics.CheckSphere(spherePosition, _characterController.radius, _groundLayers, QueryTriggerInteraction.Ignore);

        return grounded;
    }

    private bool IsGroundedWhileAirborne()
    {
        Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
        float angle = Vector3.Angle(normal, Vector3.up);
        bool validAngle = angle <= _characterController.slopeLimit;

        return _characterController.isGrounded && validAngle;
    }

    private bool CanRun()
    {
        return _playerLocomotionInput.MovementInput.y >= Mathf.Abs(_playerLocomotionInput.MovementInput.x);
    }

    #endregion
}
