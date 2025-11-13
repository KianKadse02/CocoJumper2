using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    private Rigidbody body;
    public float moveMentSpeed = 10;
    public float jumpFactor;
    protected Vector3 currentInput;

    public float fallMultiplier = 2.5f;

    [SerializeField]
    private int numJumps = 1;

    private int usedJumps = 0;

    [Header("Look Settings")]
    public Camera playerCamera;
    public float lookSensitivity = 100f;
    private float xRotation = 0f; // Stores the current up/down rotation of the camera


    private WallDetection wallDetection;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        wallDetection = GetComponent<WallDetection>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = transform.right * currentInput.x + transform.forward * currentInput.z;
        Vector3 horizontalVelocity = moveDirection * moveMentSpeed;
        body.linearVelocity = new Vector3(horizontalVelocity.x, body.linearVelocity.y, horizontalVelocity.z);

        if (body.linearVelocity.y < 0)
        {
            body.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!wallDetection.isAirborne)
        {
            usedJumps = 0;
        }
    }

    private void OnMove(InputValue value)
    {
        currentInput = new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y);
        print(currentInput.ToString());
        print("onmove");
    }

    private void OnLook(InputValue value)
    {
        // Get mouse input from the InputValue
        Vector2 lookInput = value.Get<Vector2>();
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        // --- Vertical Rotation (Pitch) ---
        // We subtract mouseY from xRotation because a positive Y input should rotate the camera down, and vice-versa
        xRotation -= mouseY;
        // Clamp the vertical rotation to prevent flipping upside down (e.g., between -90 and 90 degrees)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the rotation to the camera
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // --- Horizontal Rotation (Yaw) ---
        // Rotate the entire player body left and right
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnJump()
    {
        if (!wallDetection.isAirborne)
        {
            body.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse);
        }  else if (wallDetection.isAirborne && usedJumps < numJumps)
        {
            body.linearVelocity = new Vector3(body.linearVelocity.x, 0, body.linearVelocity.z);
            body.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse);
            usedJumps++;
        }
    }
}
