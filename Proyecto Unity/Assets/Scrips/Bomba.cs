using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [Header("Throw / Explosion")]
    public float throwForce = 10f;
    public float upwardForce = 2f;
    public float explodeDelayAfterThrow = 3f;
    public float explodeRadius = 3f;
    public float explodeDamage = 50f;
    public float explosionForce = 500f;

    private Rigidbody rb;
    private bool isHeld = false;
    private bool hasTouchedGround = false;
    private Transform holder;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
    }

    void Update()
    {
        if (isHeld && holder != null)
        {
            transform.position = holder.position;
            transform.rotation = holder.rotation;
        }
    }

    public void PickUp(Transform playerHand)
    {
        if (isHeld) return;

        isHeld = true;
        holder = playerHand;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        transform.SetParent(playerHand, true);
        hasTouchedGround = false;
    }

    public void Throw()
    {
        if (!isHeld) return;

        isHeld = false;
        transform.SetParent(null, true);
        rb.isKinematic = false;
        hasTouchedGround = false;

        Vector3 forceDir = holder.forward + Vector3.up * 0.2f;
        rb.AddForce(forceDir.normalized * throwForce + Vector3.up * upwardForce, ForceMode.Impulse);

        holder = null;

        if (explodeDelayAfterThrow > 0f)
        {
            StartCoroutine(ExplodeAfterDelay(explodeDelayAfterThrow));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHeld && !hasTouchedGround)
        {
            hasTouchedGround = true;
            StartCoroutine(StopAfterDelay(0.5f));
        }
    }

    private IEnumerator StopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    private void Explode()
    {
        Debug.Log("La bomba explotó.");

        Collider[] hits = Physics.OverlapSphere(transform.position, explodeRadius);

        foreach (Collider hit in hits)
        {

            Rigidbody hitRb = hit.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(explosionForce, transform.position, explodeRadius);
            }

            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(explodeDamage);
                Debug.Log($"Enemigo dañado por bomba ({explodeDamage}).");
                continue;
            }

            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(explodeDamage / 2f); 
                Debug.Log($"Jugador dañado por bomba ({explodeDamage / 2f}).");
                continue;
            }

            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(explodeDamage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
