using UnityEngine;
using UnityEngine.Playables;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    private bool _isInitialized;

    private void Awake()
    {
        EnsureInitialized();
    }

    private void OnDestroy()
    {
        if (director != null && _isInitialized)
            director.stopped -= OnDirectorStopped;
    }

    public void PlayTimeline()
    {
        if (director == null)
            return;

        EnsureInitialized();
        director.Stop();
        director.time = 0;
        director.Play();
    }

    public void StopTimeline()
    {
        if (director == null)
            return;

        EnsureInitialized();
        director.Stop();
        director.time = 0;
    }

    private void OnDirectorStopped(PlayableDirector stoppedDirector)
    {
        if (stoppedDirector != director)
            return;

        // Keep the final timeline state instead of looping back to the start.
        director.extrapolationMode = DirectorWrapMode.Hold;
    }

    private void EnsureInitialized()
    {
        if (_isInitialized || director == null)
            return;

        director.playOnAwake = false;
        director.extrapolationMode = DirectorWrapMode.Hold;
        director.stopped += OnDirectorStopped;
        _isInitialized = true;
    }
}
