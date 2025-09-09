using UnityEngine;
using TMPro;

public enum PlayerSuperState { Grounded, Airborne }
public enum PlayerGroundedSubState { Idle, Walking }

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

    // HFSM states
    private PlayerSuperState superState;
    private PlayerGroundedSubState groundedSubState;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        characterRenderer = GetComponent<Renderer>(); // Get the Renderer component
    }

    void Start()
    {
        SetSuperState(PlayerSuperState.Grounded);
    }

    void Update()
    {
        // --- 1. State Transitions ---
        if (controller.isGrounded)
        {
            if (superState != PlayerSuperState.Grounded)
                SetSuperState(PlayerSuperState.Grounded);
        }
        else
        {
            if (superState != PlayerSuperState.Airborne)
                SetSuperState(PlayerSuperState.Airborne);
        }

        // --- 2. State Behaviors ---
        switch (superState)
        {
            case PlayerSuperState.Grounded:
                GroundedUpdate();
                break;
            case PlayerSuperState.Airborne:
                AirborneUpdate();
                break;
        }

        // --- 3. Gravity ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void SetSuperState(PlayerSuperState newState)
    {
        superState = newState;
        if (superState == PlayerSuperState.Grounded)
        {
            velocity.y = -2f; // Reset fall speed
            // Tentukan substate
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f)
                groundedSubState = PlayerGroundedSubState.Walking;
            else
                groundedSubState = PlayerGroundedSubState.Idle;
        }
    }

    void GroundedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * x + transform.forward * z;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Substate switching
        if (Mathf.Abs(x) > 0.01f || Mathf.Abs(z) > 0.01f)
            groundedSubState = PlayerGroundedSubState.Walking;
        else
            groundedSubState = PlayerGroundedSubState.Idle;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            SetSuperState(PlayerSuperState.Airborne);
        }

        // Visuals
        switch (groundedSubState)
        {
            case PlayerGroundedSubState.Idle:
                characterRenderer.material.color = idleColor;
                if (statusText != null) statusText.text = "Idle";
                break;
            case PlayerGroundedSubState.Walking:
                characterRenderer.material.color = walkColor;
                if (statusText != null) statusText.text = "Walking";
                break;
        }
    }

    void AirborneUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * x + transform.forward * z;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        characterRenderer.material.color = jumpColor;
        if (statusText != null) statusText.text = "Jumping";
    }
}