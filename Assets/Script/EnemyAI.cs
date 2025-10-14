using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyAI_BehaviourTree : MonoBehaviour
{
    [Header("AI Properties")]
    public float moveSpeed = 3f;
    public float roamRadius = 8f;
    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Health")]
    public int maxHP = 5;
    public int currentHP;
    public Image hpBarFill; // Drag UI Image (fill) ke sini di Inspector

    [Header("Visuals")]
    public TextMeshPro statusText;

    void Awake()
    {
        currentHP = maxHP;
        UpdateHPBar();
    }

    private PointClickController clickController;

    void Start()
    {
        clickController = FindObjectOfType<PointClickController>();
    }

    void Update()
    {
        if (clickController != null && clickController.hasTarget)
        {
            // Raycast ke ground untuk dapatkan posisi Y
            Vector3 moveTarget = clickController.targetPosition;
            RaycastHit hit;
            if (Physics.Raycast(moveTarget + Vector3.up * 10f, Vector3.down, out hit, 20f))
            {
                moveTarget.y = hit.point.y;
            }
            else
            {
                moveTarget.y = transform.position.y; // fallback
            }
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(moveTarget.x, transform.position.y, moveTarget.z));
            if (statusText) statusText.text = "Moving to Point";
        }
        else
        {
            if (statusText) statusText.text = "Idle";
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;
        UpdateHPBar();
        if (currentHP == 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateHPBar()
    {
        if (hpBarFill != null)
        {
            hpBarFill.fillAmount = (float)currentHP / maxHP;
        }
    }
}