using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectibleDetector : MonoBehaviour
{
    private CollectibleManager manager;

    public void Init(CollectibleManager managerRef)
    {
        manager = managerRef;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            manager.Collect(transform);
        }
    }
}
