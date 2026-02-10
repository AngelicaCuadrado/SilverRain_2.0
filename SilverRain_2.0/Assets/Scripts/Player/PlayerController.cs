using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float rotationSmoothSpeed = 10f;
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;
    
    [Header("Jump Settings")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    
    [Header("Audio")]
    [SerializeField] private string jumpSfxId = "sfx_jump";
    
    private Rigidbody _rb;
    private float _xRotation;
    private bool _canMove = true;
    // private Vector2 movementInput;
    // private Vector2 lookInput;
    
    private InputMode _lastInputMode = InputMode.Gameplay;
    
    // jump state
    private float _lastGroundedTime;
    private float _lastJumpPressedTime;
    private bool _isGrounded;
    private bool _wasGroundedLastFrame;

    
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public bool IsGrounded() => _isGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.useGravity = true;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null) 
                cameraTransform = cam.transform;
        }

        InputManager.Instance.OnJump.AddListener(OnJumpInput);
        InputManager.Instance.OnPause.AddListener(OnPauseInput);
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnJump.RemoveListener(OnJumpInput);
        InputManager.Instance.OnPause.RemoveListener(OnPauseInput);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    private void Update()
    {
        UpdateCursorState();
        
        _wasGroundedLastFrame = _isGrounded;
        _isGrounded = CheckGrounded();
        
        if (_isGrounded) _lastGroundedTime = Time.time;

        if (CanJump() && Time.time - _lastJumpPressedTime <= jumpBufferTime)
        {
            PerformJump();
            _lastJumpPressedTime = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;
        if (!IsGameplayModeActive()) return;

        HandleMovement();
        
    }

    private void LateUpdate()
    {
        if (!_canMove) return;
        if (!IsGameplayModeActive()) return;

        HandleLook();
    }

    private bool IsGameplayModeActive()
    {
        return InputManager.Instance.CurrentMode == InputMode.Gameplay;
    }

    private void UpdateCursorState()
    {
        InputMode mode = InputManager.Instance.CurrentMode;

        if (mode == _lastInputMode) return;

        _lastInputMode = mode;

        if (mode == InputMode.Gameplay)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else // UI mode
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleMovement()
    {
        var moveInput = InputManager.Instance?.Move ?? Vector2.zero;
        
        var moveDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        var targetVelocity = moveDir.normalized * moveSpeed;

        var velocityChange = new Vector3(
            targetVelocity.x - _rb.linearVelocity.x,
            0f,
            targetVelocity.z - _rb.linearVelocity.z
        );
        
        _rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void HandleLook()
    {
        var lookInput = InputManager.Instance?.Look ?? Vector2.zero;

        if (lookInput.sqrMagnitude < 0.001f) return;

        // Horizontal rotation (rotate player body)
        transform.Rotate(Vector3.up, lookInput.x * mouseSensitivity);

        // Vertical rotation (rotate camera)
        _xRotation -= lookInput.y * mouseSensitivity;
        _xRotation = Mathf.Clamp(_xRotation, minVerticalAngle, maxVerticalAngle);

        if (cameraTransform)
        {
            var targetRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            cameraTransform.localRotation = Quaternion.Slerp(
                cameraTransform.localRotation,
                targetRotation,
                Time.deltaTime * rotationSmoothSpeed
            );
        }
    }

    private void OnJumpInput()
    {
        if (!_canMove) return;
        if (!IsGameplayModeActive()) return;

        // Record jump press time for buffer
        _lastJumpPressedTime = Time.time;

        // Try immediate jump
        if (CanJump())
        {
            PerformJump();
            _lastJumpPressedTime = 0f; // Clear buffer since we jumped
        }
    }

    private void OnPauseInput()
    {
        var pauseWindow = PauseManager.Instance.PauseWindow;
        if (pauseWindow != null)
            UIManager.Instance.Push(pauseWindow);
    }
    
    private bool CanJump()
    {
        // Can jump if grounded OR within coyote time
        return _isGrounded || (Time.time - _lastGroundedTime <= coyoteTime);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void PerformJump()
    {
        // Reset coyote time to prevent double jump
        _lastGroundedTime = 0f;

        // Reset vertical velocity before jumping for consistent jump height
        Vector3 vel = _rb.linearVelocity;
        vel.y = 0f;
        _rb.linearVelocity = vel;

        // Calculate jump force from desired height
        float jumpForce = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Play jump sound
        AudioManager.Instance.PlaySFX(jumpSfxId);
    }
    
    private bool CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * groundCheckRadius;
        float checkDistance = groundCheckDistance + groundCheckRadius;

        // Method 1: SphereCast (catches slopes and edges)
        bool sphereHit = Physics.SphereCast(origin, groundCheckRadius * 0.9f, Vector3.down, out _, checkDistance, groundLayer);

        // Method 2: Raycast (more precise for flat surfaces)
        bool rayHit = Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, groundCheckDistance + 0.1f, groundLayer);

        return sphereHit || rayHit;
    }
    
    #region Public Methods
    
    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }
    
    public void Teleport(Vector3 position)
    {
        _rb.position = position;
        _rb.linearVelocity = Vector3.zero;
    }
    
    public void AddForce(Vector3 force)
    {
        _rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void ResetVelocity()
    {
        _rb.linearVelocity = Vector3.zero;
    }
    #endregion
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw ground check sphere
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Vector3 origin = transform.position + Vector3.up * groundCheckRadius;
        Gizmos.DrawWireSphere(origin, groundCheckRadius * 0.9f);

        // Draw check endpoint
        Vector3 endPoint = origin + Vector3.down * (groundCheckDistance + groundCheckRadius);
        Gizmos.DrawWireSphere(endPoint, groundCheckRadius * 0.9f);
        Gizmos.DrawLine(origin, endPoint);

        // Draw raycast
        Gizmos.color = Color.yellow;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.05f;
        Gizmos.DrawRay(rayOrigin, Vector3.down * (groundCheckDistance + 0.1f));
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Label($"Position Y: {transform.position.y:F3}");
        GUILayout.Label($"Velocity Y: {(_rb != null ? _rb.linearVelocity.y : 0):F3}");
        GUILayout.Label($"Grounded: {_isGrounded}");
        GUILayout.Label($"Can Jump: {CanJump()}");
        GUILayout.Label($"Coyote Time Left: {Mathf.Max(0, coyoteTime - (Time.time - _lastGroundedTime)):F2}s");
        GUILayout.EndArea();
    }
#endif
}
