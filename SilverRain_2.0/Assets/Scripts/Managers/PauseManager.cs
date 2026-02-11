using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public UIWindow PauseWindow;
    
    // Each Acquire() returns a unique token. We store them in a set.
    private readonly HashSet<object> _tokens = new();

    private bool IsPaused => _tokens.Count > 0;
    
    private void Start()
    {
        Instance = this;
    }

    public object Acquire(string reason)
    {
        var token = new PauseToken(reason);
        _tokens.Add(token);
        Apply();
        return token;
    }

    public void Release(object token)
    {
        if (token == null) return;

        _tokens.Remove(token);
        Apply();
    }
    
    private void Apply()
    {
        Time.timeScale = IsPaused ? 0f : 1f;
    }
    
    private class PauseToken
    {
        public readonly string Reason;
        public PauseToken(string reason) => Reason = reason;
    }
}
