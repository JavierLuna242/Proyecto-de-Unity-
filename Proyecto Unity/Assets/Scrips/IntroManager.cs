using UnityEngine;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [Header("Panel y Texto")]
    public GameObject introPanel;
    public TextMeshProUGUI introText;

    [Header("Configuración")]
    public KeyCode startKey = KeyCode.Space; // tecla para iniciar

    private bool gameStarted = false;

    void Start()
    {
        // Asegurarse de que el panel esté activo al inicio
        if (introPanel != null)
            introPanel.SetActive(true);

        // Detener el tiempo del juego (opcional)
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (!gameStarted && Input.GetKeyDown(startKey))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        gameStarted = true;

        if (introPanel != null)
            introPanel.SetActive(false);

        // Reanudar el tiempo del juego
        Time.timeScale = 1f;
    }
}
