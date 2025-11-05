using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; 

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de vida")]
    public float maxHealth = 100f;
    public float currentHealth;
    public TextMeshProUGUI healthText;

    [Header("Efectos de muerte")]
    public GameObject deathEffect; 
    public float restartDelay = 2f; 

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"Vida: {currentHealth}";
        }
    }

    void Die()
    {
        Debug.Log("El jugador ha muerto");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Desactivar el jugador
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false; 
        this.enabled = false;

        StartCoroutine(RestartSceneAfterDelay());
    }

    IEnumerator RestartSceneAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        
        // Reinicia la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
