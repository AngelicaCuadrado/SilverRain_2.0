using System;
using System.Collections;
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

    [Header("Danger Focus")]
    [SerializeField] private bool focusCameraOnVisibleEnemyStep = true;
    [SerializeField] private Transform visibleEnemyFocusTarget;
    [SerializeField] private float visibleEnemyFocusDuration = 0.8f;
    [SerializeField] private float visibleEnemyHoldDuration = 1.2f;
    [SerializeField] private Vector3 visibleEnemyFocusOffset = new Vector3(0f, 0.8f, 0f);
    [SerializeField, Range(0f, 1f)] private float visibleEnemyUpperBodyFocus = 0.35f;
    [SerializeField] private float visibleEnemyCenteringSpeed = 8f;
    [SerializeField] private float visibleEnemyZoomFov = 60f;
    [SerializeField] private float visibleEnemyZoomInDuration = 0.25f;
    [SerializeField] private float visibleEnemyZoomOutDuration = 0.3f;
    [SerializeField] private float visibleEnemyShakeAmplitude = 0.08f;
    [SerializeField] private float visibleEnemyShakeFrequency = 22f;

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
    private PlayerController playerController;
    private Vector3 lastPlayerPosition;
    private float movedDistance;
    private int jumpsCompleted;
    private float lookedDistance;
    private Action activeStepUpdate;
    private Coroutine visibleEnemyFocusRoutine;

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

        if (visibleEnemyFocusRoutine != null)
        {
            StopCoroutine(visibleEnemyFocusRoutine);
            visibleEnemyFocusRoutine = null;
        }

        PlayerController playerController = player != null ? player.GetComponent<PlayerController>() : null;
        if (playerController != null)
        {
            playerController.SetCanMove(true);
        }

        if (visibleEnemyFocusTarget != null)
        {
            EnemyController focusedEnemyController = visibleEnemyFocusTarget.GetComponent<EnemyController>();
            if (focusedEnemyController != null)
            {
                focusedEnemyController.SetTutorialFrozen(false);
            }
        }
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

        playerController = player != null ? player.GetComponent<PlayerController>() : null;
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

        if (visibleEnemyFocusTarget == null)
        {
            visibleEnemyFocusTarget = FindFocusTarget("MushroomEnemyTutorial");
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
                if (visibleEnemyFocusRoutine != null)
                {
                    StopCoroutine(visibleEnemyFocusRoutine);
                }

                if (focusCameraOnVisibleEnemyStep)
                {
                    visibleEnemyFocusRoutine = StartCoroutine(PlayVisibleEnemyFocusSequence());
                }
                else
                {
                    onVisibleEnemyStepStarted?.Invoke();
                }
                break;
            case TutorialStep.KillInvisibleEnemy:
                onInvisibleEnemyStepStarted?.Invoke();
                break;
            case TutorialStep.CollectPickup:
                onPickupStepStarted?.Invoke();
                break;
        }
    }

    private IEnumerator PlayVisibleEnemyFocusSequence()
    {
        onVisibleEnemyStepStarted?.Invoke();

        PlayerController playerController = player != null ? player.GetComponent<PlayerController>() : null;
        if (playerController == null || visibleEnemyFocusTarget == null || playerController.CameraTransform == null)
        {
            visibleEnemyFocusRoutine = null;
            yield break;
        }

        EnemyController focusedEnemyController = visibleEnemyFocusTarget.GetComponent<EnemyController>();
        if (focusedEnemyController != null)
        {
            focusedEnemyController.SetTutorialFrozen(true);
        }

        playerController.SetCanMove(false);
        playerController.ResetVelocity();

        yield return null;

        Camera playerCamera = playerController.CameraTransform != null
            ? playerController.CameraTransform.GetComponent<Camera>()
            : null;
        float baseFov = playerCamera != null ? playerCamera.fieldOfView : 0f;
        Vector3 baseCameraLocalPosition = playerController.CameraTransform != null
            ? playerController.CameraTransform.localPosition
            : Vector3.zero;

        Vector3 focusPoint = GetVisibleEnemyFocusPoint();
        Vector3 lookDirection = focusPoint - playerController.CameraTransform.position;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            GetLookAnglesToFocusPoint(playerController, focusPoint, out float targetYaw, out float targetPitch);

            float elapsed = 0f;
            float startYaw = playerController.CurrentYaw;
            float startPitch = playerController.CurrentPitch;

            while (elapsed < visibleEnemyFocusDuration)
            {
                elapsed += Time.deltaTime;
                float t = visibleEnemyFocusDuration > 0f ? Mathf.Clamp01(elapsed / visibleEnemyFocusDuration) : 1f;
                float eased = Mathf.SmoothStep(0f, 1f, t);

                float yaw = Mathf.LerpAngle(startYaw, targetYaw, eased);
                float pitch = Mathf.Lerp(startPitch, targetPitch, eased);
                playerController.SetViewAngles(yaw, pitch);

                yield return null;
            }

            playerController.SetViewAngles(targetYaw, targetPitch);
        }

        float holdElapsed = 0f;
        while (holdElapsed < visibleEnemyHoldDuration)
        {
            holdElapsed += Time.deltaTime;
            float holdT = visibleEnemyHoldDuration > 0f ? Mathf.Clamp01(holdElapsed / visibleEnemyHoldDuration) : 1f;

            KeepCameraAimedAtVisibleEnemy(playerController, visibleEnemyCenteringSpeed * 2f);

            if (playerCamera != null)
            {
                float zoomInWeight = visibleEnemyZoomInDuration > 0f
                    ? Mathf.Clamp01(holdElapsed / visibleEnemyZoomInDuration)
                    : 1f;
                float targetZoomFov = Mathf.Max(visibleEnemyZoomFov, 55f);
                playerCamera.fieldOfView = Mathf.Lerp(baseFov, targetZoomFov, Mathf.SmoothStep(0f, 1f, zoomInWeight));
            }

            if (playerController.CameraTransform != null)
            {
                float fade = 1f - holdT;
                float time = Time.time * visibleEnemyShakeFrequency;
                Vector3 shakeOffset = new Vector3(
                    Mathf.Sin(time),
                    Mathf.Cos(time * 1.17f),
                    0f) * (visibleEnemyShakeAmplitude * fade);

                playerController.CameraTransform.localPosition = baseCameraLocalPosition + shakeOffset;
            }

            yield return null;
        }

        float zoomOutElapsed = 0f;
        float currentFov = playerCamera != null ? playerCamera.fieldOfView : baseFov;
        Vector3 currentCameraLocalPosition = playerController.CameraTransform != null
            ? playerController.CameraTransform.localPosition
            : baseCameraLocalPosition;

        while (zoomOutElapsed < visibleEnemyZoomOutDuration)
        {
            zoomOutElapsed += Time.deltaTime;
            float t = visibleEnemyZoomOutDuration > 0f ? Mathf.Clamp01(zoomOutElapsed / visibleEnemyZoomOutDuration) : 1f;
            float eased = Mathf.SmoothStep(0f, 1f, t);

            if (playerCamera != null)
            {
                playerCamera.fieldOfView = Mathf.Lerp(currentFov, baseFov, eased);
            }

            if (playerController.CameraTransform != null)
            {
                playerController.CameraTransform.localPosition = Vector3.Lerp(currentCameraLocalPosition, baseCameraLocalPosition, eased);
            }

            yield return null;
        }

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = baseFov;
        }

        if (playerController.CameraTransform != null)
        {
            playerController.CameraTransform.localPosition = baseCameraLocalPosition;
        }

        if (focusedEnemyController != null)
        {
            focusedEnemyController.SetTutorialFrozen(false);
        }

        playerController.SetCanMove(true);
        visibleEnemyFocusRoutine = null;
    }

    private void KeepCameraAimedAtVisibleEnemy(PlayerController controller, float centeringSpeed)
    {
        Vector3 focusPoint = GetVisibleEnemyFocusPoint();
        Vector3 lookDirection = focusPoint - controller.CameraTransform.position;
        if (lookDirection.sqrMagnitude <= 0.001f) return;

        GetLookAnglesToFocusPoint(controller, focusPoint, out float targetYaw, out float targetPitch);

        float t = 1f - Mathf.Exp(-centeringSpeed * Time.deltaTime);
        float yaw = Mathf.LerpAngle(controller.CurrentYaw, targetYaw, t);
        float pitch = Mathf.Lerp(controller.CurrentPitch, targetPitch, t);
        controller.SetViewAngles(yaw, pitch);
    }

    private Vector3 GetVisibleEnemyFocusPoint()
    {
        if (visibleEnemyFocusTarget == null)
        {
            return Vector3.zero;
        }

        Bounds bounds;
        if (!TryGetFocusTargetBounds(out bounds))
        {
            return visibleEnemyFocusTarget.position + visibleEnemyFocusOffset;
        }

        Vector3 upperBodyPoint = bounds.center + Vector3.up * (bounds.extents.y * visibleEnemyUpperBodyFocus);
        return upperBodyPoint + visibleEnemyFocusOffset;
    }

    private bool TryGetFocusTargetBounds(out Bounds bounds)
    {
        Transform focusRoot = GetVisibleEnemyFocusRoot();
        Collider[] colliders = focusRoot.GetComponentsInChildren<Collider>();
        bool hasBounds = false;
        bounds = default;

        foreach (Collider col in colliders)
        {
            if (col == null || !col.enabled || col.isTrigger) continue;

            if (!hasBounds)
            {
                bounds = col.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(col.bounds);
            }
        }

        if (hasBounds)
        {
            return true;
        }

        Renderer[] renderers = focusRoot.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null || !renderer.enabled) continue;
            if (renderer is ParticleSystemRenderer || renderer is TrailRenderer || renderer is LineRenderer) continue;

            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        return hasBounds;
    }

    private Transform GetVisibleEnemyFocusRoot()
    {
        EnemyHealth enemyHealth = visibleEnemyFocusTarget.GetComponentInChildren<EnemyHealth>();
        if (enemyHealth != null)
        {
            return enemyHealth.transform;
        }

        EnemyController enemyController = visibleEnemyFocusTarget.GetComponentInChildren<EnemyController>();
        if (enemyController != null)
        {
            return enemyController.transform;
        }

        return visibleEnemyFocusTarget;
    }

    private void GetLookAnglesToFocusPoint(PlayerController controller, Vector3 focusPoint, out float yaw, out float pitch)
    {
        Vector3 lookDirection = focusPoint - controller.CameraTransform.position;
        Vector3 flatLookDirection = Vector3.ProjectOnPlane(lookDirection, Vector3.up);

        yaw = flatLookDirection.sqrMagnitude > 0.001f
            ? Quaternion.LookRotation(flatLookDirection, Vector3.up).eulerAngles.y
            : controller.CurrentYaw;

        float distanceXZ = new Vector2(lookDirection.x, lookDirection.z).magnitude;
        pitch = -Mathf.Atan2(lookDirection.y, distanceXZ) * Mathf.Rad2Deg;
    }

    private Transform FindFocusTarget(string objectName)
    {
        GameObject found = GameObject.Find(objectName);
        return found != null ? found.transform : null;
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
