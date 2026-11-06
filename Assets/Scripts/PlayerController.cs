using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float jumpForce = 11f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackRange = 1.15f;
    [SerializeField] private float attackCooldown = 0.35f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    private Rigidbody2D body;
    private int health;
    private float nextAttackTime;
    private float horizontalInput;
    private bool isGrounded;
    private int upgradeCost = 10;
    private float facingDirection = 1f;

    public int Health => health;
    public int MaxHealth => maxHealth;
    public int AttackDamage => attackDamage;
    public int NextUpgradeCost => upgradeCost;
    public float HorizontalInput => horizontalInput;
    public bool IsGrounded => isGrounded;
    public bool IsSprinting => Input.GetKey(KeyCode.LeftShift);

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        health = maxHealth;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0f)
        {
            facingDirection = Mathf.Sign(horizontalInput);
            if (attackPoint != null)
                attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x) * facingDirection,
                    attackPoint.localPosition.y, attackPoint.localPosition.z);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            body.velocity = new Vector2(body.velocity.x, jumpForce);

        if ((Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)) && Time.time >= nextAttackTime)
            Attack();

        if (Input.GetKeyDown(KeyCode.U))
            BuyDamageUpgrade();

    }

    private void FixedUpdate()
    {
        float currentSpeed = IsSprinting ? sprintSpeed : moveSpeed;
        body.velocity = new Vector2(horizontalInput * currentSpeed, body.velocity.y);
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackCooldown;

        if (attackPoint == null)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
                enemy.TakeDamage(attackDamage);
        }
    }

    private void BuyDamageUpgrade()
    {
        if (CoinManager.Instance != null && CoinManager.Instance.TrySpend(upgradeCost))
        {
            attackDamage += 5;
            upgradeCost += 10;
        }
    }

    public void TakeDamage(int amount)
    {
        health = Mathf.Max(0, health - Mathf.Max(0, amount));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
