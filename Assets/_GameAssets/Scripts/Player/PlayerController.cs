using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 8f;




    [Header("Reference")]
    //[SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;


    private Rigidbody _playerRigidbody;
    private float x;
    private float z;

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        // x = Input.GetAxis("Horizontal");
        // z = Input.GetAxis("Vertical");

        // Vector3 moveDirection = transform.right * x + transform.forward * z;
        SetMovement();
    }


    private void SetMovement()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * x + transform.forward * z;
        characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
    }
}
