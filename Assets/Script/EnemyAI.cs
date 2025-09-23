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

    void Update()
    {
        // Enemy diam saja, tidak roaming dan tidak menyerang enemy lain
        if (statusText) statusText.text = "Idle";
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