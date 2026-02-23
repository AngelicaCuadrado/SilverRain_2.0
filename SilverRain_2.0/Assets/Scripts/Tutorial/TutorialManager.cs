using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum TutorialStep
{
    None,
    Move,
    KillEnemies,
    Complete
}

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text tutorialText;

    [Header("Objectives")]
    [SerializeField] private float moveDistanceRequired = 8f;
    [SerializeField] private int enemiesToKillRequired = 3;

    [Header("Messages")]
    [SerializeField] private string moveInstruction = "Move using WASD to continue.";
    [SerializeField] private string killInstruction = "Kill enemies to continue.";
    [SerializeField] private string completeInstruction = "Tutorial complete.";

    [Header("Events")]
    [SerializeField] private UnityEvent onTutorialStarted;
    [SerializeField] private UnityEvent onMovementStepCompleted;
    [SerializeField] private UnityEvent onTutorialCompleted;

    private TutorialStep currentStep = TutorialStep.None;
    private GameObject player;
    private Vector3 lastPlayerPosition;
    private float movedDistance;
    private int enemiesKilled;

    private void OnEnable()
    {
        EnemyHealth.OnEnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyKilled -= HandleEnemyKilled;
    }

    private void Start()
    {
        FindPlayer();
        BeginTutorial();
    }

    private void Update()
    {
        if (currentStep == TutorialStep.Move)
        {
            TrackMovementProgress();
        }
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
        enemiesKilled = 0;
        currentStep = TutorialStep.Move;

        if (player != null)
        {
            lastPlayerPosition = player.transform.position;
        }

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
            currentStep = TutorialStep.KillEnemies;
            UpdateTutorialText();
            onMovementStepCompleted?.Invoke();
        }
    }

    private void HandleEnemyKilled(EnemyHealth _)
    {
        if (currentStep != TutorialStep.KillEnemies) return;

        enemiesKilled++;

        if (enemiesKilled >= enemiesToKillRequired)
        {
            currentStep = TutorialStep.Complete;
            UpdateTutorialText();
            onTutorialCompleted?.Invoke();
            return;
        }

        UpdateTutorialText();
    }

    private void UpdateTutorialText()
    {
        if (tutorialText == null) return;

        switch (currentStep)
        {
            case TutorialStep.Move:
                tutorialText.text = $"{moveInstruction} ({movedDistance:F1}/{moveDistanceRequired:F1})";
                break;
            case TutorialStep.KillEnemies:
                tutorialText.text = $"{killInstruction} ({enemiesKilled}/{enemiesToKillRequired})";
                break;
            case TutorialStep.Complete:
                tutorialText.text = completeInstruction;
                break;
            default:
                tutorialText.text = string.Empty;
                break;
        }
    }
}
