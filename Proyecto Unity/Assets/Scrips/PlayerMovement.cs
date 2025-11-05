using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 5f;              // Velocidad de movimiento
    [SerializeField] private float smoothTurnTime = 0.1f;   // Suavidad al girar
    [SerializeField] private float smoothMoveTime = 0.1f;   // Suavidad al moverse

    [Header("Configuración de Cámara")]
    [SerializeField] private Transform cameraTransform;     // Arrastra aquí la Main Camera

    [Header("Configuración de Gravedad y Salto")]
    [SerializeField] private float gravity = -9.81f;        // Gravedad hacia abajo
    [SerializeField] private float jumpHeight = 2f;         // Altura del salto

    [Header("Límite de Caída")]
    [SerializeField] private float fallLimit = -10f;        // Altura de muerte

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentMoveDir;
    private Vector3 moveDirSmoothVelocity;
    private float turnSmoothVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError(" No se encontró un CharacterController en este GameObject.");
        }
    }

    void Update()
    {
        HandleMovementAndJump();
        CheckFallOffMap();
    }

    void HandleMovementAndJump()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 moveDir = Vector3.zero;
        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothTurnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 targetMoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            currentMoveDir = Vector3.SmoothDamp(currentMoveDir, targetMoveDir, ref moveDirSmoothVelocity, smoothMoveTime);
            moveDir = currentMoveDir.normalized * speed;
        }

        // --- Gravedad y salto ---
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // --- Movimiento combinado (horizontal + vertical) ---
        Vector3 totalMove = (moveDir + new Vector3(0, velocity.y, 0)) * Time.deltaTime;
        controller.Move(totalMove);
    }

    void CheckFallOffMap()
    {
        if (transform.position.y < fallLimit)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
