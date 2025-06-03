using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float groundDistance = 0.4f;
    


    [Header("Reference")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;



    private float x;
    private float y;
    private Vector3  movementDirection;
    private Vector3 velocity;
    private bool isGrounded;
    


    void Start()
    {
        
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y <= 0)
        {
            velocity.y = -2f;
        }
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        movementDirection = transform.right * x + transform.forward * y;

        characterController.Move(movementDirection * moveSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
