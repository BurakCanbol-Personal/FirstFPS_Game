using UnityEngine;

public class PlayerControllerTest : MonoBehaviour
{
    public static PlayerControllerTest Instance { get; private set; }

    [Header("Reference")]
    [SerializeField] private InputHnadler inputHnadler;
    [SerializeField] private Transform cameraTransform;




    [Header("Movement Settings")]
    [SerializeField] private float MoveSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;



    private Rigidbody playerRigidbody;
    private bool isWalking;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerRigidbody = GetComponent<Rigidbody>();
        //playerRigidbody.freezeRotation = true;
    }


    private void Update()
    {
        HandleMovement();
        //HandleJump();
    }

    private void HandleMovement()
    {
        Vector2 input = inputHnadler.GetMovementInput();

        // Get the camera's forward and right directions, but ignore the vertical component
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Movement direction relative to camera
        Vector3 moveDirection = camForward * input.y + camRight * input.x;

        transform.position += moveDirection * MoveSpeed * Time.deltaTime;
        isWalking = moveDirection != Vector3.zero;

    }

    public void HandleJump()
    {
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

}
