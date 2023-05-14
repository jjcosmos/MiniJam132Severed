using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _master;
    [SerializeField] private AudioMixerGroup _music;
    [SerializeField] private AudioMixerGroup _sfx;

    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private float _savedMasterSlider;
    private float _savedMusicSlider;
    private float _savedSfxSlider;

    private bool _initialized;

    public void Init()
    {
        if (_initialized) return;
        
        _savedMasterSlider = PlayerPrefs.GetFloat("MasterVol", 1f);
        _savedMusicSlider = PlayerPrefs.GetFloat("MusicVol", 1f);
        _savedSfxSlider = PlayerPrefs.GetFloat("SfxVol", 1f);
        
        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        
        _masterSlider.SetValueWithoutNotify(_savedMasterSlider);
        _musicSlider.SetValueWithoutNotify(_savedMusicSlider);
        _sfxSlider.SetValueWithoutNotify(_savedSfxSlider);
        
        SetMasterVolume(_savedMasterSlider);
        SetMusicVolume(_savedMusicSlider);
        SetSfxVolume(_savedSfxSlider);

        _initialized = true;
    }

    public void SetMasterVolume(float sliderVal)
    {
        var value = Mathf.Log10(sliderVal) * 20;
        _master.audioMixer.SetFloat("MasterVolume", value);
        _savedMasterSlider = sliderVal;
    }

    public void SetMusicVolume(float sliderVal)
    {
        var value = Mathf.Log10(sliderVal) * 20;
        _music.audioMixer.SetFloat("MusicVolume", value);
        _savedMusicSlider = sliderVal;
    }

    public void SetSfxVolume(float sliderVal)
    {
        var value = Mathf.Log10(sliderVal) * 20;
        _sfx.audioMixer.SetFloat("SfxVolume", value);
        _savedSfxSlider = sliderVal;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetFloat("MasterVol", _savedMasterSlider);
        PlayerPrefs.SetFloat("MusicVol", _savedMusicSlider);
        PlayerPrefs.SetFloat("SfxVol", _savedSfxSlider);
    }
}
