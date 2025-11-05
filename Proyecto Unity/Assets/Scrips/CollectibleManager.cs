using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    [Header("Secuencia de Movimiento")]
    public float amplitud = 0.25f;
    public float speed = 2f;
    public float rotationSpeed = 45f;

    [Header("Sistema de Recolección")]
    public TextMeshProUGUI itemCounter;
    public int totalItemsScene = 2;
    public string collectibleTag = "Collectible";

    [Header("Lista de objetos")]
    public List<Transform> collectibles = new List<Transform>();
    public Dictionary<Transform, Vector3> startPosition = new Dictionary<Transform, Vector3>();

    private int itemsCollected = 0;

    void Start()
    {
        foreach (var obj in collectibles)
        {
            if (obj != null)
            {
                startPosition[obj] = obj.position;

                Collider col = obj.GetComponent<Collider>();
                if (col == null)
                    col = obj.gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;

                var detector = obj.GetComponent<PlayerCollectibleDetector>();
                if (detector == null)
                    obj.gameObject.AddComponent<PlayerCollectibleDetector>().Init(this);
            }
        }
    }

    void Update()
    {
        foreach (var obj in collectibles)
        {
            if (obj == null) continue;

            Vector3 startPos = startPosition[obj];
            float newY = startPos.y + Mathf.Sin(Time.time * speed) * amplitud;
            obj.position = new Vector3(startPos.x, newY, startPos.z);
            obj.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    public void Collect(Transform obj)
    {
        if (!collectibles.Contains(obj)) return;

        collectibles.Remove(obj);
        itemsCollected++;
        UpdateCounterUI();
        Destroy(obj.gameObject);
    }

    void UpdateCounterUI()
    {
        if (itemCounter != null)
            itemCounter.text = $"{itemsCollected} / {totalItemsScene}";
    }
}
