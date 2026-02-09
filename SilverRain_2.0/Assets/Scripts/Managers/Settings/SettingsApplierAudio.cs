public static class SettingsApplierAudio
{
    private const string MasterParam = "MasterVol";
    private const string BgmParam = "BGMVol";
    private const string SfxParam = "SFXVol";

    public static void Apply(AudioSettings a)
    {
        var audio = AudioManager.Instance;
        if (audio == null) return;

        // Apply mute by forcing volume to 0
        var master = a.masterMuted ? 0f : a.master;
        var bgm = a.bgmMuted ? 0f : a.bgm;
        var sfx = a.sfxMuted ? 0f : a.sfx;

        audio.SetMixerVolume(MasterParam, master);
        audio.SetMixerVolume(BgmParam, bgm);
        audio.SetMixerVolume(SfxParam, sfx);
    }
}