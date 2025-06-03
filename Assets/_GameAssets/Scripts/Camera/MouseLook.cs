using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform playerBody;



    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float xRotation = 0f;



    private float mouseX;
    private float mouseY;
    // Update is called once per frame

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 55f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
