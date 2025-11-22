using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de vidas y escudo")]
    public int maxLives = 3; 
    public int currentLives;     
    public bool hasShield = false; 

    [Header("Interfaz (HUD)")]
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI shieldText;

    [Header("Efectos de muerte")]
    public GameObject deathEffect;
    public float restartDelay = 2f;

    private bool isDead = false;

    void Start()
    {
        currentLives = maxLives;
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        if (hasShield)
        {
            hasShield = false;
            Debug.Log("🛡️ El escudo absorbió el daño y se rompió.");
        }
        else
        {
            currentLives--;
            Debug.Log($"El jugador perdió una vida. Vidas restantes: {currentLives}");

            if (currentLives <= 0)
            {
                Die();
                return;
            }
        }

        UpdateUI();
    }

    public void AddLife(int amount = 1)
    {
        if (currentLives < maxLives)
        {
            currentLives = Mathf.Min(maxLives, currentLives + amount);
            Debug.Log($"Vida recogida. Vidas: {currentLives}");
            UpdateUI();
        }
    }
    public void AddShield()
    {
        if (!hasShield)
        {
            hasShield = true;
            Debug.Log("Escudo activado.");
            UpdateUI();
        }
    }
    void UpdateUI()
    {
        if (livesText != null)
            livesText.text = $"Vidas: {currentLives}";

        if (shieldText != null)
            shieldText.text = hasShield ? "Escudo: ACTIVADO" : "Escudo: ---";
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("El jugador ha muerto");

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        var controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        this.enabled = false;

        StartCoroutine(RestartSceneAfterDelay());
    }

    IEnumerator RestartSceneAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
