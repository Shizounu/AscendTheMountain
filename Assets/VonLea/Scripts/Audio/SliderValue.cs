using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private AudioMixer _masterMixer;
    
    private float _masterVolume, _musicVolume, _sfxVolume;

    private string _masterText = "Master";
    private string _musicText = "MusicVolume";
    private string _sfxText = "SfxVolume";

    // Start is called before the first frame update
    void Start()
    {
        _masterSlider.value = PlayerPrefs.GetFloat(_masterText, _masterVolume);
        _musicSlider.value = PlayerPrefs.GetFloat(_musicText, _musicVolume);
        _sfxSlider.value = PlayerPrefs.GetFloat(_sfxText, _sfxVolume);
    }

    #region SaveVolume

    public void SaveMasterVolume(float volume)
    {
        _masterVolume = volume;
        PlayerPrefs.SetFloat(_masterText, _masterVolume);
    }

    public void SaveMusicVolume(float volume)
    {
        _musicVolume = volume;
        PlayerPrefs.SetFloat(_musicText, _musicVolume);
    }

    public void SaveSfxVolume(float volume)
    {
        _sfxVolume = volume;
        PlayerPrefs.SetFloat(_sfxText, _sfxVolume);
    }

    #endregion

    #region VolumeChanges

    public void ChangeMasterVolume(float volume)
    {
        _masterMixer.SetFloat(_masterText, volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        _masterMixer.SetFloat(_musicText, volume);
    }

    public void ChangeSfxVolume(float volume)
    {
        _masterMixer.SetFloat(_sfxText, volume);
    }

    #endregion
}
