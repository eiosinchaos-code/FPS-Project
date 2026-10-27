using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    private CharacterController controller;
    private Vector3 velocity;

    [Header("Look Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 2.0f;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null && GetComponentInChildren<Camera>() != null)
        {
            playerCamera = GetComponentInChildren<Camera>().transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        transform.Rotate(Vector3.up * mouseX);

        // 2. WASD Movement Input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 motion = move * moveSpeed;

        // 3. Gravity and Ground Check
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 4. Jumping (Spacebar)
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        // 5. Apply both horizontal and vertical movement in ONE single call
        motion.y = velocity.y;
        controller.Move(motion * Time.deltaTime);
    }
}