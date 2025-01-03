using UnityEngine;
using UnityEngine.UI;
using TMPro;  // <-- IMPORTANT for TMP_Dropdown
using System.Collections.Generic;

public class SettingController : MonoBehaviour
{
    [Header("UI References")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;  // Use TMP_Dropdown instead of Dropdown
    public Slider volumeSlider;

    // Store available resolutions
    private Resolution[] resolutions;

    // PlayerPrefs keys
    private const string PrefKey_Fullscreen = "Pref_Fullscreen";
    private const string PrefKey_Resolution = "Pref_Resolution";
    private const string PrefKey_Volume     = "Pref_Volume";

    void Awake()
    {
        // Get a list of system-supported resolutions
        resolutions = Screen.resolutions;

        // Build the dropdown from these resolutions
        SetupResolutionDropdown();

        // Load any previously saved settings from PlayerPrefs
        LoadSettings();

        // Reflect those loaded settings in the UI
        ApplySettingsToUI();

        // Apply them to the game screen & audio
        ApplySettingsToGame();
    }

    void Start()
    {
        // Subscribe to events so changes in UI automatically save settings
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    /// <summary>
    /// Build the resolution dropdown options using available screen resolutions.
    /// </summary>
    private void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (var res in resolutions)
        {
            options.Add($"{res.width} x {res.height}");
        }

        // TMP_Dropdown allows adding string options directly
        resolutionDropdown.AddOptions(options);
    }

    /// <summary>
    /// Load saved settings from PlayerPrefs (or create default entries if none exist).
    /// </summary>
    private void LoadSettings()
    {
        // Fullscreen
        if (PlayerPrefs.HasKey(PrefKey_Fullscreen))
        {
            bool isFullscreen = PlayerPrefs.GetInt(PrefKey_Fullscreen, 1) == 1;
            Screen.fullScreen = isFullscreen;
        }
        else
        {
            // If there's no saved setting, store the current screen's state as default
            PlayerPrefs.SetInt(PrefKey_Fullscreen, Screen.fullScreen ? 1 : 0);
        }

        // Resolution
        if (PlayerPrefs.HasKey(PrefKey_Resolution))
        {
            int savedIndex = PlayerPrefs.GetInt(PrefKey_Resolution, 0);
            // Safety check for out-of-range
            if (savedIndex < 0 || savedIndex >= resolutions.Length)
                savedIndex = 0;
            Resolution savedRes = resolutions[savedIndex];
            Screen.SetResolution(savedRes.width, savedRes.height, Screen.fullScreen);
        }
        else
        {
            // If none is saved, store the current resolution index as default
            int currentResIndex = GetCurrentResolutionIndex();
            PlayerPrefs.SetInt(PrefKey_Resolution, currentResIndex);
        }

        // Volume
        if (PlayerPrefs.HasKey(PrefKey_Volume))
        {
            float savedVolume = PlayerPrefs.GetFloat(PrefKey_Volume, 1f);
            AudioListener.volume = savedVolume;
        }
        else
        {
            // No saved value, store the current global volume
            PlayerPrefs.SetFloat(PrefKey_Volume, AudioListener.volume);
        }

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Update the UI elements (Toggle, TMP_Dropdown, Slider) to match the loaded settings.
    /// </summary>
    private void ApplySettingsToUI()
    {
        // Fullscreen toggle
        fullscreenToggle.isOn = Screen.fullScreen;

        // Resolution dropdown
        resolutionDropdown.value = GetCurrentResolutionIndex();
        resolutionDropdown.RefreshShownValue();

        // Volume slider
        volumeSlider.value = AudioListener.volume;
    }

    /// <summary>
    /// Apply the current PlayerPrefs-saved settings directly to the game.
    /// </summary>
    private void ApplySettingsToGame()
    {
        // Fullscreen
        bool isFullscreen = (PlayerPrefs.GetInt(PrefKey_Fullscreen, 1) == 1);
        Screen.fullScreen = isFullscreen;

        // Resolution
        int savedResIndex = PlayerPrefs.GetInt(PrefKey_Resolution, 0);
        if (savedResIndex < 0 || savedResIndex >= resolutions.Length)
            savedResIndex = 0;
        Resolution resToSet = resolutions[savedResIndex];
        Screen.SetResolution(resToSet.width, resToSet.height, isFullscreen);

        // Volume
        float volume = PlayerPrefs.GetFloat(PrefKey_Volume, 1f);
        AudioListener.volume = volume;
    }

    /// <summary>
    /// Detects changes to the fullscreen toggle and saves them.
    /// </summary>
    private void OnFullscreenChanged(bool isOn)
    {
        Screen.fullScreen = isOn;
        PlayerPrefs.SetInt(PrefKey_Fullscreen, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Detects changes to the resolution dropdown and saves them.
    /// </summary>
    private void OnResolutionChanged(int index)
    {
        if (index < 0 || index >= resolutions.Length) return;

        Resolution chosen = resolutions[index];
        Screen.SetResolution(chosen.width, chosen.height, Screen.fullScreen);
        PlayerPrefs.SetInt(PrefKey_Resolution, index);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Detects changes to the volume slider and saves them.
    /// </summary>
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(PrefKey_Volume, value);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Finds which resolution in our list matches the current screen resolution.
    /// </summary>
    private int GetCurrentResolutionIndex()
    {
        Resolution currentRes = Screen.currentResolution;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == currentRes.width &&
                resolutions[i].height == currentRes.height)
            {
                return i;
            }
        }
        return 0; // Default to index 0 if not found
    }
}
