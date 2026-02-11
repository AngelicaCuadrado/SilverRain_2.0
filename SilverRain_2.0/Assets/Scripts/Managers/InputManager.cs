using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum InputMode
{
    Gameplay,
    UI
}
public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    
    public static InputManager Instance { get; private set; }
    
    #region InputActions
    public InputActionAsset Actions { get; private set; }
    
    private InputAction _move;
    private InputAction _look;
    private InputAction _jump;
    private InputAction _pause;
    
    /// <summary>Reads current Move value. If _move is null, returns Vector2.zero.</summary>
    public Vector2 Move => _move != null ? _move.ReadValue<Vector2>() : Vector2.zero;
    public Vector2 Look => _look != null ? _look.ReadValue<Vector2>() : Vector2.zero;
    
    /// <summary>Raised when Jump is performed.</summary>
    public UnityEvent OnJump;

    /// <summary>Raised when Pause is performed.</summary>
    public UnityEvent OnPause;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Actions = inputActions;
        
        // FindAction("Map/Action") returns null if not found when throwIfNotFound=false.
        _move = Actions != null ? Actions.FindAction("Gameplay/Move", throwIfNotFound: false) : null;
        _look = Actions != null ? Actions.FindAction("Gameplay/Look", throwIfNotFound: false) : null;
        _jump = Actions != null ? Actions.FindAction("Gameplay/Jump", throwIfNotFound: false) : null;
        _pause = Actions != null ? Actions.FindAction("Gameplay/Pause", throwIfNotFound: false) : null;

        // Bind events only if actions exist (prevents NullReference in misconfigured projects).
        if (_jump != null)
            _jump.performed += _ => OnJump?.Invoke();
        if (_pause != null)
            _pause.performed += _ => OnPause?.Invoke();
    }

    /// <summary>
    /// Enables Gameplay action map and disables UI map.
    /// This is typically used when gameplay is active.
    /// </summary>
    public void EnableGameplay()
    {
        if (!Actions) return;

        Actions.FindActionMap("UI", false)?.Disable();
        Actions.FindActionMap("Gameplay", false)?.Enable();
            
        //Debug.Log("Input Map : Gameplay Enabled");
    }

    /// <summary>
    /// Enables UI action map and disables Gameplay map.
    /// This is typically used when a menu is open.
    /// </summary>
    public void EnableUI()
    {
        if (!Actions) return;

        Actions.FindActionMap("Gameplay", false)?.Disable();
        Actions.FindActionMap("UI", false)?.Enable();
            
        //Debug.Log("Input Map : UI Enabled");
    }
    #endregion
    
    #region InputMode
    // Default mode when no tokens are held.
    // Token id generator
    private long _nextId = 1;
    // Active tokens (acts like a stack; last acquired wins)
    private readonly List<Handle> _handles = new();
    
    public InputMode BaseMode { get; private set; } = InputMode.Gameplay;

    /// <summary>
    /// The effective mode after considering base mode + active tokens.
    /// </summary>
    public InputMode CurrentMode => ComputeEffectiveMode();
    
    /// <summary>
    /// Sets the base mode (used when there are no active tokens).
    /// Example: during gameplay scene start you may set base to Gameplay,
    /// while in main menu you might set base to UI.
    /// </summary>
    public void SetBaseMode(InputMode mode)
    {
        BaseMode = mode;
        Apply(CurrentMode);
    }
    
    /// <summary>
    /// Acquire an input mode token.
    /// The returned token must be passed back to Release().
    /// "reason" is only for debugging / readability.
    /// </summary>
    public object Acquire(InputMode mode, string reason)
    {
        var token = new Handle(_nextId++, mode, reason);
        _handles.Add(token);
        Apply(CurrentMode);
        return token;
    }

    /// <summary>
    /// Releases a previously acquired token.
    /// If token is invalid or already released, this is a no-op.
    /// </summary>
    public void Release(object token)
    {
        if (token is not Handle h) return;

        // Remove by id (safe even if token is not the latest).
        for (int i = _handles.Count - 1; i >= 0; i--)
        {
            if (_handles[i].Id == h.Id)
            {
                _handles.RemoveAt(i);
                break;
            }
        }

        Apply(CurrentMode);
    }
    
    /// <summary>
    /// Computes the effective mode.
    /// Rule: last acquired wins. If no tokens, use base mode.
    /// </summary>
    private InputMode ComputeEffectiveMode()
    {
        if (_handles.Count == 0) return BaseMode;
        return _handles[^1].Mode;
    }
    
    /// <summary>
    /// Applies the mode to the underlying input system (enable/disable action maps).
    /// </summary>
    public void Apply(InputMode mode)
    {
        if (mode == InputMode.UI) EnableUI();
        else EnableGameplay();
    }
    
    /// <summary>
    /// Token object representing a temporary input mode override.
    /// Kept private to prevent external mutation.
    /// </summary>
    private sealed class Handle
    {
        public long Id { get; }
        public InputMode Mode { get; }
        public string Reason { get; }

        public Handle(long id, InputMode mode, string reason)
        {
            Id = id;
            Mode = mode;
            Reason = reason;
        }
    }
    #endregion
}
