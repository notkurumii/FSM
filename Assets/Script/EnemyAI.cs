using UnityEngine;
using TMPro; // Add this line to use TextMeshPro

// Define the possible states for the enemy
public enum EnemyState
{
    PATROL,
    CHASE,
    ATTACK
}

public class EnemyAI_Flip : MonoBehaviour
{
    // --- State Machine ---
    public EnemyState currentState;

    // --- AI Properties ---
    public Transform player;
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;

    // --- AI Senses ---
    public float detectionRadius = 10f;
    public float attackRange = 2f;

    [Header("Visuals")]
    public Color patrolColor = Color.cyan;
    public Color chaseColor = Color.yellow;
    public Color attackColor = Color.red;
    public TextMeshPro statusText; // Assign a TextMeshPro object here

    // --- Private Variables ---
    private int currentWaypointIndex = 0;
    private bool isFacingRight = true;
    private Renderer characterRenderer; // To change the color

    void Start()
    {
        characterRenderer = GetComponent<Renderer>();
        if (player == null)
        {
            Debug.LogError("Player transform is not assigned in the inspector!");
            return;
        }
        
        SetState(EnemyState.PATROL); // Set the initial state and visuals
    }

    void Update()
    {
        if (player == null) return;

        // The main logic for each state is now just checking for transitions
        switch (currentState)
        {
            case EnemyState.PATROL:
                Patrol();
                break;
            case EnemyState.CHASE:
                Chase();
                break;
            case EnemyState.ATTACK:
                Attack();
                break;
        }
    }
    
    // This function now controls all state changes and visual updates
    void SetState(EnemyState newState)
    {
        if (currentState == newState) return; // Don't re-enter the same state

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.PATROL:
                characterRenderer.material.color = patrolColor;
                if (statusText != null) statusText.text = "Patrolling";
                break;
            case EnemyState.CHASE:
                characterRenderer.material.color = chaseColor;
                if (statusText != null) statusText.text = "Chasing";
                break;
            case EnemyState.ATTACK:
                characterRenderer.material.color = attackColor;
                if (statusText != null) statusText.text = "Attacking!";
                break;
        }
    }


    // --- State Behaviors ---

    void Patrol()
    {
        if (Vector3.Distance(transform.position, player.position) < detectionRadius)
        {
            SetState(EnemyState.CHASE);
            return;
        }

        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 targetPosition = new Vector3(targetWaypoint.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (targetWaypoint.position.x > transform.position.x && !isFacingRight) Flip();
        else if (targetWaypoint.position.x < transform.position.x && isFacingRight) Flip();

        if (Mathf.Abs(transform.position.x - targetWaypoint.position.x) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void Chase()
    {
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            SetState(EnemyState.ATTACK);
            return;
        }
        if (Vector3.Distance(transform.position, player.position) > detectionRadius)
        {
            SetState(EnemyState.PATROL);
            return;
        }

        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);

        if (player.position.x > transform.position.x && !isFacingRight) Flip();
        else if (player.position.x < transform.position.x && isFacingRight) Flip();
    }

    void Attack()
    {
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            SetState(EnemyState.CHASE);
            return;
        }

        if (player.position.x > transform.position.x && !isFacingRight) Flip();
        else if (player.position.x < transform.position.x && isFacingRight) Flip();
    }

    // --- THIS FUNCTION HAS BEEN UPDATED ---
    void Flip()
    {
        isFacingRight = !isFacingRight;
        
        // Flip the parent (the enemy)
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        // --- NEW: Counter-flip the text to keep it readable ---
        if (statusText != null)
        {
            Vector3 textScale = statusText.transform.localScale;
            textScale.x *= -1;
            statusText.transform.localScale = textScale;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}