using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyController : MonoBehaviour
{
    [Header("Movimiento del enemigo")]
    public float moveSpeed = 2f;          
    public float chaseSpeed = 3.5f;            
    public float patrolRadius = 5f;             
    public float waitTime = 2f;                

    [Header("Persecución del jugador")]
    public float detectionRadius = 7f;         
    public float loseSightRadius = 10f;      

    [Header("Ataque al jugador")]
    public float damage = 20f;                
    public float attackRange = 1.5f;           
    public float attackCooldown = 1f;         

    [Header("Vida del enemigo")]
    public float maxHealth = 100f;
    private float currentHealth;

    private Transform player;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float nextAttackTime = 0f;
    private bool isChasing = false;

    void Start()
    {
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        SetNewTargetPosition();

        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
        }
        else if (distanceToPlayer >= loseSightRadius)
        {
            isChasing = false;
        }

        if (isChasing)
            ChasePlayer();
        else
            Patrol();

        if (isChasing && distanceToPlayer <= attackRange)
        {
            TryAttack();
        }
    }

    void Patrol()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                SetNewTargetPosition();
            }
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        Vector3 dir = (targetPosition - transform.position).normalized;
        if (dir.magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(dir);

        if (Vector3.Distance(transform.position, targetPosition) < 0.3f)
        {
            isWaiting = true;
            waitTimer = 0f;
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;

        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * chaseSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void SetNewTargetPosition()
    {
        Vector2 random = Random.insideUnitCircle * patrolRadius;
        targetPosition = new Vector3(startPosition.x + random.x, startPosition.y, startPosition.z + random.y);
    }

    void TryAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log($" Enemigo golpea al jugador ({damage} daño)");
                }

                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Enemigo recibe {amount} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemigo eliminado");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startPosition == Vector3.zero ? transform.position : startPosition, patrolRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
