using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SliderValue : MonoBehaviour
{
    [SerializeField] private Slider _masterSlider, _musicSlider, _sfxSlider;
    [SerializeField] private AudioMixer _masterMixer;
    
    private float _masterVolume, _musicVolume, _sfxVolume;

    // Start is called before the first frame update
    void Start()
    {
        _masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", _masterVolume);
        _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", _musicVolume);
        _sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", _sfxVolume);
    }

    #region SaveVolume

    public void SaveMasterVolume(float volume)
    {
        _masterVolume = volume;
        PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
    }

    public void SaveMusicVolume(float volume)
    {
        _musicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
    }

    public void SaveSfxVolume(float volume)
    {
        _sfxVolume = volume;
        PlayerPrefs.SetFloat("SfxVolume", _sfxVolume);
    }

    #endregion

    #region VolumeChanges

    public void ChangeMasterVolume(float volume)
    {
        _masterMixer.SetFloat("Master", volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        _masterMixer.SetFloat("MusicVolume", volume);
    }

    public void ChangeSfxVolume(float volume)
    {
        _masterMixer.SetFloat("SfxVolume", volume);
    }

    #endregion
}
