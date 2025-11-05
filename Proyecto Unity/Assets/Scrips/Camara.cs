using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public Transform player;                  // El jugador
    public Transform cameraTarget;            // Punto de enfoque (por ejemplo, la cabeza del jugador)
    public Vector3 shoulderOffset = new Vector3(0.3f, 1.7f, -2f);
    public float followSpeed = 10f;
    public float rotationSpeed = 5f;
    public float mouseSensitivity = 2f;

    [Header("Órbita (Rotación con el ratón)")]
    public float yaw = 0f;                    // Rotación horizontal
    private float pitch = 0f;                 // Rotación vertical
    [SerializeField] private float minPitch = -30f;   // 🔹 límite inferior
    [SerializeField] private float maxPitch = 60f;    // 🔹 límite superior

    private Transform mainCamera;

    void Start()
    {
        mainCamera = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        HandleInput();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Gira con el ratón
        yaw += mouseX * rotationSpeed;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPosition = cameraTarget.position + rotation * shoulderOffset;

        // Movimiento suave
        mainCamera.position = Vector3.Lerp(mainCamera.position, targetPosition, followSpeed * Time.deltaTime);
        mainCamera.LookAt(cameraTarget);
    }
}
