using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 5.0f;
    public float flySpeed = 5.0f;
    public float mouseSensitivity = 2.0f;
    private float vAxis;
    private float hAxis;
    private bool isGrounded;
    private bool isFlying;
    private Rigidbody rb;
    private Camera playerCamera;
    private Vector3 moveDirection;
    private float gravity = -9.81f;
    private float verticalVelocity;
    private float lastJumpTime;
    private float doubleJumpDelay = 0.3f;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        playerCamera = Camera.main;
        isFlying = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        // Get input axes
        vAxis = Input.GetAxis("Vertical");
        hAxis = Input.GetAxis("Horizontal");

        // Handle rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);
        playerCamera.transform.Rotate(-mouseY, 0, 0);

        // Handle movement
        moveDirection = transform.forward * vAxis + transform.right * hAxis;
        moveDirection *= speed;

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Handle flying
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlying = !isFlying;
            rb.useGravity = !isFlying;
        }

        if (isFlying)
        {
            float flyVertical = 0;
            if (Input.GetKey(KeyCode.Space))
            {
                flyVertical = flySpeed;
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                flyVertical = -flySpeed;
            }
            rb.linearVelocity = new Vector3(moveDirection.x, flyVertical, moveDirection.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
}