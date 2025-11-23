using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public Transform player;
    public Transform cameraTarget;
    public Vector3 shoulderOffset = new Vector3(0.3f, 1.7f, -2f);

    [Header("Velocidades")]
    public float followSpeed = 8f;
    public float rotationSmoothSpeed = 12f;
    public float mouseSensitivity = 0.8f; // 🔥 Sensibilidad reducida

    [Header("Órbita (Rotación)")]
    private float yaw = 0f;
    private float pitch = 0f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;

    private Transform mainCamera;
    private Quaternion smoothRotation;

    void Start()
    {
        mainCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        HandleInput();
        UpdateCamera();
    }

    void HandleInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void UpdateCamera()
    {
        // Rotación suave
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
        smoothRotation = Quaternion.Slerp(smoothRotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);

        // Posición suave
        Vector3 desiredPosition = cameraTarget.position + smoothRotation * shoulderOffset;
        mainCamera.position = Vector3.Lerp(mainCamera.position, desiredPosition, followSpeed * Time.deltaTime);

        // Mirar al objetivo
        mainCamera.LookAt(cameraTarget);
    }
}
