using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : UIWindow
{
    public enum SettingsTab { Audio, Video }
    
    [Header("Tabs")]
    [SerializeField] private Button tabAudioButton;
    [SerializeField] private Button tabVideoButton;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;

    [Header("Common")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button cancelButton;

    [Header("Audio UI")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Toggle masterMuteToggle;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Toggle bgmMuteToggle;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle sfxMuteToggle;

    [Header("Video UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    [Header("Video Confirm Dialog (Optional)")]
    [Tooltip("If assigned, shows a confirmation dialog when video settings change")]
    [SerializeField] private GameObject confirmDialog;
    [SerializeField] private Button confirmKeepButton;
    [SerializeField] private Button confirmRevertButton;
    [SerializeField] private TMP_Text confirmTimerText;
    [SerializeField] private float confirmTimeout = 10f;
    
    // Snapshot for Cancel
        private string _snapshotJson;

        // Video safe revert
        private string _confirmedVideoJson;
        private bool _videoConfirmOpen;
        private float _confirmTimer;

        // Resolution dropdown cache
        private readonly List<(int w, int h)> _resOptions = new();

        // Prevent UI callbacks during sync
        private bool _ignoreEvents;

        private SettingsManager Settings => SettingsManager.Instance;

        /// <summary>
        /// Called when the window should close. Override or assign to integrate with your UIManager.
        /// </summary>
        public System.Action OnCloseWindow;

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private void Update()
        {
            UpdateConfirmTimer();
        }

        private void Initialize()
        {
            // Save baseline snapshot
            _snapshotJson = Settings.ExportJsonSnapshot();
            _confirmedVideoJson = JsonUtility.ToJson(Settings.Data.video);
            _videoConfirmOpen = false;

            BuildDropdowns();
            BindUIEvents();
            SyncUIFromSettings();
            ShowTab(SettingsTab.Audio);

            if (confirmDialog != null)
                confirmDialog.SetActive(false);
        }

        private void Cleanup()
        {
            UnbindUIEvents();
        }

        #region UI Events

        private void BindUIEvents()
        {
            // Tabs
            tabAudioButton?.onClick.AddListener(() => ShowTab(SettingsTab.Audio));
            tabVideoButton?.onClick.AddListener(() => ShowTab(SettingsTab.Video));

            // Common
            backButton?.onClick.AddListener(Back);
            applyButton?.onClick.AddListener(Apply);
            cancelButton?.onClick.AddListener(Cancel);

            // Audio
            masterSlider?.onValueChanged.AddListener(OnMasterChanged);
            bgmSlider?.onValueChanged.AddListener(OnBgmChanged);
            sfxSlider?.onValueChanged.AddListener(OnSfxChanged);
            masterMuteToggle?.onValueChanged.AddListener(OnMasterMuteChanged);
            bgmMuteToggle?.onValueChanged.AddListener(OnBgmMuteChanged);
            sfxMuteToggle?.onValueChanged.AddListener(OnSfxMuteChanged);

            // Video
            resolutionDropdown?.onValueChanged.AddListener(OnResolutionChanged);
            fullscreenToggle?.onValueChanged.AddListener(OnFullscreenChanged);
            qualityDropdown?.onValueChanged.AddListener(OnQualityChanged);

            // Confirm dialog
            confirmKeepButton?.onClick.AddListener(OnConfirmKeep);
            confirmRevertButton?.onClick.AddListener(OnConfirmRevert);
        }

        private void UnbindUIEvents()
        {
            tabAudioButton?.onClick.RemoveAllListeners();
            tabVideoButton?.onClick.RemoveAllListeners();

            backButton?.onClick.RemoveListener(Back);
            applyButton?.onClick.RemoveListener(Apply);
            cancelButton?.onClick.RemoveListener(Cancel);

            masterSlider?.onValueChanged.RemoveListener(OnMasterChanged);
            bgmSlider?.onValueChanged.RemoveListener(OnBgmChanged);
            sfxSlider?.onValueChanged.RemoveListener(OnSfxChanged);
            masterMuteToggle?.onValueChanged.RemoveListener(OnMasterMuteChanged);
            bgmMuteToggle?.onValueChanged.RemoveListener(OnBgmMuteChanged);
            sfxMuteToggle?.onValueChanged.RemoveListener(OnSfxMuteChanged);

            resolutionDropdown?.onValueChanged.RemoveListener(OnResolutionChanged);
            fullscreenToggle?.onValueChanged.RemoveListener(OnFullscreenChanged);
            qualityDropdown?.onValueChanged.RemoveListener(OnQualityChanged);

            confirmKeepButton?.onClick.RemoveListener(OnConfirmKeep);
            confirmRevertButton?.onClick.RemoveListener(OnConfirmRevert);
        }

        #endregion

        #region Tabs

        private void ShowTab(SettingsTab tab)
        {
            audioPanel?.SetActive(tab == SettingsTab.Audio);
            videoPanel?.SetActive(tab == SettingsTab.Video);
        }

        #endregion

        #region UI Sync

        private void SyncUIFromSettings()
        {
            _ignoreEvents = true;

            var a = Settings.Data.audio;
            masterSlider?.SetValueWithoutNotify(a.master);
            bgmSlider?.SetValueWithoutNotify(a.bgm);
            sfxSlider?.SetValueWithoutNotify(a.sfx);
            masterMuteToggle?.SetIsOnWithoutNotify(a.masterMuted);
            bgmMuteToggle?.SetIsOnWithoutNotify(a.bgmMuted);
            sfxMuteToggle?.SetIsOnWithoutNotify(a.sfxMuted);

            var v = Settings.Data.video;
            fullscreenToggle?.SetIsOnWithoutNotify(v.fullscreen);

            int resIndex = FindResolutionIndex(v.width, v.height);
            if (resIndex < 0) resIndex = 0;
            resolutionDropdown?.SetValueWithoutNotify(resIndex);

            int q = Mathf.Clamp(v.qualityIndex, 0, Mathf.Max(0, qualityDropdown != null ? qualityDropdown.options.Count - 1 : 0));
            qualityDropdown?.SetValueWithoutNotify(q);

            _ignoreEvents = false;
        }

        #endregion

        #region Audio Handlers

        private void OnMasterChanged(float v)
        {
            if (_ignoreEvents) return;
            Settings.Data.audio.master = v;
            Settings.MarkDirty();
            Settings.ApplyAll();
        }

        private void OnBgmChanged(float v)
        {
            if (_ignoreEvents) return;
            Settings.Data.audio.bgm = v;
            Settings.MarkDirty();
            Settings.ApplyAll();
        }

        private void OnSfxChanged(float v)
        {
            if (_ignoreEvents) return;
            Settings.Data.audio.sfx = v;
            Settings.MarkDirty();
            Settings.ApplyAll();
        }

        private void OnMasterMuteChanged(bool on)
        {
            if (_ignoreEvents) return;
            Settings.Data.audio.masterMuted = on;
            Settings.MarkDirty();
            Settings.ApplyAll();
        }

        private void OnBgmMuteChanged(bool on)
        {
            if (_ignoreEvents) return;
            Settings.Data.audio.bgmMuted = on;
            Settings.MarkDirty();
            Settings.ApplyAll();
        }

        private void OnSfxMuteChanged(bool on)
        {
            if (_ignoreEvents) return;
            Settings.Data.audio.sfxMuted = on;
            Settings.MarkDirty();
            Settings.ApplyAll();
        }

        #endregion

        #region Video Handlers

        private void OnResolutionChanged(int index)
        {
            if (_ignoreEvents) return;
            if (index < 0 || index >= _resOptions.Count) return;

            BeginVideoPreview();

            var (w, h) = _resOptions[index];
            Settings.Data.video.width = w;
            Settings.Data.video.height = h;
            Settings.MarkDirty();
            Settings.ApplyAll();
        }

        private void OnFullscreenChanged(bool on)
        {
            if (_ignoreEvents) return;
            BeginVideoPreview();

            Settings.Data.video.fullscreen = on;
            Settings.MarkDirty();
            Settings.ApplyAll();
        }

        private void OnQualityChanged(int index)
        {
            if (_ignoreEvents) return;
            BeginVideoPreview();

            Settings.Data.video.qualityIndex = index;
            Settings.MarkDirty();
            Settings.ApplyAll();
        }

        private void BeginVideoPreview()
        {
            if (_videoConfirmOpen) return;
            if (confirmDialog == null) return; // No confirm dialog assigned, skip

            _videoConfirmOpen = true;
            _confirmTimer = confirmTimeout;
            confirmDialog.SetActive(true);
            UpdateTimerText();
        }

        private void UpdateConfirmTimer()
        {
            if (!_videoConfirmOpen) return;

            _confirmTimer -= Time.unscaledDeltaTime;
            UpdateTimerText();

            if (_confirmTimer <= 0f)
            {
                OnConfirmRevert();
            }
        }

        private void UpdateTimerText()
        {
            if (confirmTimerText != null)
                confirmTimerText.text = $"Reverting in {Mathf.CeilToInt(_confirmTimer)}s...";
        }

        private void OnConfirmKeep()
        {
            _confirmedVideoJson = JsonUtility.ToJson(Settings.Data.video);
            _videoConfirmOpen = false;
            confirmDialog?.SetActive(false);
        }

        private void OnConfirmRevert()
        {
            ApplyConfirmedVideoSnapshot();
            _videoConfirmOpen = false;
            confirmDialog?.SetActive(false);
        }

        private void ApplyConfirmedVideoSnapshot()
        {
            if (string.IsNullOrEmpty(_confirmedVideoJson)) return;

            var v = JsonUtility.FromJson<VideoSettings>(_confirmedVideoJson);
            if (v == null) return;

            Settings.Data.video = v;
            Settings.ApplyAll();
            SyncUIFromSettings();
        }

        #endregion

        #region Dropdown Building

        private void BuildDropdowns()
        {
            BuildResolutionOptions();
            BuildQualityOptions();
        }

        private void BuildResolutionOptions()
        {
            _resOptions.Clear();
            if (resolutionDropdown == null) return;

            resolutionDropdown.ClearOptions();

            var set = new HashSet<(int, int)>();
            var list = new List<(int w, int h)>();

            foreach (var r in Screen.resolutions)
            {
                var key = (r.width, r.height);
                if (set.Add(key))
                    list.Add(key);
            }

            // Fallback
            if (list.Count == 0)
            {
                list.Add((1920, 1080));
                list.Add((1600, 900));
                list.Add((1280, 720));
            }

            list.Sort((a, b) => (a.w * a.h).CompareTo(b.w * b.h));
            _resOptions.AddRange(list);

            var options = new List<string>(_resOptions.Count);
            foreach (var res in _resOptions)
                options.Add($"{res.w} x {res.h}");

            resolutionDropdown.AddOptions(options);
        }

        private int FindResolutionIndex(int w, int h)
        {
            for (int i = 0; i < _resOptions.Count; i++)
                if (_resOptions[i].w == w && _resOptions[i].h == h) return i;
            return -1;
        }

        private void BuildQualityOptions()
        {
            if (qualityDropdown == null) return;

            qualityDropdown.ClearOptions();
            var names = QualitySettings.names;
            var options = new List<string>(names);
            qualityDropdown.AddOptions(options);
        }

        #endregion

        #region Common Actions

        private void Back()
        {
            Cancel();
            UIManager.Instance.Pop();
        }

        private void Apply()
        {
            Settings.MarkDirty();
            Settings.Save();
            _snapshotJson = Settings.ExportJsonSnapshot();
            Debug.Log("[SettingsWindow] Settings applied");
        }

        private void Cancel()
        {
            Settings.ImportJsonSnapshot(_snapshotJson, false);
            SyncUIFromSettings();
        }
        
        #endregion
}
