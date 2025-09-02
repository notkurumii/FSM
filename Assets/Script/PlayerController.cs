using UnityEngine;
using TMPro; // Add this line to use TextMeshPro

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Public Variables (Editable in the Inspector) ---
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -15f;
    
    [Header("Visuals")]
    public Color idleColor = Color.black;
    public Color walkColor = new Color(0.2f, 0.6f, 1f); // A nice blue
    public Color jumpColor = new Color(0.2f, 1f, 0.2f); // A bright green
    public TextMeshPro statusText; // Assign a TextMeshPro object here

    // --- Private Variables ---
    private CharacterController controller;
    private Renderer characterRenderer; // To change the color
    private Vector3 velocity;
    // We no longer need a separate isGrounded variable.

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        characterRenderer = GetComponent<Renderer>(); // Get the Renderer component
    }

    void Update()
    {
        // We now use the CharacterController's built-in isGrounded property.
        // It's more reliable and updates automatically after a Move() call.
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // --- 2. MOVEMENT ---
        float x = Input.GetAxis("Horizontal");
        Vector3 moveDirection = transform.right * x;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // --- 3. JUMPING ---
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- 4. APPLY GRAVITY ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- 5. UPDATE VISUALS ---
        UpdateVisualState();
    }

    void UpdateVisualState()
    {
        // --- UPDATED to use controller.isGrounded ---

        // If we are in the air
        if (!controller.isGrounded)
        {
            // State: Jumping / Falling
            characterRenderer.material.color = jumpColor;
            if (statusText != null) statusText.text = "Jumping";
        }
        else // Otherwise, we are grounded
        {
            // Check for horizontal input to determine idle vs. walk
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f)
            {
                // State: Walking
                characterRenderer.material.color = walkColor;
                if (statusText != null) statusText.text = "Walking";
            }
            else
            {
                // State: Idle
                characterRenderer.material.color = idleColor;
                if (statusText != null) statusText.text = "Idle";
            }
        }
    }
}