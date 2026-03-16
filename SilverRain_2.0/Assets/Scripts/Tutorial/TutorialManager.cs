using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum TutorialStep
{
    None,
    Move,
    Jump,
    Look,
    KillVisibleEnemy,
    KillInvisibleEnemy,
    CollectPickup,
    Complete
}

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text tutorialText;

    [Header("Objectives")]
    [SerializeField] private float moveDistanceRequired = 8f;
    [SerializeField] private int jumpsRequired = 3;
    [SerializeField] private float lookDistanceRequired = 250f;

    [Header("Messages")]
    [SerializeField] private string moveInstruction = "Move using WASD to continue.";
    [SerializeField] private string jumpInstruction = "Jump with Space.";
    [SerializeField] private string lookInstruction = "Move the camera with the mouse.";
    [SerializeField] private string visibleEnemyInstruction = "Your weapon shoots automatically. Defeat the visible monster.";
    [SerializeField] private string invisibleEnemyInstruction = "Defeat the invisible monster.";
    [SerializeField] private string pickupInstruction = "Pick up the item.";
    [SerializeField] private string completeInstruction = "Tutorial complete.";

    [Header("Events")]
    [SerializeField] private UnityEvent onTutorialStarted;
    [SerializeField] private UnityEvent onMovementStepCompleted;
    [SerializeField] private UnityEvent onJumpStepStarted;
    [SerializeField] private UnityEvent onJumpStepCompleted;
    [SerializeField] private UnityEvent onLookStepStarted;
    [SerializeField] private UnityEvent onLookStepCompleted;
    [SerializeField] private UnityEvent onVisibleEnemyStepStarted;
    [SerializeField] private UnityEvent onVisibleEnemyStepCompleted;
    [SerializeField] private UnityEvent onInvisibleEnemyStepStarted;
    [SerializeField] private UnityEvent onInvisibleEnemyStepCompleted;
    [SerializeField] private UnityEvent onPickupStepStarted;
    [SerializeField] private UnityEvent onTutorialCompleted;

    private TutorialStep currentStep = TutorialStep.None;
    private GameObject player;
    private Vector3 lastPlayerPosition;
    private float movedDistance;
    private int jumpsCompleted;
    private float lookedDistance;
    private Action activeStepUpdate;

    private void OnEnable()
    {
        EnemyHealth.OnEnemyKilled += HandleEnemyKilled;
        PlayerController.OnJumpPerformed += HandleJumpPerformed;
        Pickup.OnAnyPickupCollected += HandlePickupCollected;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyKilled -= HandleEnemyKilled;
        PlayerController.OnJumpPerformed -= HandleJumpPerformed;
        Pickup.OnAnyPickupCollected -= HandlePickupCollected;
    }

    private void Start()
    {
        FindPlayer();
        BeginTutorial();
    }

    private void Update()
    {
        activeStepUpdate?.Invoke();
    }

    private void FindPlayer()
    {
        if (PlayerFinder.Instance != null && PlayerFinder.Instance.Player != null)
        {
            player = PlayerFinder.Instance.Player;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void BeginTutorial()
    {
        movedDistance = 0f;
        jumpsCompleted = 0;
        lookedDistance = 0f;

        if (player != null)
        {
            lastPlayerPosition = player.transform.position;
        }

        SetCurrentStep(TutorialStep.Move);
        UpdateTutorialText();
        onTutorialStarted?.Invoke();
    }

    private void TrackMovementProgress()
    {
        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
            lastPlayerPosition = player.transform.position;
        }

        Vector2 input = InputManager.Instance != null ? InputManager.Instance.Move : Vector2.zero;
        if (input.sqrMagnitude < 0.001f)
        {
            lastPlayerPosition = player.transform.position;
            return;
        }

        Vector3 currentPosition = player.transform.position;
        Vector3 delta = currentPosition - lastPlayerPosition;
        delta.y = 0f;

        movedDistance += delta.magnitude;
        lastPlayerPosition = currentPosition;

        if (movedDistance >= moveDistanceRequired)
        {
            onMovementStepCompleted?.Invoke();
            SetCurrentStep(TutorialStep.Jump);
        }

        UpdateTutorialText();
    }

    private void TrackLookProgress()
    {
        Vector2 lookInput = InputManager.Instance != null ? InputManager.Instance.Look : Vector2.zero;
        lookedDistance += lookInput.magnitude;

        if (lookedDistance >= lookDistanceRequired)
        {
            onLookStepCompleted?.Invoke();
            SetCurrentStep(TutorialStep.KillVisibleEnemy);
            return;
        }

        UpdateTutorialText();
    }

    private void HandleJumpPerformed()
    {
        if (currentStep != TutorialStep.Jump) return;

        jumpsCompleted++;

        if (jumpsCompleted >= jumpsRequired)
        {
            onJumpStepCompleted?.Invoke();
            SetCurrentStep(TutorialStep.Look);
            return;
        }

        UpdateTutorialText();
    }

    private void HandleEnemyKilled(EnemyHealth _)
    {
        if (currentStep == TutorialStep.KillVisibleEnemy)
        {
            onVisibleEnemyStepCompleted?.Invoke();
            SetCurrentStep(TutorialStep.KillInvisibleEnemy);
            return;
        }

        if (currentStep == TutorialStep.KillInvisibleEnemy)
        {
            onInvisibleEnemyStepCompleted?.Invoke();
            SetCurrentStep(TutorialStep.CollectPickup);
            return;
        }
    }

    private void HandlePickupCollected(Pickup _)
    {
        if (currentStep != TutorialStep.CollectPickup) return;

        SetCurrentStep(TutorialStep.Complete);
        onTutorialCompleted?.Invoke();
    }

    private void UpdateTutorialText()
    {
        if (tutorialText == null) return;

        switch (currentStep)
        {
            case TutorialStep.Move:
                tutorialText.text = $"{moveInstruction} ({movedDistance:F1}/{moveDistanceRequired:F1})";
                break;
            case TutorialStep.Jump:
                tutorialText.text = $"{jumpInstruction} ({jumpsCompleted}/{jumpsRequired})";
                break;
            case TutorialStep.Look:
                tutorialText.text = $"{lookInstruction} ({lookedDistance:F0}/{lookDistanceRequired:F0})";
                break;
            case TutorialStep.KillVisibleEnemy:
                tutorialText.text = visibleEnemyInstruction;
                break;
            case TutorialStep.KillInvisibleEnemy:
                tutorialText.text = invisibleEnemyInstruction;
                break;
            case TutorialStep.CollectPickup:
                tutorialText.text = pickupInstruction;
                break;
            case TutorialStep.Complete:
                tutorialText.text = completeInstruction;
                break;
            default:
                tutorialText.text = string.Empty;
                break;
        }
    }

    private void SetCurrentStep(TutorialStep nextStep)
    {
        currentStep = nextStep;
        activeStepUpdate = GetStepUpdater(nextStep);
        UpdateTutorialText();
        InvokeStepStartedEvent(nextStep);
    }

    private void InvokeStepStartedEvent(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.Jump:
                onJumpStepStarted?.Invoke();
                break;
            case TutorialStep.Look:
                onLookStepStarted?.Invoke();
                break;
            case TutorialStep.KillVisibleEnemy:
                onVisibleEnemyStepStarted?.Invoke();
                break;
            case TutorialStep.KillInvisibleEnemy:
                onInvisibleEnemyStepStarted?.Invoke();
                break;
            case TutorialStep.CollectPickup:
                onPickupStepStarted?.Invoke();
                break;
        }
    }

    private Action GetStepUpdater(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.Move:
                return TrackMovementProgress;
            case TutorialStep.Look:
                return TrackLookProgress;
            default:
                return null;
        }
    }
}
