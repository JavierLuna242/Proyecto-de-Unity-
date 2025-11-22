using UnityEngine;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [Header("Panel y Texto")]
    public GameObject introPanel;
    public TextMeshProUGUI introText;

    [Header("Configuración")]
    public KeyCode startKey = KeyCode.Space; 

    private bool gameStarted = false;

    void Start()
    {
        if (introPanel != null)
            introPanel.SetActive(true);

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
        Time.timeScale = 1f;
    }
}
