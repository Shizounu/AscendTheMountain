using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _sfxMixer;

    private Queue<AudioSource> _audioSourcePool = new Queue<AudioSource>();
    private List<AudioSource> _currentRunningAudioSources = new List<AudioSource>();

    public static AudioManager Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
            Instance = this;
        }
        else Instance = this;

        _audioSourcePool.Enqueue(GetComponent<AudioSource>());
    }

    private void Update()
    {
        if(_currentRunningAudioSources.Count > 0) // unfortunately there is no event for when an audio source finishes playing
        {
            foreach (AudioSource source in _currentRunningAudioSources)
            {
                if (!source.isPlaying) // so like that it works fine, even though I dont really like these kinda checks in the update xc
                {
                    ReturnSourceToPool(source);
                    return; // otherwise I get errors cause after the method call it would try to iterate through the loop again even though this source gets removed there
                }
            }
        }
    }

    /// <summary>
    /// if there are not enough AudioSources to play the clip another Source is added
    /// </summary>
    private void AddAudioSource()
    {
        var newSource = transform.AddComponent<AudioSource>();
        newSource.outputAudioMixerGroup = _sfxMixer;
        _audioSourcePool.Enqueue(newSource);
    }

    /// <summary>
    /// Gets an AudioSource from the Pool, enables and returns it
    /// </summary>
    /// <returns></returns>
    private AudioSource GetAudioSourceFromPool()
    {
        if(_audioSourcePool.Count == 0)
        {
            AddAudioSource();
        }

        var currentSource = _audioSourcePool.Dequeue();
        currentSource.enabled = true;
        return currentSource;
    }

    /// <summary>
    /// Plays the desired Audio Clip in an AudioSource on the AudioManager
    /// </summary>
    /// <param name="clip">the clip you want to play</param>
    public void PlayAudioClip(AudioClip clip)
    {
        var audioSource = GetAudioSourceFromPool();
        audioSource.clip = clip;
        audioSource.Play();
        _currentRunningAudioSources.Add(audioSource);
    }

    /// <summary>
    /// returns the AudioSource to the AudioSourcePool and disables it
    /// </summary>
    /// <param name="source"></param>
    public void ReturnSourceToPool(AudioSource source)
    {
        _currentRunningAudioSources.Remove(source);
        _audioSourcePool.Enqueue(source);
        source.clip = null;
        source.enabled = false;
    }
}
