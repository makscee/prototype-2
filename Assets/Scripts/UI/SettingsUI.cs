﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] GameObject fullscreenCheckmark;
    [SerializeField] AudioMixer mixer;
    [SerializeField] SplitSlider master, music, sfx;
    Resolution[] _resolutions;

    const string MasterParam = "MasterVolume", MusicParam = "MusicVolume", SfxParam = "SFXVolume";

    public void Init()
    {
        var currentResolutionIndex = 0;
        // _resolutions = Screen.resolutions;
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
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(MasterParam, master.value);
        PlayerPrefs.SetFloat(MusicParam, music.value);
        PlayerPrefs.SetFloat(SfxParam, sfx.value);
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
}