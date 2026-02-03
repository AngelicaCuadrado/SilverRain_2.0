using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float groundCheckRayLength = 1.1f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private AudioClip jumpSound;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSmoothSpeed = 10f;
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;

    private Rigidbody rb;
    private Vector2 movementInput;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private AudioSource audioSource;

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckRayLength, groundLayer);
    }

    private void FixedUpdate()
    {
        var moveDirection = transform.right * movementInput.x + transform.forward * movementInput.y;
        var targetVelocity = moveDirection.normalized * moveSpeed;
        Vector3 velocityChange = new(targetVelocity.x - rb.linearVelocity.x, 0, targetVelocity.z - rb.linearVelocity.z);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (lookInput.magnitude > 0.1f)
        {
            transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

            xRotation -= lookInput.y * mouseSensitivity;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
            var targetRotation = Quaternion.Euler(xRotation, 0f, 0f);
            cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }

            float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics.gravity.y * gravityScale));
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void ResetVelocity()
    {
        rb.linearVelocity = Vector3.zero;
    }

    // public void EnemyKilled(string enemyType)
    // {
    //     var hud = FindAnyObjectByType<HUDController>();
    //     if (hud != null)
    //     {
    //         hud.SpownKillInfo(enemyType);
    //     }
    //
    //     var playerLevel = GetComponent<PlayerLevel>();
    //     var stats = GetComponent<PlayerStats>();
    //
    //     if (playerLevel != null)
    //     {
    //         float baseXp = 10f;
    //         float xpMod = (stats != null) ? stats.experienceMod : 1f;
    //         playerLevel.GainXP(baseXp * xpMod);
    //     }
    // }
}