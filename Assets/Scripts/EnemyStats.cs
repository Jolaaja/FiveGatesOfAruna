using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Balancing Prototype/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Identity")]
    public string enemyName = "Enemy";

    [Header("Combat")]
    [Min(1f)] public float maxHealth = 50f;
    [Min(0f)] public float damage = 10f;
    [Min(0f)] public float moveSpeed = 2f;
    [Min(0.1f)] public float attackCooldown = 1.5f;
    [Range(0f, 0.9f)] public float armorPercent = 0.2f;

    [Header("Economy")]
    [Min(0)] public int cost = 10;
    [Min(0)] public int coinReward = 5;

    public float DPS => damage / Mathf.Max(0.1f, attackCooldown);
    public float EffectiveHP => maxHealth / Mathf.Max(0.1f, 1f - armorPercent);
    public float PowerScore => DPS * EffectiveHP;
}
