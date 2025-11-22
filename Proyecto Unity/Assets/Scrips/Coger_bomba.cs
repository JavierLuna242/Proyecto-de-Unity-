using UnityEngine;

public class PlayerBombHandler : MonoBehaviour
{
    [Header("Pickup / Throw Settings")]
    [SerializeField] private Transform handPosition;    
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private LayerMask pickupLayerMask = ~0; 
    [SerializeField] private KeyCode pickupThrowKey = KeyCode.F;

    private Bomb heldBomb;

    void Update()
    {
        if (Input.GetKeyDown(pickupThrowKey))
        {
            if (heldBomb != null)
            {
                heldBomb.Throw();
                heldBomb = null;
            }
            else
            {
                TryPickUpBomb();
            }
        }
    }

    private void TryPickUpBomb()
    {
        Vector3 center = handPosition != null ? handPosition.position : transform.position;
        Collider[] cols = Physics.OverlapSphere(center, pickupRange, pickupLayerMask);

        foreach (Collider col in cols)
        {
            Bomb bomb = col.GetComponent<Bomb>();
            if (bomb != null)
            {
                bomb.PickUp(handPosition != null ? handPosition : transform);
                heldBomb = bomb;
                return;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (handPosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(handPosition.position, pickupRange);
        }
    }
}