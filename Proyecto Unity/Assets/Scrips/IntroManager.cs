using UnityEngine;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [Header("Panel y Texto")]
    public GameObject introPanel;
    public TextMeshProUGUI introText;

    private bool gameStarted = false;

    void Start()
    {
        if (introPanel != null)
            introPanel.SetActive(true);

        // Pausar el juego
        Time.timeScale = 0f;
    }

    void Update()
    {
        // Si no ha empezado y se presiona cualquier tecla
        if (!gameStarted && Input.anyKeyDown)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        gameStarted = true;

        if (introPanel != null)
            introPanel.SetActive(false);

        // Reanudar el juego
        Time.timeScale = 1f;
    }
}
