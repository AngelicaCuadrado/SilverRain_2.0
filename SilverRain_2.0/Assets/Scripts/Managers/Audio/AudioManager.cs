using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Required References")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioLibrary library;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    private AudioSource _bgm;
    private AudioSource _sfx;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        switch (sceneName)
        {
            case "Level_Menu":
                PlayBGM("bgm_main");
                break;
            case "Level1":
                PlayBGM("bgm_level1");
                break;
            case "Level2":
                PlayBGM("bgm_level2");
                break;
            case "Level3":
                PlayBGM("bgm_level3");
                break;
        }
    }
    private void InitializeAudioSources()
    {
        if (GetComponent<AudioListener>() == null)
        {
            gameObject.AddComponent<AudioListener>();
        }

        _bgm = gameObject.AddComponent<AudioSource>();
        _bgm.loop = true;
        _bgm.playOnAwake = false;
        _bgm.outputAudioMixerGroup = bgmGroup;
        _bgm.ignoreListenerPause = true;

        _sfx = gameObject.AddComponent<AudioSource>();
        _sfx.playOnAwake = false;
        _sfx.outputAudioMixerGroup = sfxGroup;
        _sfx.ignoreListenerPause = true;
    }

    public void PlayBGM(string id)
    {
        var clip = library.Find(id);
        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] BGM clip not found: {id}");
            return;
        }

        if (_bgm.isPlaying)
        {
            if (_bgm.clip == clip) return;
            _bgm.Stop();
        }

        _bgm.clip = clip;
        _bgm.Play();
    }

    public void StopBGM() => _bgm.Stop();

    public void PlaySFX(string id)
    {
        var clip = library.Find(id);
        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] SFX clip not found: {id}");
            return;
        }
        _sfx.PlayOneShot(clip);
    }

    public void SetMixerVolume(string exposedParam, float volume01)
    {
        volume01 = Mathf.Clamp01(volume01);

        float db = (volume01 <= 0.0001f) ? -80f : Mathf.Log10(volume01) * 20f;

        if (!mixer.SetFloat(exposedParam, db))
            Debug.LogWarning($"[AudioManager] Mixer param not found: {exposedParam}");
    }

    public bool TryGetMixerDb(string exposedParam, out float db)
    {
        return mixer.GetFloat(exposedParam, out db);
    }

    public AudioMixer Mixer => mixer;
}