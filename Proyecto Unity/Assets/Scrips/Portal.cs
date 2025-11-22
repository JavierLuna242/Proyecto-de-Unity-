using UnityEngine;

public class PortalFinJuego : MonoBehaviour
{
    public GameObject mensajeFinJuego;
    private bool juegoTerminado = false;

    private void Start()
    {
        mensajeFinJuego.SetActive(false); // Aseguramos que empiece oculto
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mensajeFinJuego.SetActive(true); 
            Time.timeScale = 0f; 
            juegoTerminado = true;
        }
    }

    void Update()
    {
        if (juegoTerminado && Input.GetKeyDown(KeyCode.Return))
        {
            Time.timeScale = 1f;
            Application.Quit();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
