using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio (using AudioSources)")]
    public AudioSource musicSource; // assign your music AudioSource
    public AudioSource sfxSource;   // assign your sfx AudioSource
    public Slider masterSlider;     // 0..1
    public Slider musicSlider;      // 0..1
    public Slider sfxSlider;        // 0..1

    [Header("Video (TMP Dropdowns)")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;

    // PlayerPrefs keys
    const string PREF_MASTER = "pref_master";
    const string PREF_MUSIC = "pref_music";
    const string PREF_SFX = "pref_sfx";
    const string PREF_RESOLUTION = "pref_resolution_index";
    const string PREF_QUALITY = "pref_quality_index";
    const string PREF_FULLSCREEN = "pref_fullscreen";

    Resolution[] resolutions;

    void Awake()
    {
        PopulateResolutions();
        PopulateQuality();
        HookupUIEvents();
        LoadSettings();
    }

    void PopulateResolutions()
    {
        // Get distinct resolutions (width x height x refresh)
        resolutions = Screen.resolutions
            .Select(r => new Resolution { width = r.width, height = r.height, refreshRate = r.refreshRate })
            .Distinct()
            .ToArray();

        if (resolutionDropdown == null) return;

        resolutionDropdown.ClearOptions();
        List<string> options = resolutions.Select(r => $"{r.width} x {r.height} @{r.refreshRate}Hz").ToList();
        resolutionDropdown.AddOptions(options);

        // select current resolution index if found
        int currentIndex = Array.FindIndex(resolutions, r => r.width == Screen.width && r.height == Screen.height && r.refreshRate == Screen.currentResolution.refreshRate);
        if (currentIndex < 0) currentIndex = Mathf.Clamp(resolutions.Length - 1, 0, resolutions.Length - 1);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void PopulateQuality()
    {
        if (qualityDropdown == null) return;
        qualityDropdown.ClearOptions();
        List<string> names = QualitySettings.names.ToList();
        qualityDropdown.AddOptions(names);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    void HookupUIEvents()
    {
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSfxVolume);

        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.AddListener(SetResolutionByIndex);
        if (qualityDropdown != null) qualityDropdown.onValueChanged.AddListener(SetQuality);
        if (fullscreenToggle != null) fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    #region Audio setters (using AudioSource.volume 0..1)
    // Master multiplies per-channel sliders. Slider values are 0..1 (linear).
    public void SetMasterVolume(float masterValue)
    {
        PlayerPrefs.SetFloat(PREF_MASTER, masterValue);
        ApplyAudioVolumes();
    }

    public void SetMusicVolume(float musicValue)
    {
        PlayerPrefs.SetFloat(PREF_MUSIC, musicValue);
        ApplyAudioVolumes();
    }

    public AudioClip sfxPreviewClip;    // assign a short SFX preview in inspector
    private float lastSfxPreviewTime = 0f;
    public float sfxPreviewCooldown = 0.1f; // seconds, prevents spamming

    // modify SetSfxVolume or add this helper near ApplyAudioVolumes:

    public void SetSfxVolume(float sfxValue)
    {
        PlayerPrefs.SetFloat(PREF_SFX, sfxValue);
        ApplyAudioVolumes();

        // play preview (but limit frequency so it doesn't spam)
        if (sfxPreviewClip != null && sfxSource != null)
        {
            if (Time.unscaledTime - lastSfxPreviewTime > sfxPreviewCooldown)
            {
                sfxSource.PlayOneShot(sfxPreviewClip, Mathf.Clamp01((masterSlider != null ? masterSlider.value : 1f) * sfxValue));
                lastSfxPreviewTime = Time.unscaledTime;
            }
        }
    }

    // Applies current slider values to the AudioSources
    void ApplyAudioVolumes()
    {
        float master = masterSlider != null ? masterSlider.value : 1f;
        float music = musicSlider != null ? musicSlider.value : 1f;
        float sfx = sfxSlider != null ? sfxSlider.value : 1f;

        if (musicSource != null) musicSource.volume = Mathf.Clamp01(master * music);
        if (sfxSource != null) sfxSource.volume = Mathf.Clamp01(master * sfx);
    }
    #endregion

    #region Video setters
    public void SetResolutionByIndex(int index)
    {
        if (resolutions == null || resolutions.Length == 0) return;
        if (index < 0 || index >= resolutions.Length) return;

        Resolution r = resolutions[index];
        bool isFull = fullscreenToggle != null ? fullscreenToggle.isOn : Screen.fullScreen;
        Screen.SetResolution(r.width, r.height, isFull, r.refreshRate);
        PlayerPrefs.SetInt(PREF_RESOLUTION, index);
    }

    public void SetQuality(int index)
    {
        index = Mathf.Clamp(index, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt(PREF_QUALITY, index);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(PREF_FULLSCREEN, isFullscreen ? 1 : 0);
    }
    #endregion

    public void ApplyAll()
    {
        // call these to ensure everything is applied & saved
        PlayerPrefs.SetFloat(PREF_MASTER, masterSlider != null ? masterSlider.value : 1f);
        PlayerPrefs.SetFloat(PREF_MUSIC, musicSlider != null ? musicSlider.value : 1f);
        PlayerPrefs.SetFloat(PREF_SFX, sfxSlider != null ? sfxSlider.value : 1f);
        PlayerPrefs.SetInt(PREF_QUALITY, qualityDropdown != null ? qualityDropdown.value : QualitySettings.GetQualityLevel());
        PlayerPrefs.SetInt(PREF_RESOLUTION, resolutionDropdown != null ? resolutionDropdown.value : 0);
        PlayerPrefs.SetInt(PREF_FULLSCREEN, fullscreenToggle != null && fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

        // Apply immediately
        ApplyAudioVolumes();
        SetQuality(qualityDropdown != null ? qualityDropdown.value : QualitySettings.GetQualityLevel());
        SetResolutionByIndex(resolutionDropdown != null ? resolutionDropdown.value : 0);
        SetFullscreen(fullscreenToggle != null ? fullscreenToggle.isOn : Screen.fullScreen);
    }

    public void LoadSettings()
    {
        // Audio defaults
        float master = PlayerPrefs.HasKey(PREF_MASTER) ? PlayerPrefs.GetFloat(PREF_MASTER) : 0.75f;
        float music = PlayerPrefs.HasKey(PREF_MUSIC) ? PlayerPrefs.GetFloat(PREF_MUSIC) : 0.75f;
        float sfx = PlayerPrefs.HasKey(PREF_SFX) ? PlayerPrefs.GetFloat(PREF_SFX) : 0.75f;

        if (masterSlider != null) masterSlider.value = master;
        if (musicSlider != null) musicSlider.value = music;
        if (sfxSlider != null) sfxSlider.value = sfx;
        ApplyAudioVolumes();

        // Video
        if (resolutionDropdown != null && resolutions != null && resolutions.Length > 0)
        {
            int resIndex = PlayerPrefs.HasKey(PREF_RESOLUTION) ? PlayerPrefs.GetInt(PREF_RESOLUTION) : resolutionDropdown.value;
            resIndex = Mathf.Clamp(resIndex, 0, resolutions.Length - 1);
            resolutionDropdown.value = resIndex;
            resolutionDropdown.RefreshShownValue();
            SetResolutionByIndex(resIndex);
        }

        if (qualityDropdown != null)
        {
            int qualityIndex = PlayerPrefs.HasKey(PREF_QUALITY) ? PlayerPrefs.GetInt(PREF_QUALITY) : QualitySettings.GetQualityLevel();
            qualityIndex = Mathf.Clamp(qualityIndex, 0, QualitySettings.names.Length - 1);
            qualityDropdown.value = qualityIndex;
            qualityDropdown.RefreshShownValue();
            SetQuality(qualityIndex);
        }

        bool full = PlayerPrefs.HasKey(PREF_FULLSCREEN) ? PlayerPrefs.GetInt(PREF_FULLSCREEN) == 1 : Screen.fullScreen;
        if (fullscreenToggle != null) fullscreenToggle.isOn = full;
        SetFullscreen(full);
    }

    // Optional: restore defaults
    public void SetDefaults()
    {
        float defaultVal = 0.75f;
        if (masterSlider != null) masterSlider.value = defaultVal;
        if (musicSlider != null) musicSlider.value = defaultVal;
        if (sfxSlider != null) sfxSlider.value = defaultVal;

        if (qualityDropdown != null) qualityDropdown.value = QualitySettings.names.Length - 1;
        if (resolutionDropdown != null && resolutions != null && resolutions.Length > 0) resolutionDropdown.value = resolutions.Length - 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = true;

        ApplyAll();
    }
}
