using System;
using NUnit.Framework;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float initalMoveSpeed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private float runningSpeed = 1.5f; // Multiplier for running speed
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float runningDrag;



    [Header("Reference")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;




    private Rigidbody playerRigidbody;
    private StateController playerStateController;
    private float x;
    private float y;
    private Vector3 movementDirection;
    private Vector3 velocity;
    private bool isGrounded;
    private float movementSpeed;
    private bool isJumping;



    void Awake()
    {
        playerStateController = GetComponent<StateController>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.freezeRotation = true;
        movementSpeed = initalMoveSpeed;
    }

    void Update()
    {
        isGrounded = IsGrounded();

        setMovementDirection();

        setJumping();

        SetState();

        SetPlayerDrag();

        // LimitPlayerSpeed();

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        SetRunningSpeed();
    }

    private void setMovementDirection()
    {
        if (isGrounded && velocity.y <= 0)
        {
            velocity.y = -2f;
        }
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        movementDirection = transform.right * x + transform.forward * y;

        characterController.Move(movementDirection * movementSpeed * Time.deltaTime);
    }

    private void SetPlayerDrag()
    {
        if (IsRunning())
        {
            playerRigidbody.linearDamping = runningDrag;
        }
        else if (IsJumping())
        {
            playerRigidbody.linearDamping = airDrag;
        }
        else
        {
            playerRigidbody.linearDamping = groundDrag;
        }
    }

    // private void LimitPlayerSpeed()
    // {
    //     Vector3 flatVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.z);
    //     if( flatVelocity.magnitude > movementSpeed)
    //     {
    //         Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
    //         playerRigidbody.linearVelocity = new Vector3(limitedVelocity.x, playerRigidbody.linearVelocity.y, limitedVelocity.z);
    //     }
    // }

    private bool IsGrounded()
    {
        return isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }


    private void setJumping()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
        }
        
        if(isGrounded && velocity.y <= 0)
        {
            isJumping = false;
        }
    }

    private void SetRunningSpeed()
    {
        if (IsRunning())
        {
            movementSpeed = initalMoveSpeed * runningSpeed; // Increase speed for running
            //playerStateController.ChangeState(PlayerState.Running);
        }
        else
        {
            movementSpeed = initalMoveSpeed; // Reset speed to walking
            //playerStateController.ChangeState(PlayerState.Walking);
        }
    }

    private void SetState()
    {
        var moveDirection = GetMovementDirection();
        var isGrounded = IsGrounded();
        var currentState = playerStateController.GetCurrentState();
        var _isRunning = IsRunning();
        var _isJumping = IsJumping() && !isGrounded;

        var newState = currentState switch
        {
            _ when moveDirection == Vector3.zero && isGrounded && !_isRunning => PlayerState.Idle,
            _ when moveDirection != Vector3.zero && isGrounded && !_isRunning => PlayerState.Walking,
            _ when moveDirection != Vector3.zero && isGrounded && _isRunning => PlayerState.Running,
            _ when _isJumping => PlayerState.Jumping,
            _ => currentState
        };

        if (newState != currentState)
        {
            playerStateController.ChangeState(newState);
        }
        
        Debug.Log($"Current State: {newState}, Is Grounded: {isGrounded}, Is Running: {_isRunning}, Is Jumping: {_isJumping}");
    }

    private Vector3 GetMovementDirection()
    {
        return movementDirection.normalized;
    }


    private bool IsRunning()
    {
        return Input.GetKey(KeyCode.LeftShift) && movementDirection != Vector3.zero && isGrounded;
    }

    private bool IsJumping()
    {
        return isJumping;
    }
}

