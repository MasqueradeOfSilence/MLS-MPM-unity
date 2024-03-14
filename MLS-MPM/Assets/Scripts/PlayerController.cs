using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 100f;
    public Vector3 cameraOffset = new Vector3(0f, 1.5f, -2f); // Adjust these values as needed

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);

        // Rotation
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up, mouseX);

        mainCamera.transform.RotateAround(transform.position, mainCamera.transform.right, -mouseY);

        // Set camera position slightly in front of the player and facing opposite direction
        mainCamera.transform.position = transform.position + transform.TransformDirection(cameraOffset);
        mainCamera.transform.rotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
    }
}
