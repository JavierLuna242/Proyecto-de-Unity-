using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyController : MonoBehaviour
{
    [Header("Movimiento del enemigo")]
    public float walkSpeed = 2f;
    public float runSpeed = 3.5f;
    public float smoothTurnTime = 0.1f;
    public float smoothMoveTime = 0.1f;
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

    [Header("Animaciones")]
    public Animator anim;

    private Transform player;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private Vector3 currentMoveDir;
    private Vector3 moveDirSmoothVelocity;
    private float turnSmoothVelocity;

    private bool isWaiting = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isDead = false;

    private float waitTimer;
    private float nextAttackTime;

    private float fixedY;

    void Start()
    {
        startPosition = transform.position;
        fixedY = transform.position.y;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;

        SetNewTargetPosition();

        if (anim == null)
            Debug.LogError("Asigna un Animator al enemigo " + gameObject.name);
    }

    void Update()
    {
        if (isDead || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRadius && !isAttacking)
            isChasing = true;
        else if (dist >= loseSightRadius)
            isChasing = false;

        if (!isAttacking)
        {
            if (isChasing) ChasePlayer();
            else Patrol();
        }

        if (isChasing && dist <= attackRange)
            TryAttack();

        UpdateAnimations();
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

        Vector3 dir = (targetPosition - transform.position);
        dir.y = 0f;
        float distance = dir.magnitude;
        dir.Normalize();

        Vector3 targetDir = dir * walkSpeed;
        Vector3 moveDir = Vector3.SmoothDamp(currentMoveDir, targetDir, ref moveDirSmoothVelocity, smoothMoveTime);
        currentMoveDir = moveDir;

        MoveAndRotate(moveDir);

        if (distance < 0.3f)
        {
            isWaiting = true;
            waitTimer = 0f;
        }
    }

    void ChasePlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;
        dir.Normalize();

        Vector3 targetDir = dir * runSpeed;
        Vector3 moveDir = Vector3.SmoothDamp(currentMoveDir, targetDir, ref moveDirSmoothVelocity, smoothMoveTime);
        currentMoveDir = moveDir;

        MoveAndRotate(moveDir);
    }

    void MoveAndRotate(Vector3 moveDir)
    {
        if (moveDir.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothTurnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        transform.position += moveDir * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }

    void TryAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            isAttacking = true;
            anim.SetTrigger("attack");
            nextAttackTime = Time.time + attackCooldown;

            Invoke(nameof(DealDamage), 0.4f);
            Invoke(nameof(ResetAttack), 0.8f);
        }
    }

    void DealDamage()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
            player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
    }

    void ResetAttack() => isAttacking = false;

    void UpdateAnimations()
    {
        if (isDead || anim == null) return;

        float speedPercent = currentMoveDir.magnitude / runSpeed;
        anim.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
    }
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        anim.SetTrigger("hurt");

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("die");
        Destroy(gameObject, 2f);
    }


    void SetNewTargetPosition()
    {
        Vector2 rnd = Random.insideUnitCircle * patrolRadius;
        targetPosition = new Vector3(startPosition.x + rnd.x, fixedY, startPosition.z + rnd.y);
    }
}
