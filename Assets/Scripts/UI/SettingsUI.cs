using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] GameObject fullscreenCheckmark;
    [SerializeField] GameObject vibrationCheckmark;
    [SerializeField] GameObject fpsCheckmark;
    [SerializeField] GameObject fpsCanvas;
    [SerializeField] AudioMixer mixer;
    [SerializeField] SplitSlider master, music, sfx;
    Resolution[] _resolutions;

    public const string
        MasterParam = "MasterVolume",
        MusicParam = "MusicVolume",
        SfxParam = "SFXVolume",
        VibrationParam = "Vibration",
        FPSParam = "ShowFPS"
        ;

    public void Init()
    {
        var currentResolutionIndex = 0;
        _resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        resolutionDropdown.ClearOptions();

        var options = new List<string>();

        for (var i = 0; i < _resolutions.Length; i++)
        {
            var option = $"{_resolutions[i].width} x {_resolutions[i].height}";
            options.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        fullscreenCheckmark.SetActive(Screen.fullScreen);
        
        master.onValueChange += v => mixer.SetFloat(MasterParam, Mathf.Log10(v) * 30f);
        music.onValueChange += v => mixer.SetFloat(MusicParam, Mathf.Log10(v) * 30f);
        sfx.onValueChange += v => mixer.SetFloat(SfxParam, Mathf.Log10(v) * 30f);

        var masterV = PlayerPrefs.HasKey(MasterParam) ? PlayerPrefs.GetFloat(MasterParam) : 1f;
        var musicV = PlayerPrefs.HasKey(MusicParam) ? PlayerPrefs.GetFloat(MusicParam) : 0.69f;
        var sfxV = PlayerPrefs.HasKey(SfxParam) ? PlayerPrefs.GetFloat(SfxParam) : 1f;
        
        mixer.SetFloat(MasterParam, Mathf.Log10(masterV) * 30f);
        mixer.SetFloat(MusicParam, Mathf.Log10(musicV) * 30f);
        mixer.SetFloat(SfxParam, Mathf.Log10(sfxV) * 30f);

        master.value = masterV;
        music.value = musicV;
        sfx.value = sfxV;

        SetVibration(!PlayerPrefs.HasKey(VibrationParam) || PlayerPrefs.GetInt(VibrationParam) > 0);
        SetFPS(!PlayerPrefs.HasKey(FPSParam) || PlayerPrefs.GetInt(FPSParam) > 0);
    }

    public void Mute()
    {
        mixer.SetFloat(MusicParam, 0f);
        mixer.SetFloat(SfxParam, 0f);
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(MasterParam, master.value);
        PlayerPrefs.SetFloat(MusicParam, music.value);
        PlayerPrefs.SetFloat(SfxParam, sfx.value);
        PlayerPrefs.SetInt(VibrationParam, Vibration.isEnabled ? 1 : 0);
        PlayerPrefs.SetInt(FPSParam, fpsCanvas.activeSelf ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        var resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        fullscreenCheckmark.SetActive(isFullscreen);
    }

    public void NegateFullscreen()
    {
        SetFullscreen(!Screen.fullScreen);
    }

    public void SetVibration(bool value)
    {
        Vibration.isEnabled = value;
        vibrationCheckmark.SetActive(value);
    }

    public void NegateVibration()
    {
        SetVibration(!Vibration.isEnabled);
    }

    public void SetFPS(bool value)
    {
        fpsCanvas.SetActive(value);
        fpsCheckmark.SetActive(value);
    }

    public void NegateFPS()
    {
        SetFPS(!fpsCanvas.activeSelf);
    }
}