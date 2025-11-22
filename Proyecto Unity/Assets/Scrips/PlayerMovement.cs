using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float smoothTurnTime = 0.1f;
    [SerializeField] private float smoothMoveTime = 0.1f;

    [Header("Configuración de Cámara")]
    [SerializeField] private Transform cameraTransform;

    [Header("Gravedad y Salto")]
    [SerializeField] private float gravity = -20f;          // 👈 Más peso al caer
    [SerializeField] private float jumpHeight = 1.2f;       // 👈 Salto más bajo
    [SerializeField] private float fallGravityMultiplier = 2f; // 👈 Aumenta gravedad solo al caer

    [Header("Límite de Caída")]
    [SerializeField] private float fallLimit = -10f;

    [Header("Animaciones")]
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private float turnSmoothVelocity;
    private Vector3 currentMoveDir;
    private Vector3 moveDirSmoothVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleMovementAndJump();
        CheckFallOffMap();
        UpdateAnimations();
    }

    void HandleMovementAndJump()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDir = Vector3.zero;

        bool isTryingToRun = Input.GetKey(KeyCode.LeftShift) && inputDir.magnitude >= 0.1f;
        float currentSpeed = isTryingToRun ? runSpeed : walkSpeed;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothTurnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 targetMoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            currentMoveDir = Vector3.SmoothDamp(currentMoveDir, targetMoveDir, ref moveDirSmoothVelocity, smoothMoveTime);
            moveDir = currentMoveDir.normalized * currentSpeed;
        }

        // 🚀 Salto
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetBool("IsJump", true);
            }
        }
        else
        {
            // 📌 Aumentar gravedad solo cuando cae (más natural)
            if (velocity.y < 0)
                velocity.y += gravity * fallGravityMultiplier * Time.deltaTime;
            else
                velocity.y += gravity * Time.deltaTime;
        }

        controller.Move((moveDir + new Vector3(0, velocity.y, 0)) * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float inputMagnitude = new Vector2(horizontal, vertical).magnitude;

        bool isTryingToRun = Input.GetKey(KeyCode.LeftShift) && inputMagnitude > 0.1f;
        float movementSpeed = isTryingToRun ? runSpeed * inputMagnitude : walkSpeed * inputMagnitude;

        animator.SetFloat("Speed", movementSpeed, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", controller.isGrounded);
        animator.SetFloat("VerticalSpeed", velocity.y);

        if (controller.isGrounded && !Input.GetButtonDown("Jump"))
            animator.SetBool("IsJump", false);
    }

    void CheckFallOffMap()
    {
        if (transform.position.y < fallLimit)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
