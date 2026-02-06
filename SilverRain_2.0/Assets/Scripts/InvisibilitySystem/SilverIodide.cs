using UnityEngine;

public class SilverIodide : MonoBehaviour
{
    [SerializeField]
    private float revealTime = 30f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Pick Up Silver");
            EnemyEvents.OnGlobalReveal?.Invoke(revealTime);
            GlobalInvisibilityManager.Instance.SetTimer(revealTime);

            Destroy(gameObject);
        }
    }

}
