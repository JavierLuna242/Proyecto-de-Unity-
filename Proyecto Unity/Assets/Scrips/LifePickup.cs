using UnityEngine;

public class LifePickup : MonoBehaviour
{
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.AddLife(amount);
            Destroy(gameObject);
        }
    }
}
