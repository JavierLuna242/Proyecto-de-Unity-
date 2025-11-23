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

    public GameObject explosionEffect;

    [Header("Hold Position Offset")]
    public Vector3 holdLocalPosition = new Vector3(0f, 0.40f, 0.25f);
    public Vector3 holdLocalRotation = new Vector3(0f, 0f, 0f);

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

    public void PickUp(Transform playerHand)
    {
        if (isHeld) return;

        isHeld = true;
        holder = playerHand;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        transform.SetParent(playerHand, false);
        transform.localPosition = holdLocalPosition;     
        transform.localEulerAngles = holdLocalRotation;

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
            StartCoroutine(ExplodeAfterDelay(explodeDelayAfterThrow));
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
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Debug.Log("La bomba explotó.");

        Collider[] hits = Physics.OverlapSphere(transform.position, explodeRadius);

        foreach (Collider hit in hits)
        {
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(explosionForce, transform.position, explodeRadius);
            }

            if (hit.CompareTag("Caja"))
            {
                Debug.Log("Caja destruida por la explosión.");
                Destroy(hit.gameObject);
                continue;
            }

            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(explodeDamage);
                continue;
            }

            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(explodeDamage / 2f);
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
