using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 6f;
    public float roamRadius = 10f;
    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.2f;

    [Header("Visuals")]
    public Color idleColor = Color.black;
    public Color walkColor = new Color(0.2f, 0.6f, 1f);
    public Color attackColor = new Color(1f, 0.2f, 0.2f);
    public TextMeshPro statusText;

    private CharacterController controller;
    private Renderer characterRenderer;
    private Vector3 roamTarget;
    private float lastAttackTime = -999f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        characterRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        PickNewRoamTarget();
    }

    void Update()
    {
        if (AttackIfEnemyInRange()) return;
        if (ChaseNearestEnemy()) return;
        RoamRandomly();
    }

    // 1. Attack if enemy in range
    bool AttackIfEnemyInRange()
    {
        GameObject target = FindNearestEnemyInRange(attackRange);
        if (target != null)
        {
            characterRenderer.material.color = attackColor;
            if (statusText) statusText.text = "Attacking!";
            FaceTarget(target.transform.position);

            if (Time.time - lastAttackTime > attackCooldown)
            {
                lastAttackTime = Time.time;
                Destroy(target);
            }
            return true;
        }
        return false;
    }

    // 2. Chase nearest enemy if detected
    bool ChaseNearestEnemy()
    {
        GameObject target = FindNearestEnemyInRange(detectionRadius);
        if (target != null)
        {
            characterRenderer.material.color = walkColor;
            if (statusText) statusText.text = "Chasing";
            MoveTowards(target.transform.position, moveSpeed);
            FaceTarget(target.transform.position);
            return true;
        }
        return false;
    }

    // 3. Roam randomly
    void RoamRandomly()
    {
        if (statusText) statusText.text = "Roaming";
        characterRenderer.material.color = walkColor;
        if (Vector3.Distance(transform.position, roamTarget) < 1f)
        {
            PickNewRoamTarget();
        }
        MoveTowards(roamTarget, moveSpeed * 0.7f);
    }

    // --- Utility ---
    GameObject FindNearestEnemyInRange(float range)
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDist = Mathf.Infinity;
        foreach (var enemy in allEnemies)
        {
            if (enemy == this.gameObject) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < range && dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }
        return nearest;
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position);
        dir.y = 0;
        if (dir.magnitude > 0.1f)
        {
            controller.Move(dir.normalized * speed * Time.deltaTime);
        }
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 dir = (target - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.forward = dir.normalized;
        }
    }

    void PickNewRoamTarget()
    {
        Vector2 circle = Random.insideUnitCircle * roamRadius;
        roamTarget = new Vector3(transform.position.x + circle.x, transform.position.y, transform.position.z + circle.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, roamRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(roamTarget, 0.2f);
    }
}