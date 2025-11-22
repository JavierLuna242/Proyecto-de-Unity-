using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public Transform player;                
    public Transform cameraTarget;            
    public Vector3 shoulderOffset = new Vector3(0.3f, 1.7f, -2f);
    public float followSpeed = 10f;
    public float rotationSpeed = 5f;
    public float mouseSensitivity = 2f;

    [Header("Órbita (Rotación con el ratón)")]
    public float yaw = 0f;                 
    private float pitch = 0f;               
    [SerializeField] private float minPitch = -30f; 
    [SerializeField] private float maxPitch = 60f;   

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

        yaw += mouseX * rotationSpeed;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPosition = cameraTarget.position + rotation * shoulderOffset;

        mainCamera.position = Vector3.Lerp(mainCamera.position, targetPosition, followSpeed * Time.deltaTime);
        mainCamera.LookAt(cameraTarget);
    }
}
