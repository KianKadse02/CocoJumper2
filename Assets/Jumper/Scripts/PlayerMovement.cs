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

    [Header("Wall Run Settings")]
    public float wallRunSpeed = 8f;
    [Tooltip("Upward force to fight gravity while wall running")]
    public float wallRunGravityCounter = 5f;
    [Tooltip("How long can you wall run before falling (seconds)")]
    public float maxWallRunDuration = 2f;
    [Tooltip("Force applied when jumping off a wall")]
    public float wallJumpForce = 15f;
    [Tooltip("How much force pushes you away from wall when jumping")]
    public float wallJumpAwayForce = 8f;

    private bool wasOnwall = false;
    private float wallRunTimer = 0f;
    private Vector3 wallRunDirection; // Direction we're running along the wall

    public PlayerUI playerUI;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        wallDetection = GetComponent<WallDetection>();
    }

    private void FixedUpdate()
    {
        

        if (wallDetection.isNearRunnableWall)
        {
            UpdateWallRun();
        } else
        {
            Vector3 moveDirection = transform.right * currentInput.x + transform.forward * currentInput.z;
            Vector3 horizontalVelocity = moveDirection * moveMentSpeed;
            body.linearVelocity = new Vector3(horizontalVelocity.x, body.linearVelocity.y, horizontalVelocity.z);

            if (body.linearVelocity.y < 0)
            {
                body.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
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
            wasOnwall = false;
        }
    }

    private void UpdateWallRun()
    {
        wallRunTimer += Time.fixedDeltaTime;

        // Check if wall run should end
        if (wallRunTimer >= maxWallRunDuration || !wallDetection.IsNearRunnableWall())
        {
            StopWallRun();
            return;
        }
        if (wasOnwall == true)
        {
            return;
        }

        playerUI.SetWallRunBarVisibility(true);
        playerUI.UpdateWallRunBar(wallRunTimer, maxWallRunDuration);

        // Calculate wall run direction (perpendicular to wall normal)
        Vector3 wallNormal = wallDetection.GetWallNormal();
        // Run along the wall, perpendicular to its surface
        wallRunDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;

        // Determine which direction to run based on player's current velocity
        if (Vector3.Dot(body.linearVelocity, wallRunDirection) < 0)
        {
            wallRunDirection = -wallRunDirection;
        }

        // Apply wall run movement
        Vector3 wallRunVelocity = wallRunDirection * wallRunSpeed;

        // Counter gravity with upward force (gradually weakens over time)
        float gravityCounterStrength = Mathf.Lerp(wallRunGravityCounter, 0, wallRunTimer / maxWallRunDuration);
        float upwardForce = gravityCounterStrength;

        body.linearVelocity = new Vector3(wallRunVelocity.x, upwardForce, wallRunVelocity.z);

        Debug.DrawRay(transform.position, wallRunDirection * 2f, Color.cyan);
    }
    private void StopWallRun()
    {
        playerUI.SetWallRunBarVisibility(false);
        wallRunTimer = 0f;
        wasOnwall = true;
        Debug.Log("Stopped wall run");
    }


    private void OnMove(InputValue value)
    {
        currentInput = new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y);
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
        if (!wallDetection.isAirborne && !wallDetection.isNearRunnableWall)
        {
            body.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse);
        }  else if (wallDetection.isAirborne && usedJumps < numJumps)
        {
            body.linearVelocity = new Vector3(body.linearVelocity.x, 0, body.linearVelocity.z);
            body.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse);
            usedJumps++;
        } else if (wallDetection.isNearRunnableWall && !wasOnwall)
        {
            body.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse);
            Vector3 wallNormal = wallDetection.GetWallNormal();

            // Jump up and away from wall
            Vector3 jumpDirection = (Vector3.up + wallNormal).normalized;

            // Stop current wall interaction
            StopWallRun();

            // Apply jump force
            body.linearVelocity = Vector3.zero; // Reset velocity first
            body.AddForce(jumpDirection * wallJumpForce, ForceMode.Impulse);
            body.AddForce(wallNormal * wallJumpAwayForce, ForceMode.Impulse);

        }
    }
}
