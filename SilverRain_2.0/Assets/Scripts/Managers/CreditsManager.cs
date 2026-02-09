using UnityEngine;
using UnityEngine.Playables;

public class CreditsManager_Timeline : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    public void PlayTimeline()
    {
        director.time = 0;
        director.Play();
    }

    public void StopTimeline()
    {
        director.Stop();
    }
}
