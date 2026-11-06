using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;
    [SerializeField] private float attackRange = 1.1f;

    private Rigidbody2D body;
    private Transform player;
    private float health;
    private float nextAttackTime;
    private bool defeated;

    public EnemyStats Stats => stats;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (stats == null)
        {
            Debug.LogError($"{name} has no EnemyStats asset assigned.", this);
            enabled = false;
            return;
        }

        health = stats.maxHealth;
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            player = playerController.transform;
    }

    private void FixedUpdate()
    {
        if (defeated || player == null)
        {
            body.velocity = new Vector2(0f, body.velocity.y);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            body.velocity = new Vector2(direction * stats.moveSpeed, body.velocity.y);
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }
        else
        {
            body.velocity = new Vector2(0f, body.velocity.y);
            if (Time.time >= nextAttackTime)
            {
                player.GetComponent<PlayerController>().TakeDamage(Mathf.RoundToInt(stats.damage));
                nextAttackTime = Time.time + stats.attackCooldown;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (defeated)
            return;

        health -= Mathf.Max(0, amount) * (1f - stats.armorPercent);
        if (health <= 0f)
        {
            defeated = true;
            if (CoinManager.Instance != null)
                CoinManager.Instance.AddCoins(stats.coinReward);
            Destroy(gameObject);
        }
    }
}
